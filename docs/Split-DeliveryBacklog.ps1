#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$docsRoot = $PSScriptRoot
$sourcePath = Join-Path $docsRoot 'delivery-backlog.md'
$outputRoot = Join-Path $docsRoot 'backlog'
$epicsRoot = Join-Path $outputRoot 'epics'
$tasksRoot = Join-Path $outputRoot 'tasks'

$existingOutput = @(Get-ChildItem -Path $outputRoot -File -Recurse -ErrorAction SilentlyContinue)
if ($existingOutput.Count -gt 0 -and -not $Force.IsPresent) {
    $message = "The split backlog already exists at '$outputRoot'. Regeneration resets task status and completion evidence. Rerun with -Force only when that data may be replaced."
    $exception = [System.InvalidOperationException]::new($message)
    $errorRecord = [System.Management.Automation.ErrorRecord]::new(
        $exception,
        'SplitBacklogAlreadyExists',
        [System.Management.Automation.ErrorCategory]::ResourceExists,
        $outputRoot
    )
    $PSCmdlet.ThrowTerminatingError($errorRecord)
}

function Normalize-MarkdownText {
    param([Parameter(Mandatory)][string]$Text)

    return ($Text -replace '\s+', ' ').Trim()
}

function ConvertTo-Title {
    param([Parameter(Mandatory)][string]$Description)

    return ($Description -split ';|\.(?:\s|$)', 2)[0].Trim()
}

function ConvertTo-YamlString {
    param([Parameter(Mandatory)][string]$Value)

    return "'" + $Value.Replace("'", "''") + "'"
}

function Get-ReferenceLinks {
    param([Parameter(Mandatory)][string]$Text)

    $references = [System.Collections.Generic.List[string]]::new()
    $knownDocuments = [ordered]@{
        'product-scope.md' = 'Product scope'
        'architecture-overview.md' = 'Architecture overview'
        'domain-model.md' = 'Domain model'
        'implementation-plan.md' = 'Implementation plan'
        'windows-service-reference.md' = 'Windows Service implementation reference'
        'api-contracts.md' = 'API contracts'
    }

    foreach ($document in $knownDocuments.Keys) {
        if (
            $Text.Contains($document, [System.StringComparison]::OrdinalIgnoreCase) -and
            (Test-Path (Join-Path $docsRoot $document))
        ) {
            $references.Add("- [$($knownDocuments[$document])](../../$document)")
        }
    }

    if ($references.Count -eq 0) {
        $references.Add('- [Delivery backlog](../../delivery-backlog.md)')
        $references.Add('- [Architecture overview](../../architecture-overview.md)')
        $references.Add('- [Domain model](../../domain-model.md)')
    }
    else {
        $references.Insert(0, '- [Delivery backlog](../../delivery-backlog.md)')
    }

    return $references
}

$lines = Get-Content $sourcePath
$epicSummary = @{}
$epics = [ordered]@{}
$currentEpic = $null

foreach ($line in $lines) {
    if ($line -match '^\| (EPIC-\d{2}) \| (.+?) \| (.+?) \| (Gate \d) \|$') {
        $epicSummary[$Matches[1]] = [ordered]@{
            Outcome = $Matches[2]
            Dependencies = $Matches[3]
            Gate = $Matches[4]
        }
        continue
    }

    if ($line -match '^## (EPIC-\d{2}): (.+)$') {
        $currentEpic = $Matches[1]
        $epics[$currentEpic] = [ordered]@{
            Id = $currentEpic
            Title = $Matches[2]
            OutcomeLines = [System.Collections.Generic.List[string]]::new()
            ExitLines = [System.Collections.Generic.List[string]]::new()
            Tasks = [System.Collections.Generic.List[object]]::new()
            ReadingOutcome = $false
            ReadingExit = $false
        }
        continue
    }

    if ($null -eq $currentEpic) {
        continue
    }

    if ($line -match '^## Release Gates$') {
        $currentEpic = $null
        continue
    }

    if ($line -match '^\*\*Outcome:\*\*\s*(.*)$') {
        $epics[$currentEpic].ReadingOutcome = $true
        $epics[$currentEpic].ReadingExit = $false
        $epics[$currentEpic].OutcomeLines.Add($Matches[1])
        continue
    }

    if ($line -match '^\*\*Epic exit:\*\*\s*(.*)$') {
        $epics[$currentEpic].ReadingOutcome = $false
        $epics[$currentEpic].ReadingExit = $true
        $epics[$currentEpic].ExitLines.Add($Matches[1])
        continue
    }

    if ($line -match '^\| (TASK-\d{4}) \| (.+?) \| (.+?) \| (.+?) \|$') {
        $epics[$currentEpic].ReadingOutcome = $false
        $epics[$currentEpic].ReadingExit = $false
        $epics[$currentEpic].Tasks.Add([ordered]@{
            Id = $Matches[1]
            DependenciesText = $Matches[2]
            Description = $Matches[3]
            Acceptance = $Matches[4]
            EpicId = $currentEpic
        })
        continue
    }

    if ([string]::IsNullOrWhiteSpace($line) -or $line.StartsWith('|')) {
        continue
    }

    if ($epics[$currentEpic].ReadingOutcome) {
        $epics[$currentEpic].OutcomeLines.Add($line)
    }
    elseif ($epics[$currentEpic].ReadingExit) {
        $epics[$currentEpic].ExitLines.Add($line)
    }
}

$tasks = [ordered]@{}
foreach ($epic in $epics.Values) {
    $epic.Outcome = Normalize-MarkdownText ($epic.OutcomeLines -join ' ')
    $epic.Exit = Normalize-MarkdownText ($epic.ExitLines -join ' ')
    $epic.Dependencies = $epicSummary[$epic.Id].Dependencies
    $epic.Gate = $epicSummary[$epic.Id].Gate

    foreach ($task in $epic.Tasks) {
        $task.Title = ConvertTo-Title $task.Description
        $task.Dependencies = @([regex]::Matches($task.DependenciesText, 'TASK-\d{4}') | ForEach-Object Value)
        $task.Downstream = [System.Collections.Generic.List[string]]::new()
        $tasks[$task.Id] = $task
    }
}

foreach ($task in $tasks.Values) {
    foreach ($dependency in $task.Dependencies) {
        $tasks[$dependency].Downstream.Add($task.Id)
    }
}

New-Item -ItemType Directory -Force -Path $epicsRoot, $tasksRoot | Out-Null

foreach ($task in $tasks.Values) {
    $dependencyYaml = if ($task.Dependencies.Count -eq 0) {
        '[]'
    }
    else {
        '[' + (($task.Dependencies | ForEach-Object { ConvertTo-YamlString $_ }) -join ', ') + ']'
    }
    $downstreamYaml = if ($task.Downstream.Count -eq 0) {
        '[]'
    }
    else {
        '[' + (($task.Downstream | Sort-Object | ForEach-Object { ConvertTo-YamlString $_ }) -join ', ') + ']'
    }
    $dependencyLinks = if ($task.Dependencies.Count -eq 0) {
        '- None. This task may start after the backlog rules and repository prerequisites are understood.'
    }
    else {
        @($task.Dependencies | ForEach-Object {
            "- [$_](./$_.md): $($tasks[$_].Title)"
        }) -join "`n"
    }
    $downstreamLinks = if ($task.Downstream.Count -eq 0) {
        '- None in the current backlog.'
    }
    else {
        @($task.Downstream | Sort-Object | ForEach-Object {
            "- [$_](./$_.md): $($tasks[$_].Title)"
        }) -join "`n"
    }
    $implementationRequirements = @($task.Description -split ';' | ForEach-Object {
        $requirement = $_.Trim()
        if ($requirement) {
            '- ' + $requirement.Substring(0, 1).ToUpperInvariant() + $requirement.Substring(1).TrimEnd('.') + '.'
        }
    }) -join "`n"
    $references = Get-ReferenceLinks ($task.Description + ' ' + $task.Acceptance)
    $referenceText = $references -join "`n"
    $epic = $epics[$task.EpicId]

    $taskContent = @"
---
id: $($task.Id)
title: $(ConvertTo-YamlString $task.Title)
type: task
status: not-started
epic: $($task.EpicId)
release_gate: $(ConvertTo-YamlString $epic.Gate)
depends_on: $dependencyYaml
unblocks: $downstreamYaml
---

# $($task.Id): $($task.Title)

## Objective

$($task.Description)

This task contributes to [$($task.EpicId): $($epic.Title)](../epics/$($task.EpicId).md)
and must preserve the epic outcome: $($epic.Outcome)

## Dependency Context

### Prerequisites

$dependencyLinks

### Tasks Unblocked

$downstreamLinks

## Scope

### Included

$implementationRequirements
- Update the owning implementation, tests, configuration, and maintained
  documentation needed to make the behavior complete.
- Preserve stable public contracts unless this task explicitly owns a
  versioned contract change.

### Excluded

- Work assigned to downstream tasks listed above.
- Opportunistic refactors that do not contribute to the objective or its
  acceptance evidence.
- Declaring adjacent backlog items complete without running their acceptance
  checks.

## Implementation Plan

1. Confirm that every prerequisite is complete and review its recorded
   acceptance evidence.
2. Inspect the current owning code, tests, configuration, and documentation;
   record whether this task adapts an existing surface or adds a new one.
3. Implement the requirements in the smallest owning component without
   bypassing established API, persistence, authorization, or deployment
   boundaries.
4. Add focused automated coverage for the success path, boundary conditions,
   expected failures, and relevant authorization or concurrency behavior.
5. Run the narrowest executable validation first, then the affected project
   suite and any release-gate checks named below.
6. Update contracts, runbooks, architecture, or operational guidance when the
   implemented behavior changes those surfaces.
7. Record evidence and mark the task complete only after every acceptance item
   is demonstrably satisfied.

## Deliverables

- The production or decision artifact described in the objective.
- Focused automated tests or deterministic validation for the changed
  behavior.
- Updated contract, configuration, architecture, or runbook documentation
  where applicable.
- Reviewable completion evidence containing commands, test results, or links
  to generated artifacts.

## Acceptance Criteria

- $($task.Acceptance)
- All prerequisite behavior remains passing after this change.
- No credentials, API keys, refresh tokens, or sensitive configuration values
  are committed, persisted in plaintext, or emitted to logs.
- Failures are explicit and actionable; the implementation does not silently
  discard telemetry, authorization failures, migration errors, or installer
  errors.

## Validation

1. Run focused tests that directly falsify the objective if it is incomplete.
2. Run integration tests for every changed database, HTTP, authentication,
   queue, or process boundary.
3. Run platform packaging tests when Windows Service or systemd behavior is
   affected; a successful compile alone is insufficient.
4. Run portal lint, unit, browser, and automated accessibility checks when UI
   behavior is affected, followed by a manual keyboard review.
5. Run the repository-level build and the checks required by $($epic.Gate).

## Completion Checklist

- [ ] Every prerequisite task is complete.
- [ ] The implementation requirements are satisfied.
- [ ] Focused tests cover success, boundary, and failure behavior.
- [ ] The acceptance evidence is captured and reviewable.
- [ ] Security, privacy, accessibility, and operational impacts were reviewed.
- [ ] Related documentation and contracts were updated.
- [ ] Required $($epic.Gate) checks pass.

## Related Documents

$referenceText
- [$($task.EpicId): $($epic.Title)](../epics/$($task.EpicId).md)
- [Backlog index](../README.md)
"@

    Set-Content -Path (Join-Path $tasksRoot "$($task.Id).md") -Value $taskContent -Encoding utf8
}

foreach ($epic in $epics.Values) {
    $taskRows = @($epic.Tasks | ForEach-Object {
        $dependencies = if ($_.Dependencies.Count -eq 0) {
            'None'
        }
        else {
            ($_.Dependencies | ForEach-Object { "[$_](../tasks/$_.md)" }) -join ', '
        }
        "| [$($_.Id)](../tasks/$($_.Id).md) | $($_.Title) | $dependencies | Not started |"
    }) -join "`n"

    $edges = [System.Collections.Generic.List[string]]::new()
    foreach ($task in $epic.Tasks) {
        if ($task.Dependencies.Count -eq 0) {
            $edges.Add("  START --> $($task.Id.Replace('-', '_'))")
            continue
        }

        $localDependencies = @($task.Dependencies | Where-Object { $tasks[$_].EpicId -eq $epic.Id })
        if ($localDependencies.Count -eq 0) {
            $edges.Add("  UPSTREAM --> $($task.Id.Replace('-', '_'))")
        }
        else {
            foreach ($dependency in $localDependencies) {
                $edges.Add("  $($dependency.Replace('-', '_')) --> $($task.Id.Replace('-', '_'))")
            }
        }
    }
    $nodes = @($epic.Tasks | ForEach-Object {
        "  $($_.Id.Replace('-', '_'))[$($_.Id)]"
    }) -join "`n"
    $edgeText = ($edges | Sort-Object -Unique) -join "`n"
    $epicDependencyLinks = if ($epic.Dependencies -eq 'None') {
        '- None.'
    }
    else {
        @([regex]::Matches($epic.Dependencies, 'EPIC-\d{2}') | ForEach-Object {
            $dependencyId = $_.Value
            "- [$dependencyId](./$dependencyId.md): $($epics[$dependencyId].Title)"
        }) -join "`n"
    }
    $epicContent = @"
---
id: $($epic.Id)
title: $(ConvertTo-YamlString $epic.Title)
type: epic
status: not-started
release_gate: $(ConvertTo-YamlString $epic.Gate)
depends_on: $(ConvertTo-YamlString $epic.Dependencies)
---

# $($epic.Id): $($epic.Title)

## Outcome

$($epic.Outcome)

## Epic Completion Dependencies

$epicDependencyLinks
- Each listed epic must have reviewable acceptance evidence before this epic can be declared complete. Individual tasks may start earlier when all of their task-level prerequisites are complete.
- Cross-epic contracts consumed by this work are versioned and stable enough
  for the tasks below.
- Required development and test environments are available.

## Task Dependency Tree

~~~mermaid
flowchart TD
  START([Epic may start])
  UPSTREAM([External prerequisites complete])
$nodes
$edgeText
~~~

`UPSTREAM` represents dependencies owned by another epic. Open an individual
task file to follow every concrete predecessor link.

## Tasks

| Task | Objective | Depends on | Initial status |
|---|---|---|---|
$taskRows

## Execution Guidance

- Begin only tasks whose linked predecessors are complete.
- Tasks with all predecessors satisfied may proceed in parallel.
- Keep implementation inside the owning architecture boundary; use the shared
  contracts rather than creating an epic-specific integration surface.
- Validate each task independently before starting a dependent task.
- Treat task-file acceptance criteria as required evidence, not illustrative
  guidance.

## Epic Acceptance

- Every task in this epic is complete with linked evidence.
- $($epic.Exit)
- The affected solution, integration, functional, and packaging suites pass.
- Security, accessibility, performance, observability, and operational review
  findings are resolved or explicitly accepted.
- $($epic.Gate) evidence is updated when this epic contributes to that gate.

## Related Documents

- [Backlog index](../README.md)
- [Delivery backlog and dependency tree](../../delivery-backlog.md)
- [Implementation plan](../../implementation-plan.md)
- [Architecture overview](../../architecture-overview.md)
- [Domain model](../../domain-model.md)
"@

    Set-Content -Path (Join-Path $epicsRoot "$($epic.Id).md") -Value $epicContent -Encoding utf8
}

$epicRows = @($epics.Values | ForEach-Object {
    "| [$($_.Id)](./epics/$($_.Id).md) | $($_.Title) | $($_.Dependencies) | $($_.Gate) | $($_.Tasks.Count) |"
}) -join "`n"

$remainingTasks = [System.Collections.Generic.HashSet[string]]::new([string[]]$tasks.Keys)
$scheduledTasks = [System.Collections.Generic.HashSet[string]]::new()
$executionWaves = [System.Collections.Generic.List[object]]::new()
$waveNumber = 0

while ($remainingTasks.Count -gt 0) {
    $readyTasks = @($remainingTasks | Where-Object {
        $taskId = $_
        @($tasks[$taskId].Dependencies | Where-Object {
            -not $scheduledTasks.Contains($_)
        }).Count -eq 0
    } | Sort-Object)

    if ($readyTasks.Count -eq 0) {
        throw "The task dependency graph contains a cycle: $($remainingTasks -join ', ')."
    }

    $waveNumber++
    $executionWaves.Add([ordered]@{
        Number = $waveNumber
        Tasks = $readyTasks
    })

    foreach ($taskId in $readyTasks) {
        [void]$scheduledTasks.Add($taskId)
        [void]$remainingTasks.Remove($taskId)
    }
}

$waveSections = @($executionWaves | ForEach-Object {
    $wave = $_
    $rows = @($wave.Tasks | ForEach-Object {
        $task = $tasks[$_]
        "| [$($task.Id)](./tasks/$($task.Id).md) | [$($task.EpicId)](./epics/$($task.EpicId).md) | $($task.Title) |"
    }) -join "`n"
    @"
## Wave $($wave.Number)

Start these tasks after every task in earlier waves that they depend on is
complete. Tasks in this wave have no dependency on one another and may execute
in parallel when staffing and environments allow.

| Task | Epic | Objective |
|---|---|---|
$rows
"@
}) -join "`n`n"

$dependencyTreeContent = @"
# Task Dependency Execution Tree

## Purpose

This document is the topological execution order for all $($tasks.Count) tasks.
It complements the program-level epic graph in
[delivery-backlog.md](../delivery-backlog.md) and the local task graphs in each
[epic file](./epics/).

## How To Use The Waves

- Complete all declared prerequisite tasks before starting a dependent task.
- Task-file ``depends_on`` metadata controls task start order. Epic dependencies control epic completion and release-gate readiness.
- A wave is a scheduling aid, not a mandatory sprint boundary.
- Work from later waves may begin as soon as its own predecessors are done; it
  does not need to wait for unrelated work in every earlier wave.
- Tasks listed in the same wave are dependency-independent and may run in
  parallel.
- Follow each task link for detailed scope, implementation steps, validation,
  and acceptance evidence.

## Summary

| Measure | Value |
|---|---:|
| Epics | $($epics.Count) |
| Tasks | $($tasks.Count) |
| Execution waves | $($executionWaves.Count) |

$waveSections
"@

Set-Content -Path (Join-Path $outputRoot 'dependency-tree.md') -Value $dependencyTreeContent -Encoding utf8

$taskIndexRows = @($tasks.Values | ForEach-Object {
    $dependencies = if ($_.Dependencies.Count -eq 0) {
        'None'
    }
    else {
        ($_.Dependencies | ForEach-Object { "[$_](./tasks/$_.md)" }) -join ', '
    }
    "| [$($_.Id)](./tasks/$($_.Id).md) | [$($_.EpicId)](./epics/$($_.EpicId).md) | $($_.Title) | $dependencies |"
}) -join "`n"

$indexContent = @"
# Delivery Backlog

## Purpose

This directory is the canonical task-level execution tree for System Uptime
Tracker. Every epic and task has its own file so implementation, review, and
completion evidence can be managed without editing one monolithic document.

Use [the delivery overview](../delivery-backlog.md) for the full program graph,
critical path, release gates, definition of done, and traceability matrix.

The split files are working records. ``Split-DeliveryBacklog.ps1`` refuses to
overwrite them by default because regeneration resets task status and
completion evidence. Use its ``-Force`` switch only when replacing that data is
intentional.

## Navigation

- [Epic files](./epics/)
- [Task files](./tasks/)
- [Task dependency execution tree](./dependency-tree.md)
- [Implementation plan](../implementation-plan.md)
- [Architecture overview](../architecture-overview.md)
- [Domain model](../domain-model.md)

## Dependency Rules

- Start a task only after every linked prerequisite task is complete.
- Task-file ``depends_on`` metadata controls task start order. Epic dependencies control epic completion and release-gate readiness.
- Tasks with satisfied predecessors may run in parallel, including tasks from
  different epics.
- Complete the acceptance evidence in the task file before marking it done.
- Keep IDs and file names stable; add IDs rather than renumbering existing
  work.
- Update the epic file and this index whenever a task is added, removed, or
  assigned a different predecessor.

## Epics

| Epic | Title | Completion depends on | Release gate | Tasks |
|---|---|---|---|---:|
$epicRows

## Tasks

| Task | Epic | Objective | Depends on |
|---|---|---|---|
$taskIndexRows
"@

Set-Content -Path (Join-Path $outputRoot 'README.md') -Value $indexContent -Encoding utf8

[pscustomobject]@{
    Epics = $epics.Count
    Tasks = $tasks.Count
    OutputRoot = $outputRoot
}