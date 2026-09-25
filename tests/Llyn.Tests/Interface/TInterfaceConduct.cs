using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConduct
{
    internal static string TDisplayStampFormat(string? utc) => LDisplay.LDisplayStampFormat(utc);

    internal static LDisplaySound TDisplaySoundCreate(LEngine engine) => new(
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LMediaOutlet(engine),
        new LSettingsOutlet(engine));

    internal static void TDisplayFoldSet(this LDisplaySound sound, bool opened) => sound.LDisplayFoldSet(opened);

    internal static HashSet<string> TDisplayFoldScan(IReadOnlyList<LReflexRule> rules) =>
        LDisplaySound.LDisplayFoldScan(rules);

    internal static int TDisplayBandResolve(IReadOnlyList<LFrequency> rows) => LDisplay.LDisplayBandResolve(rows);

    internal static string TDisplaySourceFormat(IReadOnlyList<LFrequency> rows, string once) =>
        LDisplay.LDisplaySourceFormat(rows, once);
}
