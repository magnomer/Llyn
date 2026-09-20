using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPortraitPort
{
    Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket);

    Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket);

    Task LEnginePortraitExport(LVista? vista, string path, LPortraitFormat format, LPortraitLabel label);

    Task<LMarkupCargo> LEngineMarkupStart(string path);

    Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes);
}
