using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductPortrait
{
    internal static LPortraitMedium TPortraitMediumRead(CPortraitMedium medium) =>
        CPortrait.LPortraitMediumRead(medium);

    internal static LPressTicket TPortraitTicketRead(CPressTicket ticket) => CPortrait.LPortraitTicketRead(ticket);

    internal static LPortraitLabel TPortraitLabelRead(LSettingsPort settings) => CPortrait.LPortraitLabelRead(settings);

    internal static LPortraitLegend TPortraitLegendRead(LSettingsPort settings, string realm) =>
        CPortrait.LPortraitLegendRead(settings, realm);

    internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead() => CPortrait.LPortraitChoiceRead();

    internal static Task TPortraitFileExport(CEnvoy envoy, string file, Func<string, LPortraitMedium, Task> export) =>
        CPortrait.LPortraitFileExport(envoy, TInterfaceConduct.TSettingsCreate(), file, export);

    internal static Task TPortraitTicketPrint(CEnvoy envoy, Func<LPressTicket, Task> print) =>
        CPortrait.LPortraitTicketPrint(envoy, TInterfaceConduct.TSettingsCreate(), print);
}
