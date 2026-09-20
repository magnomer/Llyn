using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LYunjing
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lShengmuVista;

    private LVista? _lYunmuVista;

    private LVista? _lXiaoyunVista;

    private bool _lYunjingFinalSide;

    private int _lShengmuCount;

    private int _lYunmuCount;

    private int _lXiaoyunCount;

    public LYunjing(
        LPhonologyPort phonology,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LYunjingEditor = editor;
        LYunjingPanel = new LPanel(
            "Yunjing.LoadFailed", editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LYunjingPanel.LPanelCleared += LYunjingEditorClear;
        LYunjingPanel.LPanelEdited += LYunjingEditorOpen;
    }

    private void LYunjingEditorClear()
    {
        LYunjingEditor.LEditorOpen(null);
    }

    private void LYunjingEditorOpen(long id)
    {
        LYunjingEditor.LEditorOpen(id);
    }

    public event Action? LYunjingChanged;

    public event Action<string, string>? LYunjingGlyphChosen;

    public LEditor LYunjingEditor { get; }

    public LPanel LYunjingPanel { get; }

    public bool LYunjingAllowed => _lPhonologyPort.LEngineBookFind() is not null;

    public bool LYunjingDiweiShown => LYunjingDiweiChosen && !LYunjingPanel.LPanelModeEnabled;

    public bool LYunjingDisplayShown => !LYunjingDiweiShown && !LYunjingPanel.LPanelEditing;

    public bool LYunjingEditorShown => LYunjingPanel.LPanelEditing;

    public string LYunjingDiweiKey => _lYunjingFinalSide ? "Yunjing.Yunmu" : "Yunjing.Shengmu";

    public bool LYunjingShengmuEmpty => _lShengmuCount == 0;

    public bool LYunjingYunmuEmpty => _lYunmuCount == 0;

    public bool LYunjingXiaoyunEmpty => _lXiaoyunCount == 0;

    public string LYunjingShengmuKey => LYunjingPlumbQueried ? "Yunjing.ShengmuUnmatched" : "Yunjing.ShengmuEmpty";

    public string LYunjingYunmuKey => LYunjingFathomQueried ? "Yunjing.YunmuUnmatched" : "Yunjing.YunmuEmpty";

    public string LYunjingXiaoyunKey => LYunjingDiweiChosen ? LYunjingVacantKey : "Yunjing.XiaoyunEmpty";

    private string LYunjingVacantKey =>
        LYunjingBeaconQueried ? "Yunjing.XiaoyunUnmatched" : "Yunjing.XiaoyunVacant";

    private bool LYunjingPlumbQueried => _lShengmuVista?.LVistaQueried ?? false;

    private bool LYunjingFathomQueried => _lYunmuVista?.LVistaQueried ?? false;

    private bool LYunjingBeaconQueried => _lXiaoyunVista?.LVistaQueried ?? false;

    private bool LYunjingDiweiChosen => LYunjingSideVista?.LVistaChosen is not null;

    private LVista? LYunjingSideVista => _lYunjingFinalSide ? _lYunmuVista : _lShengmuVista;

    private string LYunjingLanguage =>
        _lPhonologyPort.LEngineDiweiRead(LYunjingSideVista?.LVistaChosen)?.LDiweiLanguage
        ?? _lPhonologyPort.LEngineBookFind()
        ?? string.Empty;

    public void LYunjingVistaRestore(LVista shengmu, LVista yunmu, LVista xiaoyun)
    {
        ArgumentNullException.ThrowIfNull(shengmu);
        ArgumentNullException.ThrowIfNull(yunmu);
        ArgumentNullException.ThrowIfNull(xiaoyun);

        _lShengmuVista = shengmu;
        _lYunmuVista = yunmu;
        _lXiaoyunVista = xiaoyun;
        LYunjingPanel.LPanelVistaRestore(xiaoyun);
        LYunjingEditor.LEditorVistaRestore(xiaoyun);
    }

    public IReadOnlyList<LDiwei> LYunjingShengmuRead()
    {
        IReadOnlyList<LDiwei> rows = LYunjingDiweiFind(_lShengmuVista, LDiwei.LDiweiInitial);
        _lShengmuCount = rows.Count;
        return rows;
    }

    public IReadOnlyList<LDiwei> LYunjingYunmuRead()
    {
        IReadOnlyList<LDiwei> rows = LYunjingDiweiFind(_lYunmuVista, LDiwei.LDiweiRime);
        _lYunmuCount = rows.Count;
        return rows;
    }

    private IReadOnlyList<LDiwei> LYunjingDiweiFind(LVista? vista, string kind)
    {
        if (vista is null)
        {
            return [];
        }

        if (!LYunjingAllowed)
        {
            return [];
        }

        return _lPhonologyPort.LEngineDiweiFind(vista, LYunjingLanguage, kind);
    }

    public IReadOnlyList<LVistaRow> LYunjingXiaoyunRead()
    {
        IReadOnlyList<LVistaRow> rows = LYunjingXiaoyunFind();
        _lXiaoyunCount = rows.Count;
        return rows;
    }

    private IReadOnlyList<LVistaRow> LYunjingXiaoyunFind()
    {
        if (_lShengmuVista is not LVista onset)
        {
            return [];
        }

        if (_lYunmuVista is not LVista rime)
        {
            return [];
        }

        if (_lXiaoyunVista is not LVista vista)
        {
            return [];
        }

        return _lPhonologyPort.LEngineXiaoyunFind(LYunjingLanguage, onset, rime, vista);
    }

    public LDiweiPage LYunjingDiweiRead()
    {
        if (!LYunjingDiweiShown)
        {
            return LDiweiPage.LDiweiPageBlank;
        }

        return _lPhonologyPort.LEngineDiweiResolve(
            LYunjingSideVista?.LVistaChosen, _lSettingsPort.LEngineTextFind);
    }

    public void LYunjingPlumbSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lShengmuVista?.LVistaQuerySet(query);
    }

    public void LYunjingFathomSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lYunmuVista?.LVistaQuerySet(query);
    }

    public void LYunjingBeaconSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lXiaoyunVista?.LVistaQuerySet(query);
    }

    public void LYunjingLadderSet(LCatalogOrder? order)
    {
        LYunjingOrderSet(_lShengmuVista, order);
    }

    public void LYunjingStairSet(LCatalogOrder? order)
    {
        LYunjingOrderSet(_lYunmuVista, order);
    }

    private static void LYunjingOrderSet(LVista? vista, LCatalogOrder? order)
    {
        if (vista is null)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LYunjingReset()
    {
        _lShengmuVista?.LVistaSelect(null);
        _lYunmuVista?.LVistaSelect(null);
        LYunjingPanel.LPanelReset();
        LYunjingChanged?.Invoke();
    }

    public void LYunjingRowsUpdate()
    {
        LYunjingChanged?.Invoke();
        LYunjingPanel.LPanelRowsUpdate();
    }

    public void LYunjingEntryHandle(LBulletin bulletin)
    {
        LYunjingChanged?.Invoke();
        LYunjingPanel.LPanelEntryHandle(bulletin);
    }

    public void LYunjingDiweiSelect(long? id, bool? final)
    {
        if (id is not long cell)
        {
            return;
        }

        if (final is not bool rime)
        {
            return;
        }

        _lYunjingFinalSide = rime;
        LYunjingSideVista?.LVistaToggle(cell);
        LYunjingPanel.LPanelClear();
        LYunjingChanged?.Invoke();
    }

    public void LYunjingDiweiShow(string language, string kind, string key)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(key);

        LYunjingDiweiOpen(_lPhonologyPort.LEngineDiweiFind(language, kind, key));
    }

    private void LYunjingDiweiOpen(LDiwei? found)
    {
        if (found is null)
        {
            return;
        }

        _lShengmuVista?.LVistaSelect(null);
        _lYunmuVista?.LVistaSelect(null);
        LYunjingDiweiSelect(found.LDiweiId, found.LDiweiFinal);
    }

    public void LYunjingTallySet(bool? respelled)
    {
        if (respelled is not bool chosen)
        {
            return;
        }

        _lPhonologyPort.LEngineTallySave(chosen);
        LYunjingChanged?.Invoke();
    }

    public void LYunjingGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!LYunjingDiweiShown)
        {
            return;
        }

        if (!LYunjingPanel.LPanelLeaveConfirm())
        {
            return;
        }

        LYunjingGlyphChosen?.Invoke(character, LYunjingLanguage);
    }

    public Task LYunjingPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        if (!LYunjingPanel.LPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(LYunjingPanel.LPanelVista, label, ticket);
    }

    public void LYunjingVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LYunjingVistaRestore(
            window.LWindowVistaStart("yunjing", null, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("yunmu", null, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("xiaoyun", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public void LYunjingShengmuAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lShengmuVista?.LVistaObserverAttach(subject, observer);
    }

    public void LYunjingYunmuAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lYunmuVista?.LVistaObserverAttach(subject, observer);
    }

    public LCatalogOrder LYunjingLadder => _lShengmuVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderName;

    public LCatalogOrder LYunjingStair => _lYunmuVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderName;

    public string LYunjingFileRead()
    {
        return LVista.LVistaFileRead(LYunjingPanel.LPanelVista);
    }

    public Task LYunjingPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(LYunjingPanel.LPanelVista, path, format, label);
    }
}
