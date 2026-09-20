using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LNotation
{
    private LForay? _lNotationForay;

    public bool LNotationHeld => _lNotationForay is not null;

    public string LNotationLanguage => _lNotationForay?.LForayLanguage ?? string.Empty;

    public bool LNotationFlagged => _lNotationForay?.LForayFlagged ?? false;

    public bool LNotationPrimary => _lNotationForay?.LForayPrimary ?? false;

    public long LNotationTarget => _lNotationForay?.LForayTarget ?? 0;

    public string LNotationScheme => _lNotationForay?.LForayScheme ?? string.Empty;

    public bool LNotationSchemed => _lNotationForay?.LForaySchemed ?? false;

    public void LNotationForaySet(LForay? foray)
    {
        LNotationCancel();
        _lNotationForay = foray;
    }

    public void LNotationCancel()
    {
        _lNotationForay?.LForayCancel();
        _lNotationForay = null;
    }
}
