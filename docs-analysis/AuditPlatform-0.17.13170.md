# Platform audit - Llyn 0.17.13170

- Generation: 19
- Enforced: true
- Above ceiling: 0
- Stale ceilings: 0
- Unreadable files: 0

## Table

| Project | Role | Column | On disk | Targets | References |
|---|---|---|---|---|---|
| Llyn.Application | portable | Llyn.Application | yes | net10.0 | Llyn.Core |
| Llyn.Conduct | portable | Llyn.Conduct | yes | net10.0 | Llyn.ShellEngine |
| Llyn.Core | portable | Llyn.Core | yes | net10.0 |  |
| Llyn.Core.Windows | twin | Llyn.Core | yes | net10.0-windows | Llyn.Core |
| Llyn.Host | host |  | yes | net10.0-windows10.0.17763.0 | Llyn.Core, Llyn.Application, Llyn.Infrastructure, Llyn.Core.Windows, Llyn.ShellEngine, Llyn.Conduct, Llyn.UIDeportment, Llyn.UIVeneer, Llyn.UIDemeanor, Llyn.UITerminal |
| Llyn.Infrastructure | portable | Llyn.Infrastructure | yes | net10.0 | Llyn.Core |
| Llyn.ShellEngine | portable | Llyn.ShellEngine | yes | net10.0 | Llyn.Application |
| Llyn.UIDemeanor | portable | Llyn.UIDemeanor | yes | net10.0 | Llyn.Conduct |
| Llyn.UIDeportment | twin | Llyn.Conduct | yes | net10.0-windows10.0.17763.0 | Llyn.Conduct, Llyn.UIDeportment.Capsule |
| Llyn.UIDeportment.Capsule | portable | Llyn.UIDeportment.Capsule | yes | net10.0 |  |
| Llyn.UITerminal | portable | Llyn.UITerminal | yes | net10.0 | Llyn.UIDemeanor |
| Llyn.UIVeneer | twin | Llyn.Conduct | yes | net10.0-windows10.0.17763.0 | Llyn.UIDeportment |

## Counts

| Kind | Hits | Ceiling | Standing |
|---|---|---|---|
| Unmapped | 0 | 0 | at ceiling |
| Absent | 0 | 0 | at ceiling |
| Framework | 0 | 0 | at ceiling |
| Reference | 0 | 0 | at ceiling |
| Column | 0 | 0 | at ceiling |
| Analyzer | 0 | 0 | at ceiling |
| Windows | 0 | 0 | at ceiling |
| Empty | 0 | 0 | at ceiling |
| Suppress | 0 | 0 | at ceiling |
| Implicit | 0 | 0 | at ceiling |
| Domain | 0 | 0 | at ceiling |
| Total | 0 | | |
