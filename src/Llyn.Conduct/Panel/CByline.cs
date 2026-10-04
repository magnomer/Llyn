using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CByline
{
    private readonly CImprint _cBylineImprint;

    private readonly LDraftPort _cBylineDraftPort;

    private readonly CLedgerNoticed _cBylineNoticed;

    private int _cBylineIndex = -1;

    private int _cBylineCount;

    private string _cBylineWord = string.Empty;

    internal CByline(CImprint imprint, LDraftPort drafts, CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(imprint);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(noticed);

        _cBylineImprint = imprint;
        _cBylineDraftPort = drafts;
        _cBylineNoticed = noticed;
    }

    public event Action? CBylineChanged;

    public bool CBylineShown => _cBylineCount > 0;

    public int CBylineIndex => _cBylineIndex;

    public void CBylineWordSet(string? text, bool? focused)
    {
        if (!_cBylineImprint.CImprintHeld)
        {
            return;
        }

        if (focused != true)
        {
            return;
        }

        _cBylineIndex = -1;
        _cBylineCount = 0;
        _cBylineWord = _cBylineDraftPort.LEngineBylineRead(text);
        CBylineChanged?.Invoke();
    }

    public IReadOnlyList<CAuthor> CBylineRowsRead()
    {
        IReadOnlyList<CAuthor> rows = LBylineFind();
        _cBylineCount = rows.Count;
        return rows;
    }

    public bool CBylineMove(int? position, long? id, int step)
    {
        if (!_cBylineImprint.CImprintHeld)
        {
            return false;
        }

        if (position is null || id is null || !CBylineShown)
        {
            return false;
        }

        _cBylineIndex = CLantern.CLanternMove(_cBylineIndex, _cBylineCount, step) ?? -1;
        CBylineChanged?.Invoke();
        return true;
    }

    public void CBylineSelect(long? id, int? position, long? held)
    {
        if (id is not long picked || position is not int at || held is not long author)
        {
            CBylineClose();
            return;
        }

        LBylineCommit(at, author, picked);
    }

    public void CBylineClose()
    {
        LBylineReset();
        CBylineChanged?.Invoke();
    }

    internal void LBylineCommit(int at, long author, long picked)
    {
        CBylineClose();
        _cBylineImprint.LImprintAuthorInsert(at, author, picked);
    }

    internal void LBylineReset()
    {
        _cBylineIndex = -1;
        _cBylineCount = 0;
        _cBylineWord = string.Empty;
    }

    private IReadOnlyList<CAuthor> LBylineFind()
    {
        if (!_cBylineImprint.CImprintHeld)
        {
            return [];
        }

        try
        {
            return _cBylineDraftPort.LEngineBylineFind(_cBylineImprint.CImprintDesk.CDeskId, _cBylineWord)
                .Select(static row => new CAuthor(
                    row.LBylineRowId, row.LBylineRowLead, row.LBylineRowMark, row.LBylineRowTail))
                .ToList();
        }
        catch (Exception exception)
        {
            _cBylineImprint.CImprintDesk.LDeskRepaintShow(_cBylineNoticed, ".FindFailed", exception);
            return [];
        }
    }
}
