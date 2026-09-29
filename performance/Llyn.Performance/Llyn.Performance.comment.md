# Llyn.Performance.csproj

Builds the drills, the fixed workloads `scripts/performance.ps1` runs under the sampling profiler.

## `<OutputType>Exe</OutputType>`

The script starts the drills as their own process, so the runtime traces them without the test host.

## `<TargetFramework>net10.0</TargetFramework>`

The drills drive only portable projects, so they target the portable framework.

## `<ItemGroup>`

The language drill reads the packs from beside the binary, as the engine does.
The markup drill reads its fixture from beside the binary too.

## `<ProjectReference Include="../../src/Llyn.Core/Llyn.Core.csproj" />`

Core holds the markup records and the markup tree the markup drill round-trips.

## `<ProjectReference Include="../../src/Llyn.Infrastructure/Llyn.Infrastructure.csproj" />`

Infrastructure holds the markup text adapter and the language pack loader the drills exercise.
