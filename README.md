<div align="center">

# DARK OPTIMIZER 2026 ⚡

### High-Performance WinUI System Optimizer for Windows 10/11

[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-0078D4?style=for-the-badge&logo=windows)](#)
[![Runtime](https://img.shields.io/badge/.NET-10%20Preview-512BD4?style=for-the-badge&logo=dotnet)](#)
[![UI](https://img.shields.io/badge/UI-WinUI%203-0A84FF?style=for-the-badge)](#)
[![AOT](https://img.shields.io/badge/Native-AOT-111111?style=for-the-badge)](#)
[![Status](https://img.shields.io/badge/Status-Experimental-8A2BE2?style=for-the-badge)](#)

<a href="#-quick-start"><img src="https://img.shields.io/badge/%F0%9F%9A%80-Quick%20Start-1f6feb?style=for-the-badge"/></a>
<a href="#-tutorial-generate-unpackaged-exe-for-github-releases"><img src="https://img.shields.io/badge/%F0%9F%93%A6-Build%20EXE-238636?style=for-the-badge"/></a>
<a href="#-architecture-map"><img src="https://img.shields.io/badge/%F0%9F%A7%A0-Architecture-bb6bd9?style=for-the-badge"/></a>

</div>

---

## ✨ Overview

Dark Optimizer 2026 is a WinUI 3 + .NET 10 Native AOT optimizer focused on deterministic execution, low overhead, and modular system tuning.

- **Native AOT + trimming friendly** (no runtime reflection discovery).
- **Unpackaged desktop mode** (no MSIX required for distribution).
- **Tiered optimization model**: Simple / Intermediate / Advanced / Supervised.
- **Mem Reduct-style Free RAM engine** using low-level Win32/NT operations.

---

## 🧭 Architecture Map

```text
DarkOptimizer.sln
├─ src/
│  ├─ DarkOptimizer.App/              # WinUI shell, startup bootstrap, commands
│  ├─ DarkOptimizer.Core/             # contracts, policies, orchestration
│  └─ DarkOptimizer.Infrastructure/   # interop + optimizer actions
└─ tests/
   └─ DarkOptimizer.Core.Tests/
```

### UI + Startup

- `{x:Bind}` across shell data paths.
- `x:Load` for deferred heavy cards.
- Skeleton state for async initialization.
- Manual Windows App SDK bootstrap (`MddBootstrapInitialize2`) before app startup.

### Optimization Engine

- Deterministic `OptimizationService` execution order.
- Static `OptimizationRegistry` (AOT-safe, no dynamic runtime scanning).
- Explicit elevation gating by action.

### Free RAM Engine (Mem Reduct Style)

- `SetProcessWorkingSetSizeEx` for process working set trims.
- `NtSetSystemInformation(SystemMemoryListInformation, ...)` for:
  - Empty working sets,
  - Purge standby list,
  - Flush modified list,
  - Purge low-priority standby list.
- Privilege-aware strategy ladder via `FreeRamPolicyEngine`.

---

## 📋 Build Requirements

- Windows 10 22H2+ or Windows 11.
- Visual Studio 2026 / MSBuild 18.0+ for `.NET 10` SDK feature bands.
- .NET 10 preview SDK (see `global.json`).
- Windows App SDK 1.8.x is the current stable servicing line (1.7 is maintenance/ended servicing in March 2026).

> ⚠️ `net10.0` + WinUI 3 + Native AOT remains a high-change toolchain. If your team needs maximum stability for CI and local builds, evaluate an LTS fallback branch on `net8.0-windows10.0.19041.0` while keeping WinUI toolchain versions fully supported.

---

## 🚀 Quick Start

```powershell
# 1) Restore
 dotnet restore DarkOptimizer.sln

# 2) Build
 dotnet build DarkOptimizer.sln -c Release

# 3) Test
 dotnet test tests/DarkOptimizer.Core.Tests/DarkOptimizer.Core.Tests.csproj -c Release
```

---

## 🧪 Build Diagnostics (binlog + XAML targets)

When you need to troubleshoot WinUI/XAML build issues, generate an MSBuild binary log:

```powershell
dotnet build DarkOptimizer.sln /bl
```

This writes `msbuild.binlog` in the current working directory. Then:

1. Open `msbuild.binlog` with [MSBuild Structured Log Viewer](https://msbuildlog.com/).
2. Focus on target/task execution around:
   - `MarkupCompilePass1`
   - `MarkupCompilePass2`
   - `XamlCompiler`
3. Compare failing node input properties/items with project settings such as:
   - `<UseWinUI>true</UseWinUI>`
   - `<UseXamlCompilerExecutable>false</UseXamlCompilerExecutable>`
   - `<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>`

If failures persist and are tied to preview SDK interactions, test a fallback branch with an LTS TFM that your team's VS + Windows App SDK toolchain fully supports.

---

## 📦 Tutorial: Generate Unpackaged `.exe` for GitHub Releases

This flow creates a release-ready artifact that can be uploaded directly to GitHub Releases.

### 1) Publish (Native AOT + self-contained + single-file)

```powershell
 dotnet publish src/DarkOptimizer.App/DarkOptimizer.App.csproj \
   -c Release \
   -r win-x64 \
   -p:PublishAot=true \
   -p:SelfContained=true \
   -p:PublishSingleFile=true \
   -p:WindowsAppSDKSelfContained=true \
   -p:IncludeNativeLibrariesForSelfExtract=true
```

Default output directory:

```text
src/DarkOptimizer.App/bin/Release/net10.0-windows10.0.19041.0/win-x64/publish/
```

### 2) Local Validation Checklist

- Run executable from publish folder.
- Confirm shell loads with no Windows App SDK runtime prompt.
- Confirm Free RAM action executes and updates UI summary.

### 3) Create Release ZIP

```powershell
Compress-Archive \
  -Path "src/DarkOptimizer.App/bin/Release/net10.0-windows10.0.19041.0/win-x64/publish/*" \
  -DestinationPath "DarkOptimizer-2026-win-x64.zip" \
  -Force
```

Upload `DarkOptimizer-2026-win-x64.zip` to the GitHub Release page.

### 4) Optional GitHub Actions Workflow

```yaml
name: release-win-x64

on:
  workflow_dispatch:

jobs:
  publish:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore DarkOptimizer.sln

      - name: Publish
        run: |
          dotnet publish src/DarkOptimizer.App/DarkOptimizer.App.csproj `
            -c Release `
            -r win-x64 `
            -p:PublishAot=true `
            -p:SelfContained=true `
            -p:PublishSingleFile=true `
            -p:WindowsAppSDKSelfContained=true `
            -p:IncludeNativeLibrariesForSelfExtract=true

      - name: Zip Artifact
        shell: pwsh
        run: |
          Compress-Archive \
            -Path "src/DarkOptimizer.App/bin/Release/net10.0-windows10.0.19041.0/win-x64/publish/*" \
            -DestinationPath "DarkOptimizer-2026-win-x64.zip" \
            -Force

      - name: Upload Artifact
        uses: actions/upload-artifact@v4
        with:
          name: DarkOptimizer-2026-win-x64
          path: DarkOptimizer-2026-win-x64.zip
```

---

## 🎛️ Technical Caveats

- NT memory-list commands may be restricted by endpoint hardening/policies.
- Advanced/Supervised tiers and full Free RAM strategy require elevation.
- Single-file WinUI AOT still depends on proper extraction/loading of bundled native assets.

---

<div align="center">

### 🖤 Built for speed, determinism, and clean system tuning.

</div>
