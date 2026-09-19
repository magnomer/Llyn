using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LPhonology
{
    private readonly LEngine _lEngine;

    private LVista? _lPhonologyVista;

    private int _lPhonologyCount;

    public LPhonology(
        LEngine engine, Func<bool> changeSeam, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;
        LPhonologyPanel = new LPanel("Sound.LoadFailed", changeSeam, shownSeam, leaveSeam, deleteSeam);
    }

    public LPanel LPhonologyPanel { get; }

    public bool LPhonologyInventoryEmpty => _lPhonologyCount == 0;

    public bool LPhonologyFilterActive => _lPhonologyVista?.LVistaFiltered ?? false;

    public void LPhonologyVistaRestore(LVista vista)
    {
        _lPhonologyVista = vista;
        LPhonologyPanel.LPanelVistaRestore(vista);
    }

    public IReadOnlyList<LCatalogPronunciation> LPhonologyRowsRead()
    {
        IReadOnlyList<LCatalogPronunciation> rows =
            _lPhonologyVista is LVista vista ? _lEngine.LEnginePronunciationFind(vista) : [];
        _lPhonologyCount = rows.Count;
        return rows;
    }

    public void LPhonologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lPhonologyVista?.LVistaQuerySet(query);
    }

    public void LPhonologyOrderSet(string? choice)
    {
        if (choice is null)
        {
            return;
        }

        if (_lPhonologyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, vista.LVistaOrder));
    }

    public void LPhonologyFilterSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lPhonologyVista?.LVistaFilterSet(filter);
    }

    public Task LPhonologyPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        if (_lPhonologyVista is not LVista vista)
        {
            return Task.CompletedTask;
        }

        return _lEngine.LEnginePortraitPrint(vista, label, ticket);
    }
}
