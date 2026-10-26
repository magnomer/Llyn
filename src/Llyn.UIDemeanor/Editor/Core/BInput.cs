using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BInput
{
    private readonly BEditor _bInputEditor;

    private readonly Func<bool> _bInputShown;

    private readonly CAtelier _bInputAtelier;

    private readonly CEnvoy _bInputEnvoy;

    internal BInput(BLocalizationCatalog catalog, CAtelier atelier, CEnvoy envoy, Func<bool> shown, Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(changed);

        _bInputShown = shown;
        _bInputAtelier = atelier;
        _bInputEnvoy = envoy;
        _bInputEditor = new BEditor(
            catalog,
            atelier.CAtelierInputCreate(envoy),
            "Navigation.Input",
            shown,
            shown,
            changed,
            languages: BInputLanguageLoad);
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

    private Task<IReadOnlyList<string>> BInputLanguageLoad()
    {
        return _bInputAtelier.CAtelierCatalog.CCatalogEnsignLoad(_bInputEnvoy, static (_, _) => static () => { });
    }
}
