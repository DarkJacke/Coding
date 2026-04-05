# DARK OPTIMIZER 2026 ⚡

> High-performance Windows 10/11 optimizer built for **Native AOT**, **strict trimming**, and **deterministic execution**.

## Why this project

Dark Optimizer 2026 targets the engineering gap between modern WinUI UX and low-level system tuning. The solution is split for isolation, testability, and predictable startup behavior on `win-x64`.

- **UI stack**: WinUI 3 (Windows App SDK), Fluent visuals, Mica backdrop, skeleton loading.
- **Architecture**: MVVM with source generators + code-behind only for hot UI interactions.
- **Execution model**: static registration, non-reflective pipelines, linker-safe contracts.
- **Memory module**: Mem Reduct-inspired RAM reduction primitives via Win32/NT interop.

---

## Solution layout

```text
DarkOptimizer.sln
├─ src/
│  ├─ DarkOptimizer.App/              # WinUI shell, sidebar navigation, skeleton states
│  ├─ DarkOptimizer.Core/             # contracts, orchestration, policy engines
│  └─ DarkOptimizer.Infrastructure/   # Win32/NT interop and action implementations
└─ tests/
   └─ DarkOptimizer.Core.Tests/       # pipeline and policy tests
```

---

## Dashboard architecture (modular)

### Shell UX

- Left navigation sidebar with strongly-typed items and `{x:Bind}` everywhere.
- Deferred heavy panels using `x:Load` to reduce memory at startup.
- Async skeleton placeholders while telemetry/memory sections initialize.
- Fast code-behind route switching (`ShellPage.xaml.cs`) to avoid extra allocations.

### Tiered OptimizationService

`OptimizationService` executes ordered actions per tier with:

1. explicit elevation checks,
2. cooperative cancellation,
3. deterministic result aggregation (`ActionResult[]`).

Tiers:

- **Simple** → temp cleanup, visual effects tuning, startup scan.
- **Intermediate** → service orchestration plan, scheduled maintenance profile, telemetry profile.
- **Advanced** → registry profile, memory compression targeting, I/O priority plan.
- **Supervised** → deep debloat plan, BCD profile prep, kernel power scheme profile.

> Registration is static in `OptimizationRegistry` (no runtime discovery, no reflection).

---

## Free RAM section (Mem Reduct-style primitives)

Implemented through `IFreeRamService` + `FreeRamPolicyEngine`:

- `ReduceProcessWorkingSetsAsync` → `SetProcessWorkingSetSizeEx(-1, -1, ...)`
- `EmptyWorkingSetsAsync` → `NtSetSystemInformation(SystemMemoryListInformation, MemoryEmptyWorkingSets)`
- `PurgeStandbyListAsync` → standby purge command
- `PurgeModifiedPageListAsync` → modified list flush command
- `PurgeLowPriorityStandbyListAsync` → low-priority standby purge
- `CombinedAggressiveTrimAsync` → chained high-impact strategy

Policy behavior:

- non-elevated: runs safe per-process working set trim,
- elevated: adds global memory list commands,
- elevated + profile privilege: enables modified/low-priority list purges.

---

## Native AOT / Trimming notes

- `PublishAot=true`, `IsAotCompatible=true`, `InvariantGlobalization=true`
- `LangVersion=preview`, trim analyzers enabled, warnings-as-errors
- No reflection-based registration paths in optimization/free-ram pipelines

---

## Build and test

```powershell
# restore
 dotnet restore DarkOptimizer.sln

# build
 dotnet build DarkOptimizer.sln -c Debug

# tests
 dotnet test tests/DarkOptimizer.Core.Tests/DarkOptimizer.Core.Tests.csproj

# publish Native AOT
 dotnet publish src/DarkOptimizer.App/DarkOptimizer.App.csproj -c Release -r win-x64
```

---

## Technical caveats (max 3)

- NT memory list commands can be policy/build dependent across Windows 10/11 revisions.
- Advanced and Supervised tiers require elevation; run from an elevated host for full effects.
- Real-world startup/RAM targets depend on final packaging, R2R/AOT profile, and enabled modules.
