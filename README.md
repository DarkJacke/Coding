# Dark optimizer 2026

Optimizador modular para Windows 10/11 orientado a rendimiento extremo, arquitectura no reflectiva y despliegue Native AOT.

## Stack principal

- **UI**: WinUI 3 (Windows App SDK) con Fluent, fondo Mica y sidebar moderna.
- **Arquitectura**: MVVM con Source Generators (`CommunityToolkit.Mvvm`) + lógica de interacción caliente en code-behind.
- **Runtime**: `.NET 10` preview, trimming estricto y banderas de compatibilidad AOT.
- **Módulos**:
  - Dashboard
  - Optimization (Simple / Intermediate / Advanced / Supervised)
  - Free RAM (mecanismos estilo Mem Reduct)

## Estructura

```text
DarkOptimizer.sln
src/
  DarkOptimizer.App/             # WinUI shell + navegación
  DarkOptimizer.Core/            # contratos, modelos, motor de políticas
  DarkOptimizer.Infrastructure/  # interop Win32/NT + registro estático de acciones
tests/
  DarkOptimizer.Core.Tests/
```

## Diseño de rendimiento

- Todo binding de UI usa **`{x:Bind}`** en la shell.
- Se difiere contenido pesado con **`x:Load`** (skeleton y paneles avanzados).
- Registro de acciones de optimización por **arreglos estáticos** (sin reflexión, seguro para trimming).
- Interop nativo con **`[LibraryImport]`** para minimizar marshalling en runtime.

## Módulo Optimization

El motor ejecuta acciones por `OptimizationTier`:

1. **Simple**: limpieza temporal, efectos visuales, startup apps.
2. **Intermediate**: servicios, mantenimiento programado, telemetría.
3. **Advanced**: tweaks de registro, memoria comprimida, prioridad de I/O.
4. **Supervised**: debloat profundo, BCD y power scheme de kernel.

El `OptimizationService` aplica:
- gating por privilegios,
- cancelación cooperativa,
- resultados determinísticos por acción (`ActionResult`).

## Módulo Free RAM (inspirado en Mem Reduct)

Se implementan estrategias de liberación:

- `ReduceProcessWorkingSetsAsync`  
  Usa `SetProcessWorkingSetSizeEx` con `-1/-1` para trim de working set.
- `PurgeStandbyListAsync`  
  Invoca `NtSetSystemInformation(SystemMemoryListInformation, MemoryPurgeStandbyList)`.
- `CombinedAggressiveTrimAsync`  
  Encadena trim + `MemoryEmptyWorkingSets` + `MemoryPurgeLowPriorityStandbyList`.
- `GetMemorySnapshotAsync`  
  Lectura de métricas desde `GlobalMemoryStatusEx`.

`FreeRamPolicyEngine` decide fallback según privilegios, ejecutando en orden seguro.

## Requisitos de build (Windows)

- Windows 11 (recomendado) o Windows 10 22H2+
- Visual Studio 2022 17.10+ con workload:
  - .NET Desktop
  - Windows App SDK / WinUI
- SDK de .NET 10 preview (`global.json`)

## Comandos

```powershell
# Restaurar
 dotnet restore DarkOptimizer.sln

# Build Debug
 dotnet build DarkOptimizer.sln -c Debug

# Tests
 dotnet test tests/DarkOptimizer.Core.Tests/DarkOptimizer.Core.Tests.csproj

# Publicar AOT (x64)
 dotnet publish src/DarkOptimizer.App/DarkOptimizer.App.csproj -c Release -r win-x64
```

## Caveats técnicos

- Requiere elevación para estrategias avanzadas de RAM y tiers de mayor impacto.
- Algunas llamadas NT (`NtSetSystemInformation`) pueden variar por build de Windows/políticas locales.
- Proyecto orientado a Native AOT: evita reflexión dinámica y registro runtime implícito.
