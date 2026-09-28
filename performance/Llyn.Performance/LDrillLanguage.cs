using Llyn.Infrastructure;

namespace Llyn.Performance;

internal sealed class LDrillLanguage : LDrill
{
    private IReadOnlyList<string> _lDrillLanguageNames = [];

    public override void LDrillPrepare()
    {
        _lDrillLanguageNames = LLanguageLoader.LLanguageLoaderScan();
    }

    public override void LDrillCycleRun()
    {
        foreach (string language in _lDrillLanguageNames)
        {
            LLanguageLoader.LLanguageLoaderLoad(language);
        }
    }
}
