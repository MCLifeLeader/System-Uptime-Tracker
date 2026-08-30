import { afterEach, describe, expect, it, vi } from "vitest";

import { sanitizeMultipartDispositionValue } from "./multipart";

const cookiesGet = vi.fn((name: unknown) => {
  if (typeof name !== "string") {
    // Next 16.3+ behavior: reading a cookie without a string name throws.
    throw new TypeError("Cannot read properties of undefined (reading 'name')");
  }
  return undefined;
});

vi.mock("next/headers", () => ({
  cookies: vi.fn().mockResolvedValue({
    get: (name: unknown) => cookiesGet(name),
    getAll: () => [],
  }),
}));

vi.mock("@/utils/auth/auth", () => ({
  auth: vi.fn(() => ({
    getAccessToken: vi.fn().mockResolvedValue(undefined),
  })),
}));

vi.mock("@/utils/encryption", () => ({
  decrypt: vi.fn(),
}));

vi.mock("@/utils/error-reference", () => ({
  createTraceId: vi.fn(() => "generatedtraceid0000000000000000"),
  createTraceableError: vi.fn((message) => new Error(message)),
  extractTraceId: vi.fn(() => undefined),
  getPublicErrorDetail: vi.fn(() => "The request could not be completed."),
}));

vi.mock("@/utils/logger-server", () => ({
  createLogger: vi.fn().mockResolvedValue({
    warn: vi.fn().mockResolvedValue(undefined),
    error: vi.fn().mockResolvedValue(undefined),
  }),
}));

describe("sanitizeMultipartDispositionValue", () => {
  it("removes CRLF characters and escapes quotes before values are written into multipart headers", () => {
    expect(
      sanitizeMultipartDispositionValue('field"name\r\nX-Injected: true'),
    ).toBe("field%22name X-Injected: true");
  });
});

describe("serverApiGet impersonation guard", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.unstubAllEnvs();
    vi.resetModules();
  });

  it("does not read the impersonation cookie when IMPERSONATING_COOKIE is not configured", async () => {
    vi.resetModules();
    vi.stubEnv("API_BASE_URL", "http://localhost:7061/");
    vi.stubEnv("IMPERSONATING_COOKIE", undefined);
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue(
        new Response("[]", {
          status: 200,
          headers: { "content-type": "application/json" },
        }),
      ),
    );
    cookiesGet.mockClear();

    const { serverApiGet } = await import("./server-api");
    const response = await serverApiGet({ url: "api/users" });

    expect(response).not.toBe("unauthorized");
    expect((response as Response).status).toBe(200);
    expect(cookiesGet).not.toHaveBeenCalled();
  });
});
