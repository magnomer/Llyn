using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LYunjing
{
    private readonly LEngine _lEngine;

    private LVista? _lShengmuVista;

    private LVista? _lYunmuVista;

    private LVista? _lXiaoyunVista;

    private bool _lYunjingFinalSide;

    private int _lShengmuCount;

    private int _lYunmuCount;

    private int _lXiaoyunCount;

    public LYunjing(
        LEngine engine, Func<bool> changeSeam, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;
        LYunjingPanel = new LPanel("Yunjing.LoadFailed", changeSeam, shownSeam, leaveSeam, deleteSeam);
    }

    public event Action? LYunjingChanged;

    public event Action<string, string>? LYunjingGlyphChosen;

    public LPanel LYunjingPanel { get; }

    public bool LYunjingAllowed => _lEngine.LEngineBookFind() is not null;

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
        _lEngine.LEngineDiweiRead(LYunjingSideVista?.LVistaChosen)?.LDiweiLanguage
        ?? _lEngine.LEngineBookFind()
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

        return _lEngine.LEngineDiweiFind(vista, LYunjingLanguage, kind);
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

        return _lEngine.LEngineXiaoyunFind(LYunjingLanguage, onset, rime, vista);
    }

    public LDiweiPage LYunjingDiweiRead()
    {
        if (!LYunjingDiweiShown)
        {
            return LDiweiPage.LDiweiPageBlank;
        }

        return _lEngine.LEngineDiweiResolve(LYunjingSideVista?.LVistaChosen, LLocalization.LLocalizationTextFind);
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

    public void LYunjingLadderSet(string? choice)
    {
        LYunjingOrderSet(_lShengmuVista, choice);
    }

    public void LYunjingStairSet(string? choice)
    {
        LYunjingOrderSet(_lYunmuVista, choice);
    }

    private static void LYunjingOrderSet(LVista? vista, string? choice)
    {
        if (choice is null)
        {
            return;
        }

        vista?.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, vista.LVistaOrder));
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

        LYunjingDiweiOpen(_lEngine.LEngineDiweiFind(language, kind, key));
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

        _lEngine.LEngineTallySave(chosen);
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

        return _lEngine.LEnginePortraitPrint(LYunjingPanel.LPanelVista, label, ticket);
    }
}
