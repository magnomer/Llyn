using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LXiesheng
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lGroveVista;

    private LVista? _lKindredVista;

    private int _lGroveCount;

    private int _lKindredCount;

    public LXiesheng(
        LPhonologyPort phonology,
        LPortraitPort portraits,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        LXieshengEditor = editor;
        LXieshengPanel = new LPanel(
            "Xiesheng.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LXieshengPanel.LPanelCleared += LXieshengEditorClear;
        LXieshengPanel.LPanelEdited += LXieshengEditorOpen;
    }

    private void LXieshengEditorClear()
    {
        LXieshengEditor.LEditorOpen(null);
    }

    private void LXieshengEditorOpen(long id)
    {
        LXieshengEditor.LEditorOpen(id);
    }

    public event Action? LXieshengChanged;

    public event Action<string, string>? LXieshengGlyphChosen;

    public LEditor LXieshengEditor { get; }

    public LPanel LXieshengPanel { get; }

    public bool LXieshengAllowed => _lPhonologyPort.LEngineStemFind() is not null;

    public bool LXieshengStemShown => LXieshengStemChosen && !LXieshengPanel.LPanelModeEnabled;

    public bool LXieshengDisplayShown => !LXieshengStemShown && !LXieshengPanel.LPanelEditing;

    public bool LXieshengEditorShown => LXieshengPanel.LPanelEditing;

    public bool LXieshengGroveEmpty => _lGroveCount == 0;

    public bool LXieshengKindredEmpty => _lKindredCount == 0;

    public string LXieshengGroveKey => LXieshengLodestarQueried ? "Xiesheng.GroveUnmatched" : "Xiesheng.GroveEmpty";

    public string LXieshengKindredKey => LXieshengStemChosen ? LXieshengVacantKey : "Xiesheng.KindredEmpty";

    public LCatalogOrder LXieshengRung => _lGroveVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderName;

    private string LXieshengVacantKey =>
        LXieshengSextantQueried ? "Xiesheng.KindredUnmatched" : "Xiesheng.KindredVacant";

    private bool LXieshengLodestarQueried => _lGroveVista?.LVistaQueried ?? false;

    private bool LXieshengSextantQueried => _lKindredVista?.LVistaQueried ?? false;

    private bool LXieshengStemChosen => _lGroveVista?.LVistaChosen is not null;

    private string LXieshengLanguage =>
        _lPhonologyPort.LEngineStemRead(_lGroveVista?.LVistaChosen)?.LStemLanguage
        ?? _lPhonologyPort.LEngineStemFind()
        ?? string.Empty;

    public void LXieshengVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LXieshengVistaRestore(
            window.LWindowVistaStart("grove", null, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("kindred", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public void LXieshengVistaRestore(LVista grove, LVista kindred)
    {
        ArgumentNullException.ThrowIfNull(grove);
        ArgumentNullException.ThrowIfNull(kindred);

        _lGroveVista = grove;
        _lKindredVista = kindred;
        LXieshengPanel.LPanelVistaRestore(kindred);
        LXieshengEditor.LEditorVistaRestore(kindred);
    }

    public IReadOnlyList<LStem> LXieshengGroveRead()
    {
        IReadOnlyList<LStem> rows = LXieshengStemFind();
        _lGroveCount = rows.Count;
        return rows;
    }

    private IReadOnlyList<LStem> LXieshengStemFind()
    {
        if (_lGroveVista is not LVista vista)
        {
            return [];
        }

        return LXieshengAllowed ? _lPhonologyPort.LEngineStemFind(vista, LXieshengLanguage) : [];
    }

    public IReadOnlyList<LVistaRow> LXieshengKindredRead()
    {
        IReadOnlyList<LVistaRow> rows = LXieshengKindredFind();
        _lKindredCount = rows.Count;
        return rows;
    }

    private IReadOnlyList<LVistaRow> LXieshengKindredFind()
    {
        if (_lGroveVista is not LVista grove)
        {
            return [];
        }

        if (_lKindredVista is not LVista vista)
        {
            return [];
        }

        return _lPhonologyPort.LEngineKindredFind(LXieshengLanguage, grove, vista);
    }

    public LStemPage LXieshengStemRead()
    {
        return LXieshengStemShown
            ? _lPhonologyPort.LEngineStemResolve(_lGroveVista?.LVistaChosen)
            : LStemPage.LStemPageBlank;
    }

    public void LXieshengLodestarSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lGroveVista?.LVistaQuerySet(query);
    }

    public void LXieshengSextantSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lKindredVista?.LVistaQuerySet(query);
    }

    public void LXieshengRungSet(LCatalogOrder? order)
    {
        if (_lGroveVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LXieshengReset()
    {
        _lGroveVista?.LVistaSelect(null);
        LXieshengPanel.LPanelClear();
        LXieshengChanged?.Invoke();
    }

    public void LXieshengRowsUpdate()
    {
        LXieshengChanged?.Invoke();
        LXieshengPanel.LPanelRowsUpdate();
    }

    public void LXieshengEntryHandle(LBulletin bulletin)
    {
        LXieshengChanged?.Invoke();
        LXieshengPanel.LPanelEntryHandle(bulletin);
    }

    public void LXieshengStemSelect(long? id)
    {
        if (id is not long stem)
        {
            return;
        }

        _lGroveVista?.LVistaToggle(stem);
        LXieshengPanel.LPanelClear();
        LXieshengChanged?.Invoke();
    }

    public void LXieshengStemShow(string language, string? key)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (_lPhonologyPort.LEngineStemFind(language, key) is not LStem found)
        {
            return;
        }

        _lGroveVista?.LVistaSelect(null);
        LXieshengStemSelect(found.LStemId);
    }

    public void LXieshengGlyphSelect(string? character)
    {
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        if (!LXieshengStemShown)
        {
            return;
        }

        if (!LXieshengPanel.LPanelLeaveConfirm())
        {
            return;
        }

        LXieshengGlyphChosen?.Invoke(character, LXieshengLanguage);
    }

    public void LXieshengGroveAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lGroveVista?.LVistaObserverAttach(subject, observer);
    }

    public Task LXieshengPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        if (!LXieshengPanel.LPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(LXieshengPanel.LPanelVista, label, ticket);
    }

    public Task LXieshengPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(LXieshengPanel.LPanelVista, path, format, label);
    }

    public string LXieshengFileRead()
    {
        return LVista.LVistaFileRead(LXieshengPanel.LPanelVista);
    }
}
