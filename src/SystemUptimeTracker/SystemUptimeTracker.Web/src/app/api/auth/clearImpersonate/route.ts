import { NextResponse } from "next/server";

import { createLogger } from "@/utils/logger-server";

const cookieName = process.env.IMPERSONATING_COOKIE;

const GET = async () => {
  const log = await createLogger("ClearImpersonateApiRoute");
  const response = new NextResponse(null, {
    status: 200,
  });

  // cookies.delete throws on a non-string name, and IMPERSONATING_COOKIE is
  // legitimately unset when impersonation is not configured.
  if (cookieName) {
    response.cookies.delete(cookieName);
    response.cookies.delete(`${cookieName}-data`);
    await log.info("Cleared impersonation cookies");
  } else {
    await log.info(
      "Impersonation cookie name is not configured; nothing to clear",
    );
  }

  return response;
};

export { GET };
