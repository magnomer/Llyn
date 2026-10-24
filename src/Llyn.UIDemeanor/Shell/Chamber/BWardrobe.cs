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
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> BWardrobeMarksNone =
        new Dictionary<string, IReadOnlyList<string>>();

    private const string BWardrobeAmbiguousDefault = "narrow";

    private readonly (string?, BTheme) _bWardrobeThemeDefault;
    private readonly string _bWardrobeGlyphDefault;
    private readonly Func<string, BTheme?> _bWardrobeThemeSeam;
    private readonly Action<string> _bWardrobeThemeReport;
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> _bWardrobeSets;
    private readonly LCapsuleConsole _bWardrobeCapsule;
    private string? _bWardrobeRoot;
    private (string, BTheme)? _bWardrobeThemeChosen;

    internal BWardrobe(
        (string?, BTheme) theme,
        Func<string, BTheme?> themes,
        Action<string> report,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> sets,
        string glyphs,
        LCapsuleConsole capsule,
        CAtelier? atelier)
    {
        ArgumentNullException.ThrowIfNull(themes);
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(sets);
        ArgumentNullException.ThrowIfNull(capsule);

        _bWardrobeThemeDefault = theme;
        _bWardrobeThemeSeam = themes;
        _bWardrobeThemeReport = report;
        _bWardrobeSets = sets;
        _bWardrobeCapsule = capsule;
        _bWardrobeGlyphDefault = glyphs;
        BWardrobeGlyphApply(glyphs);
        BWardrobeRootRefine(atelier?.CAtelierPathRead());
    }

    internal (string?, BTheme) BWardrobeTheme => _bWardrobeThemeChosen ?? _bWardrobeThemeDefault;

    internal string BWardrobeGlyphName { get; private set; }

    internal IReadOnlyDictionary<string, IReadOnlyList<string>> BWardrobeMarks =>
        _bWardrobeSets.GetValueOrDefault(BWardrobeGlyphName) ?? BWardrobeMarksNone;

    internal IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> BWardrobeGlyphSets
    {
        get;
        private set;
    }

    internal string BWardrobeAmbiguousName { get; private set; } = BWardrobeAmbiguousDefault;

    internal BCanvasWidth BWardrobeWidth { get; private set; } = new(false);

    internal bool BWardrobeAmbiguousSelect(string name)
    {
        if (name is not ("narrow" or "wide"))
        {
            return false;
        }

        BWardrobeAmbiguousApply(name);
        BWardrobeSave(stored => stored with { LCapsuleSettingAmbiguous = name });
        return true;
    }

    private void BWardrobeAmbiguousApply(string name)
    {
        BWardrobeAmbiguousName = name;
        BWardrobeWidth = new BCanvasWidth(name == "wide");
    }

    internal bool BWardrobeThemeSelect(string name)
    {
        return BWardrobeThemeApply(name, _bWardrobeThemeReport);
    }

    private bool BWardrobeThemeApply(string name, Action<string> report)
    {
        BTheme? loaded;
        try
        {
            loaded = _bWardrobeThemeSeam(name);
        }
        catch (Exception exception)
        {
            report(exception.Message);
            return false;
        }

        if (loaded is not BTheme theme)
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

    internal void BWardrobeRootRefine(string? root)
    {
        if (string.Equals(root, _bWardrobeRoot, StringComparison.Ordinal))
        {
            return;
        }

        _bWardrobeRoot = root;
        LCapsuleSetting stored = _bWardrobeCapsule.LCapsuleConsoleRead(root);
        if (stored.LCapsuleSettingTheme is not string theme
            || (theme != BWardrobeTheme.Item1 && !BWardrobeThemeApply(theme, static _ => { })))
        {
            _bWardrobeThemeChosen = null;
        }

        if (stored.LCapsuleSettingGlyph is not string glyph || !BWardrobeGlyphSelect(glyph))
        {
            BWardrobeGlyphApply(_bWardrobeGlyphDefault);
        }

        if (stored.LCapsuleSettingAmbiguous is not string ambiguous || !BWardrobeAmbiguousSelect(ambiguous))
        {
            BWardrobeAmbiguousApply(BWardrobeAmbiguousDefault);
        }
    }

    private void BWardrobeSave(Func<LCapsuleSetting, LCapsuleSetting> choice)
    {
        string? root = _bWardrobeRoot;
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

    [MemberNotNull(nameof(BWardrobeGlyphName), nameof(BWardrobeGlyphSets))]
    private void BWardrobeGlyphApply(string name)
    {
        BWardrobeGlyphName = name;
        BWardrobeGlyphSets = _bWardrobeSets.ToDictionary(
            static set => set.Key, set => _bWardrobeSets.GetValueOrDefault(name, set.Value));
    }
}
