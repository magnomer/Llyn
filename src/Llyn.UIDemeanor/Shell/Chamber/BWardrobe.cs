using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Llyn.Conduct;
using Llyn.UIDemeanor.Capsule;

namespace Llyn.UIDemeanor;

internal sealed class BWardrobe
{
    private readonly (string?, BTheme) _bWardrobeThemeDefault;
    private readonly Func<string, BTheme?> _bWardrobeThemeSeam;
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> _bWardrobeSets;
    private readonly LCapsuleConsole _bWardrobeCapsule;
    private readonly CAtelier? _bWardrobeAtelier;
    private (string, BTheme)? _bWardrobeThemeChosen;

    internal BWardrobe(
        (string?, BTheme) theme,
        Func<string, BTheme?> themes,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> sets,
        string glyphs,
        LCapsuleConsole capsule,
        CAtelier? atelier)
    {
        ArgumentNullException.ThrowIfNull(themes);
        ArgumentNullException.ThrowIfNull(sets);
        ArgumentNullException.ThrowIfNull(capsule);

        _bWardrobeThemeDefault = theme;
        _bWardrobeThemeSeam = themes;
        _bWardrobeSets = sets;
        _bWardrobeCapsule = capsule;
        _bWardrobeAtelier = atelier;
        BWardrobeGlyphApply(glyphs);

        LCapsuleSetting stored = capsule.LCapsuleConsoleRead(atelier?.CAtelierPathRead());
        if (stored.LCapsuleSettingTheme is string chosen)
        {
            _ = BWardrobeThemeSelect(chosen);
        }

        if (stored.LCapsuleSettingGlyph is string glyph)
        {
            _ = BWardrobeGlyphSelect(glyph);
        }

        if (stored.LCapsuleSettingAmbiguous is string ambiguous)
        {
            _ = BWardrobeAmbiguousSelect(ambiguous);
        }
    }

    internal (string?, BTheme) BWardrobeTheme => _bWardrobeThemeChosen ?? _bWardrobeThemeDefault;

    internal string BWardrobeGlyphName { get; private set; }

    internal IReadOnlyDictionary<string, IReadOnlyList<string>> BWardrobeMarks { get; private set; }

    internal IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> BWardrobeGlyphSets
    {
        get;
        private set;
    }

    internal string BWardrobeAmbiguousName { get; private set; } = "narrow";

    internal BCanvasWidth BWardrobeWidth { get; private set; } = new(false);

    internal bool BWardrobeAmbiguousSelect(string name)
    {
        if (name is not ("narrow" or "wide"))
        {
            return false;
        }

        BWardrobeAmbiguousName = name;
        BWardrobeWidth = new BCanvasWidth(name == "wide");
        BWardrobeSave(stored => stored with { LCapsuleSettingAmbiguous = name });
        return true;
    }

    internal bool BWardrobeThemeSelect(string name)
    {
        if (_bWardrobeThemeSeam(name) is not BTheme theme)
        {
            return false;
        }

        _bWardrobeThemeChosen = (name, theme);
        BWardrobeSave(stored => stored with { LCapsuleSettingTheme = name });
        return true;
    }

    internal bool BWardrobeGlyphSelect(string name)
    {
        if (!_bWardrobeSets.ContainsKey(name))
        {
            return false;
        }

        BWardrobeGlyphApply(name);
        BWardrobeSave(stored => stored with { LCapsuleSettingGlyph = name });
        return true;
    }

    private void BWardrobeSave(Func<LCapsuleSetting, LCapsuleSetting> choice)
    {
        string? root = _bWardrobeAtelier?.CAtelierPathRead();
        LCapsuleSetting stored = _bWardrobeCapsule.LCapsuleConsoleRead(root);
        LCapsuleSetting chosen = choice(stored);
        if (chosen == stored)
        {
            return;
        }

        try
        {
            _bWardrobeCapsule.LCapsuleConsoleSave(root, chosen);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    [MemberNotNull(nameof(BWardrobeGlyphName), nameof(BWardrobeMarks), nameof(BWardrobeGlyphSets))]
    private void BWardrobeGlyphApply(string name)
    {
        BWardrobeGlyphName = name;
        BWardrobeMarks = _bWardrobeSets.GetValueOrDefault(name) ?? new Dictionary<string, IReadOnlyList<string>>();
        BWardrobeGlyphSets = _bWardrobeSets.ToDictionary(
            static set => set.Key, set => _bWardrobeSets.GetValueOrDefault(name, set.Value));
    }
}
