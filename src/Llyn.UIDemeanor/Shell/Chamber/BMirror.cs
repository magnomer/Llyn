using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BMirror
{
    private readonly BLocalizationCatalog _bMirrorCatalog;
    private readonly CAtelier? _bMirrorAtelier;
    private readonly CEnvoy _bMirrorEnvoy;
    private readonly BWardrobe _bMirrorWardrobe;
    private readonly BNavigation _bMirrorTabs;
    private readonly Func<Func<CCourierState?>, BRoofMenu> _bMirrorHeadquarter;
    private readonly Action _bMirrorPicker;
    private readonly Action _bMirrorRestyled;
    private readonly Action _bMirrorChanged;
    private CLedgerState? _bMirrorLedger;

    internal BMirror(
        BLocalizationCatalog catalog,
        CAtelier? atelier,
        CEnvoy envoy,
        BWardrobe wardrobe,
        BBlueprintPart blueprint,
        BNavigation tabs,
        Func<Func<CCourierState?>, BRoofMenu> headquarter,
        Action picker,
        Action restyled,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(wardrobe);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(tabs);
        ArgumentNullException.ThrowIfNull(headquarter);
        ArgumentNullException.ThrowIfNull(picker);
        ArgumentNullException.ThrowIfNull(restyled);
        ArgumentNullException.ThrowIfNull(changed);

        _bMirrorCatalog = catalog;
        _bMirrorAtelier = atelier;
        _bMirrorEnvoy = envoy;
        _bMirrorWardrobe = wardrobe;
        _bMirrorTabs = tabs;
        _bMirrorHeadquarter = headquarter;
        _bMirrorPicker = picker;
        _bMirrorRestyled = restyled;
        _bMirrorChanged = changed;
        BMirrorRoof = new BRoof(blueprint, BMirrorMenuBuild());
    }

    internal BRoof BMirrorRoof { get; }

    internal CNavigationState? BMirrorNavigation { get; private set; }

    internal CCourierState? BMirrorCourier { get; private set; }

    internal void BMirrorOpen(CAtelier atelier, CCourier? courier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        atelier.CAtelierNavigation.CNavigationChanged +=
            BObserver.BObserverCreate<CNavigationState>(BMirrorNavigationObserve);
        atelier.CAtelierLedger.CLedgerChanged += BObserver.BObserverCreate<CLedgerState>(BMirrorLedgerObserve);
        atelier.CAtelierWorkspace.CWorkspaceOpened += BMirrorWorkspaceObserve;
        if (courier is not null)
        {
            courier.CCourierChanged += BObserver.BObserverCreate<CCourierState>(BMirrorCourierObserve);
            BMirrorCourierObserve(courier.CCourierRead());
        }
    }

    private void BMirrorWorkspaceObserve()
    {
        _bMirrorWardrobe.BWardrobeRootRefine(_bMirrorAtelier?.CAtelierPathRead());
        BMirrorRoof.BRoofApply(BMirrorMenuBuild());
        _bMirrorRestyled();
    }

    private void BMirrorNavigationObserve(CNavigationState state)
    {
        BMirrorNavigation = state;
        BMirrorRoof.BRoofApply(BMirrorMenuBuild());
        _bMirrorChanged();
    }

    private void BMirrorCourierObserve(CCourierState state)
    {
        BMirrorCourier = state;
        BMirrorRoof.BRoofApply(BMirrorMenuBuild());
        _bMirrorChanged();
    }

    private void BMirrorLedgerObserve(CLedgerState state)
    {
        _bMirrorCatalog.BLocalizationCatalogApply(state.CLedgerStateTexts);
        _bMirrorLedger = state;
        BMirrorRoof.BRoofApply(BMirrorMenuBuild());
        _bMirrorChanged();
    }

    private IReadOnlyList<BRoofMenu> BMirrorMenuBuild()
    {
        BRoofMenu panel = _bMirrorTabs.BNavigationBuild(
            _bMirrorCatalog,
            BMirrorNavigation,
            _bMirrorWardrobe.BWardrobeMarks,
            _bMirrorPicker,
            _bMirrorWardrobe.BWardrobeWidth);
        (string? current, _) = _bMirrorWardrobe.BWardrobeTheme;
        Action<bool> redraw = chosen =>
        {
            if (chosen)
            {
                BMirrorRoof.BRoofApply(BMirrorMenuBuild());
                _bMirrorRestyled();
            }
        };
        BRoofMenu options = BCabinet.BCabinetBuild(
            _bMirrorCatalog,
            _bMirrorWardrobe.BWardrobeMarks,
            current,
            _bMirrorWardrobe.BWardrobeGlyphName,
            _bMirrorWardrobe.BWardrobeAmbiguousName,
            _bMirrorLedger,
            name => redraw(_bMirrorWardrobe.BWardrobeThemeSelect(name)),
            name => redraw(_bMirrorWardrobe.BWardrobeGlyphSelect(name)),
            name => redraw(_bMirrorWardrobe.BWardrobeAmbiguousSelect(name)),
            _bMirrorEnvoy,
            _bMirrorAtelier is CAtelier atelier ? atelier.CAtelierLedger.CLedgerLocalizationSave
                : static (_, _) => { },
            _bMirrorAtelier is CAtelier shown ? shown.CAtelierNavigation.CNavigationTabSelect
                : static _ => false,
            _bMirrorWardrobe.BWardrobeWidth);
        return [_bMirrorHeadquarter(() => BMirrorCourier), panel, options];
    }
}
