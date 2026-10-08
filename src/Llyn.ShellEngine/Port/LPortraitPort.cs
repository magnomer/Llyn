using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPortraitPort
{
    Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket);

    Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket);

    Task LEnginePortraitExport(LVista? vista, string path, LPortraitMedium format, LPortraitLabel label);

    Task<IReadOnlyList<LMarkupOmission>?> LEngineMarkupStart(
        string path,
        Func<
            IReadOnlyList<LMarkupEntry>,
            IReadOnlyList<IReadOnlyList<LMarkupTarget>>,
            IReadOnlyList<LMarkupIntake>?> declare);

    bool LEngineCourierCheck();

    Task<LReceipt> LEngineCourierSend(Func<string, string> lookup, CancellationToken cancellation);

    Task LEngineCourierAttach(CancellationToken cancellation);

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
        return LPortraitClerkPress.LPortraitTicketCreate(
            printer, width, height, landscape, copies, collated, side, ink);
    }

    static LPortraitLabel LEngineLabelRead(IReadOnlyList<string> words)
    {
        return LPortraitClerkLabel.LPortraitLabelCreate(words);
    }

    static IReadOnlyList<(LPortraitMedium, string, bool)> LEngineMediumRead()
    {
        return LPortraitClerkPress.LPortraitMediumRead();
    }

    static IReadOnlyList<string> LEngineKindRead()
    {
        return LPortraitClerkLabel.LPortraitKindRead();
    }

    static LMarkupIntake LEngineIntakeRead(int index, LMarkupMode mode, long target)
    {
        return LMarkupClerkIntake.LMarkupIntakeCreate(index, mode, target);
    }

    static LPortraitLegend LEngineLegendRead(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)
    {
        return LPortraitClerkLabel.LPortraitLegendCreate(words, kinds);
    }
}
