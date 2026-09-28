using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CByline
{
    private readonly CImprint _cBylineImprint;

    private readonly LDraftPort _cBylineDraftPort;

    private int _cBylineIndex = -1;

    private int _cBylineCount;

    private string _cBylineWord = string.Empty;

    internal CByline(CImprint imprint, LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(imprint);
        ArgumentNullException.ThrowIfNull(drafts);

        _cBylineImprint = imprint;
        _cBylineDraftPort = drafts;
    }

    public event Action? CBylineChanged;

    public bool CBylineShown => _cBylineCount > 0;

    public int CBylineIndex => _cBylineIndex;

    public string CBylineWord => _cBylineWord;

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

        int chosen = _cBylineIndex < 0 && step < 0 ? 0 : _cBylineIndex;
        _cBylineIndex = ((chosen + step) % _cBylineCount + _cBylineCount) % _cBylineCount;
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
                .Select(static row => new CAuthor(row.LAuthorId, row.LAuthorName))
                .ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }
}
