"use client";

import { useEffect, useState } from "react";
import { getHealth } from "@/services/health";
import type { HealthResponse } from "@/types/health";

type HealthState =
  | { kind: "loading" }
  | { kind: "success"; data: HealthResponse }
  | { kind: "error"; message: string };

export function HealthStatus() {
  const [state, setState] = useState<HealthState>({ kind: "loading" });
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    async function checkHealth() {
      try {
        const data = await getHealth(controller.signal);
        if (!controller.signal.aborted) {
          setState({ kind: "success", data });
        }
      } catch (error) {
        if (!controller.signal.aborted) {
          setState({
            kind: "error",
            message:
              error instanceof Error
                ? error.message
                : "Unable to check the service. Please try again.",
          });
        }
      }
    }

    void checkHealth();

    return () => controller.abort();
  }, [attempt]);

  function retry() {
    setState({ kind: "loading" });
    setAttempt((current) => current + 1);
  }

  const indicatorColor =
    state.kind === "success"
      ? "bg-lime-300"
      : state.kind === "error"
        ? "bg-amber-300"
        : "bg-stone-400 motion-safe:animate-pulse";

  return (
    <section
      aria-labelledby="service-status-heading"
      className="rounded-2xl border border-white/10 bg-white/5 p-6 sm:p-8"
    >
      <div className="mb-6 flex items-center justify-between gap-4">
        <h2
          id="service-status-heading"
          className="text-sm font-medium tracking-wide text-stone-300"
        >
          Service status
        </h2>
        <span aria-hidden="true" className={`h-2.5 w-2.5 rounded-full ${indicatorColor}`} />
      </div>

      <div role="status" aria-live="polite" aria-atomic="true" className="min-h-24">
        {state.kind === "loading" && (
          <>
            <p className="text-2xl font-semibold text-stone-100">Connecting…</p>
            <p className="mt-3 text-sm leading-6 text-stone-400">
              Checking the CrowdScore service.
            </p>
          </>
        )}
        {state.kind === "success" && (
          <>
            <p className="text-2xl font-semibold text-lime-300">{state.data.status}</p>
            <p className="mt-3 text-sm leading-6 text-stone-400">
              CrowdScore is connected and ready.
            </p>
          </>
        )}
        {state.kind === "error" && (
          <>
            <p className="text-2xl font-semibold text-amber-300">Connection unavailable</p>
            <p className="mt-3 text-sm leading-6 text-stone-400">{state.message}</p>
          </>
        )}
      </div>

      {state.kind === "error" && (
        <button
          type="button"
          onClick={retry}
          className="mt-6 rounded-lg bg-stone-100 px-4 py-2.5 text-sm font-semibold text-stone-950 transition hover:bg-white focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-lime-300"
        >
          Retry
        </button>
      )}
    </section>
  );
}
