using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPortraitPort
{
    Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket);

    Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket);

    Task LEnginePortraitExport(LVista? vista, string path, LPortraitMedium format, LPortraitLabel label);

    Task<LMarkupCargo> LEngineMarkupStart(string path);

    Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes);

    static LPressTicket LEngineTicketRead(
        string printer,
        double? width,
        double? height,
        bool landscape,
        int copies,
        bool collated,
        LPressSide side,
        LPressInk ink)
    {
        return LPortraitClerk.LPortraitTicketCreate(printer, width, height, landscape, copies, collated, side, ink);
    }

    static LPortraitLabel LEngineLabelRead(IReadOnlyList<string> words)
    {
        return LPortraitClerk.LPortraitLabelCreate(words);
    }

    static LPortraitLegend LEngineLegendRead(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)
    {
        return LPortraitClerk.LPortraitLegendCreate(words, kinds);
    }
}
