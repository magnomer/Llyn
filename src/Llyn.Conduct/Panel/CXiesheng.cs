using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CXiesheng
{
    private readonly CAtelier _cXieshengAtelier;

    private readonly LStemPort _cXieshengPort;

    private readonly LSettingsPort _cXieshengSettingsPort;

    private readonly CEnvoy _cXieshengEnvoy;

    private readonly Action<Action> _cXieshengMarshal;

    private CXiesheng(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cXieshengAtelier = atelier;
        _cXieshengPort = atelier.CAtelierPhonologyBundle.CPhonologyBundleStem;
        _cXieshengSettingsPort = atelier.CAtelierSettingsPort;
        _cXieshengEnvoy = envoy;
        _cXieshengMarshal = marshal;
        CXieshengGrove = new CAperture(
            envoy,
            _cXieshengSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            "Xiesheng.LoadFailed",
            "Xiesheng.GroveEmpty",
            "Xiesheng.GroveUnmatched");
        CXieshengKindred = new CEntryList(
            atelier, shownSeam, envoy, marshal, "Xiesheng", "Kindred", () => LXieshengAllowed, LXieshengKindredFind);
        CXieshengKindred.CEntryListChanged += () => CXieshengChanged?.Invoke();
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LXieshengVistaRestore);
        atelier.CAtelierNavigation.LNavigationStemAttach(LXieshengStemOpen);
        LXieshengVistaRestore();
    }

    public static CXiesheng CXieshengCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CXiesheng(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CXieshengChanged;

    public event Action? CXieshengWorkspaceChanged;

    public CAperture CXieshengGrove { get; }

    public CEntryList CXieshengKindred { get; }

    public event Action? CXieshengStemOpened;

    internal bool LXieshengAllowed => _cXieshengPort.LEngineStemCheck();

    public bool CXieshengStemShown =>
        LXieshengStemChosen && !CXieshengKindred.CEntryListPanel.CPanelModeEnabled;

    public bool CXieshengDisplayShown => !CXieshengStemShown && !CXieshengKindred.CEntryListEditing;

    public string CXieshengKindredKey => LXieshengStemChosen
        ? CXieshengKindred.CEntryListPanel.CPanelAperture.CApertureKey
        : "Xiesheng.KindredEmpty";

    private bool LXieshengStemChosen => CXieshengGrove.CApertureChosen is not null;

    internal void LXieshengVistaRestore()
    {
        CXieshengGrove.CApertureRestore(
            _cXieshengAtelier.CAtelierVistaStart("grove", null, CCatalogOrder.CCatalogOrderName));
        CXieshengKindred.LEntryListRestore();
        LXieshengObserverAttach();
    }

    private void LXieshengObserverAttach()
    {
        Action<CBulletin> rows = _ => _cXieshengMarshal(CXieshengKindred.LEntryListResonate);
        CXieshengGrove.CApertureObserverAttach(CSubject.CSubjectVista, rows);
        CXieshengGrove.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cXieshengMarshal(LXieshengWorkspaceResonate));
        CXieshengGrove.CApertureObserverAttach(CSubject.CSubjectFanqie, rows);
        CXieshengGrove.CApertureObserverAttach(CSubject.CSubjectSettings, rows);
        CXieshengGrove.CApertureObserverAttach(
            CSubject.CSubjectStemFold, _ => _cXieshengMarshal(() => CXieshengChanged?.Invoke()));
        CXieshengKindred.LEntryListAttach();
    }

    public static IReadOnlyList<CCatalogOrder> CXieshengOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public IReadOnlyList<CStem> CXieshengGroveRead()
    {
        IReadOnlyList<CStem> rows = CXieshengGrove.CApertureVista is LVista grove
            ? _cXieshengPort.LEngineStemFind(grove)
                .Select(static row => new CStem(row.LStemId, row.LStemKey, row.LStemCount, row.LStemChosen))
                .ToList()
            : [];
        CXieshengGrove.CApertureCountSet(rows.Count);
        return rows;
    }

    private IReadOnlyList<LVistaRow> LXieshengKindredFind(LVista kindred)
    {
        return CXieshengGrove.CApertureVista is LVista grove ? _cXieshengPort.LEngineKindredFind(grove, kindred) : [];
    }

    public CStemPage CXieshengStemRead()
    {
        LStemPage page = _cXieshengPort.LEngineStemResolve(CXieshengStemShown ? CXieshengGrove.CApertureChosen : null);
        return new CStemPage(
            page.LStemPageLanguage,
            page.LStemPageKey,
            CStemMember.LStemMemberRead(page.LStemPageMembers),
            page.LStemPageEmpty,
            CFont.CFontRead(_cXieshengSettingsPort, page.LStemPageLanguage, CFontRole.CFontRoleHeadword),
            CFont.CFontRead(_cXieshengSettingsPort, page.LStemPageLanguage, CFontRole.CFontRoleGlyph));
    }

    public bool CXieshengFoldToggle(string character, bool opened)
    {
        if (string.IsNullOrEmpty(character) || !CXieshengStemShown)
        {
            return false;
        }

        try
        {
            _cXieshengPort.LEngineStemSpread(CXieshengGrove.CApertureChosen, character, opened);
            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cXieshengEnvoy, _cXieshengSettingsPort, "Reflex.SpreadFailed", exception);
            return false;
        }
    }

    private void LXieshengWorkspaceResonate()
    {
        CXieshengGrove.CApertureVista?.LVistaSelect(null);
        CXieshengKindred.CEntryListPanel.CPanelEntryClose();
        CXieshengChanged?.Invoke();
        CXieshengWorkspaceChanged?.Invoke();
    }

    public void CXieshengStemSelect(long? id)
    {
        if (id is not long stem)
        {
            return;
        }

        if (!CXieshengKindred.CEntryListPanel.CPanelLeaveConfirm())
        {
            return;
        }

        LXieshengStemToggle(stem);
    }

    private void LXieshengStemToggle(long stem)
    {
        CXieshengGrove.CApertureVista?.LVistaToggle(stem);
        CXieshengKindred.CEntryListPanel.CPanelEntryClose();
        CXieshengChanged?.Invoke();
    }

    internal void LXieshengStemOpen(string language, string? key)
    {
        ArgumentNullException.ThrowIfNull(language);

        CXieshengGrove.CApertureQuerySet(string.Empty);
        CXieshengStemOpened?.Invoke();

        if (_cXieshengPort.LEngineStemFind(language, key) is not long stem)
        {
            return;
        }

        CXieshengGrove.CApertureVista?.LVistaSelect(null);
        LXieshengStemToggle(stem);
    }

    public void CXieshengGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!CXieshengStemShown)
        {
            return;
        }

        if (CCatalog.LCatalogGlyphOpen(
                _cXieshengEnvoy,
                _cXieshengSettingsPort,
                () => _cXieshengPort.LEngineStemResolve(CXieshengGrove.CApertureChosen, character))
            is long entry)
        {
            _cXieshengAtelier.CAtelierNavigation.CNavigationEntryOpen(entry);
        }
    }
}
