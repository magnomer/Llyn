using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Performance;

internal sealed class LDrillMarkup : LDrill
{
    private const string LDrillMarkupFixture = "Fixture/markup.xml";

    private const int LDrillMarkupCopies = 400;

    private string _lDrillMarkupText = string.Empty;

    public override void LDrillPrepare()
    {
        string sample = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, LDrillMarkupFixture));
        IReadOnlyList<LMarkupEntry> entries = LMarkup.LMarkupParse(LMarkupFile.LMarkupFileParse(sample), out _);
        List<LMarkupEntry> copies = new(entries.Count * LDrillMarkupCopies);
        for (int copy = 0; copy < LDrillMarkupCopies; copy++)
        {
            copies.AddRange(entries);
        }

        _lDrillMarkupText = LMarkupFile.LMarkupFileFormat(LMarkup.LMarkupFormat(copies));
    }

    public override void LDrillCycleRun()
    {
        IReadOnlyList<LMarkupEntry> entries = LMarkup.LMarkupParse(
            LMarkupFile.LMarkupFileParse(_lDrillMarkupText), out _);
        LMarkupFile.LMarkupFileFormat(LMarkup.LMarkupFormat(entries));
    }
}
