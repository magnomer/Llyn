using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BInput
{
    private readonly BEditor _bInputEditor;

    private readonly Func<bool> _bInputShown;

    internal BInput(BLocalizationCatalog catalog, CAtelier atelier, CEnvoy envoy, Func<bool> shown, Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(changed);

        _bInputShown = shown;
        _bInputEditor = new BEditor(
            catalog, atelier.CAtelierInputCreate(envoy), "Navigation.Input", shown, shown, changed);
        BInputKeymap = _bInputEditor.BEditorKeymap;
    }

    internal BKeymap BInputKeymap { get; }

    internal bool BInputWhole => _bInputShown();

    internal BKeymapFocus BInputKeymapFocus =>
        _bInputShown() ? BKeymapFocus.BKeymapFocusEditor : BKeymapFocus.BKeymapFocusWindow;

    internal void BInputObserve(ConsoleKeyInfo key)
    {
        _ = _bInputEditor.BEditorObserve(key);
    }

    internal IEnumerable<BCanvasText> BInputDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BBlueprintPart window,
        bool focused,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(window);

        return _bInputShown()
            ? _bInputEditor.BEditorDraw(glyphs, BContract.BContractFind(window, "ADisplay"), focused, measure)
            : [];
    }
}
