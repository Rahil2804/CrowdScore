import { HealthStatus } from "@/components/health-status";

export default function Home() {
  return (
    <div className="mx-auto flex min-h-dvh max-w-6xl flex-col px-6 sm:px-10">
      <header className="flex items-center gap-3 border-b border-white/10 py-7">
        <span
          aria-hidden="true"
          className="flex h-9 w-9 items-center justify-center rounded-lg bg-lime-300 text-lg font-black text-stone-950"
        >
          C
        </span>
        <span className="text-xl font-bold tracking-tight">CrowdScore</span>
      </header>

      <main className="grid flex-1 items-center gap-12 py-16 lg:grid-cols-[1.2fr_1fr] lg:gap-20">
        <div>
          <p className="mb-6 text-xs font-semibold uppercase tracking-[0.2em] text-lime-300">
            For the fans. Round by round.
          </p>
          <h1 className="max-w-xl text-5xl leading-[1.05] font-bold tracking-tight sm:text-7xl">
            Every round <span className="text-stone-400">counts.</span>
          </h1>
          <p className="mt-7 max-w-md text-base leading-7 text-stone-400 sm:text-lg">
            A new corner for MMA fans. We’re building a place to score the action
            and see the fight together.
          </p>
          <p className="mt-8 text-sm text-stone-500">The first bell is still ahead. Stay tuned.</p>
        </div>

        <HealthStatus />
      </main>

      <footer className="border-t border-white/10 py-6 text-xs text-stone-500">
        CrowdScore · Built for a closer look at every fight.
      </footer>
    </div>
  );
}
