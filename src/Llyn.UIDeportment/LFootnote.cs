using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LFootnote
{
    private readonly LEngine _lEngine;

    private LVista? _lFootnoteParent;

    private LVista? _lFootnoteVista;

    private int _lFootnoteCount;

    public LFootnote(LEngine engine, Func<bool> changeSeam, Func<bool> shownSeam, Func<bool> leaveSeam)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;
        LFootnotePanel = new LPanel("List.LoadFailed", changeSeam, shownSeam, leaveSeam, static () => false);
    }

    public event Action<long>? LFootnoteCreated;

    public LPanel LFootnotePanel { get; }

    public bool LFootnoteEmpty => _lFootnoteCount == 0;

    public string LFootnoteEmptyKey => LFootnoteQueried ? "Source.Unmatched" : "Source.Vacant";

    private bool LFootnoteQueried => _lFootnoteVista?.LVistaQueried ?? false;

    public void LFootnoteVistaRestore(LVista parent, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(vista);

        _lFootnoteParent = parent;
        _lFootnoteVista = vista;
        LFootnotePanel.LPanelVistaRestore(vista);
    }

    public IReadOnlyList<LVistaRow> LFootnoteRowsRead()
    {
        IReadOnlyList<LVistaRow> rows = _lEngine.LEngineEntryFind(_lFootnoteParent, _lFootnoteVista);
        _lFootnoteCount = rows.Count;
        return rows;
    }

    public void LFootnoteQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lFootnoteVista?.LVistaQuerySet(query);
    }

    public void LFootnoteEntryCreate()
    {
        LFootnotePanel.LPanelFreshOpen();
        if (_lFootnoteParent?.LVistaChosen is long reference)
        {
            LFootnoteCreated?.Invoke(reference);
        }
    }

    public Task LFootnotePortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lEngine.LEnginePortraitPrint(_lFootnoteVista, label, ticket);
    }
}
