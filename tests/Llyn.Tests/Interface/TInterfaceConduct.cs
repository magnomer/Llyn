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

    internal static string TDisplayBandRead(int count) => LDisplay.LDisplayBandRead(count, string.Empty);

    internal static void TDisplaySoundShow(this LDisplaySound sound, long? id, LEntryDraft draft) =>
        sound.LDisplaySoundShow(id, draft);

    internal static void TDisplaySoundClear(this LDisplaySound sound) => sound.LDisplaySoundClear();

    internal static void TDisplayReflexLoad(this LDisplaySound sound) => sound.LDisplayReflexLoad();

    internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this LDisplaySound sound) =>
        sound.LDisplayReflexRead();

    internal static string TDisplaySourceFormat(IReadOnlyList<LFrequency> rows, string once) =>
        LDisplay.LDisplaySourceFormat(rows, once);
}
