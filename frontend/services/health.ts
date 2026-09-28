import type { HealthResponse } from "@/types/health";

function isHealthResponse(value: unknown): value is HealthResponse {
  return (
    typeof value === "object" &&
    value !== null &&
    "status" in value &&
    value.status === "healthy"
  );
}

export async function getHealth(signal: AbortSignal): Promise<HealthResponse> {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  if (!baseUrl) {
    throw new Error("The service connection is not configured.");
  }

  let url: URL;

  try {
    url = new URL(`${baseUrl.replace(/\/+$/, "")}/api/health`);
    if (url.protocol !== "http:" && url.protocol !== "https:") {
      throw new Error("Unsupported API protocol.");
    }
  } catch {
    throw new Error("The service connection is not configured correctly.");
  }

  const requestSignal = AbortSignal.any([signal, AbortSignal.timeout(5_000)]);

  try {
    const response = await fetch(url, {
      method: "GET",
      headers: { Accept: "application/json" },
      cache: "no-store",
      credentials: "omit",
      signal: requestSignal,
    });

    if (!response.ok) {
      throw new Error("The service is temporarily unavailable. Please try again.");
    }

    const data: unknown = await response.json();

    if (!isHealthResponse(data)) {
      throw new Error("The service returned an unexpected response. Please try again.");
    }

    return data;
  } catch (error) {
    // Preserve cancellation so the component can ignore a request after unmount.
    if (signal.aborted) {
      throw error;
    }

    if (requestSignal.aborted) {
      throw new Error("The connection timed out. Please try again.");
    }

    if (error instanceof SyntaxError) {
      throw new Error("The service returned an unexpected response. Please try again.");
    }

    if (error instanceof TypeError) {
      throw new Error("Unable to reach the service. Please try again shortly.");
    }

    throw error;
  }
}
