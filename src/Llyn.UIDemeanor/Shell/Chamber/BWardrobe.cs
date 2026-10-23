using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BWardrobe
{
    private readonly (string?, BTheme) _bWardrobeThemeDefault;
    private readonly Func<string, BTheme?> _bWardrobeThemeSeam;
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> _bWardrobeSets;
    private (string, BTheme)? _bWardrobeThemeChosen;

    internal BWardrobe(
        (string?, BTheme) theme,
        Func<string, BTheme?> themes,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> sets,
        string glyphs)
    {
        ArgumentNullException.ThrowIfNull(themes);
        ArgumentNullException.ThrowIfNull(sets);

        _bWardrobeThemeDefault = theme;
        _bWardrobeThemeSeam = themes;
        _bWardrobeSets = sets;
        BWardrobeGlyphApply(glyphs);
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
        return true;
    }

    internal bool BWardrobeThemeSelect(string name)
    {
        if (_bWardrobeThemeSeam(name) is not BTheme theme)
        {
            return false;
        }

        _bWardrobeThemeChosen = (name, theme);
        return true;
    }

    internal bool BWardrobeGlyphSelect(string name)
    {
        if (!_bWardrobeSets.ContainsKey(name))
        {
            return false;
        }

        BWardrobeGlyphApply(name);
        return true;
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
