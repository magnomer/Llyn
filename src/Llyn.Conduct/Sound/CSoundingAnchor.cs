using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Conduct;

public sealed class CSoundingAnchor
{
    private readonly CDesk _cSoundingAnchorDesk;

    private long? _cSoundingAnchorReflex;

    internal CSoundingAnchor(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cSoundingAnchorDesk = desk;
    }

    public static CSoundingAnchor CSoundingAnchorCreate(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        return new CSoundingAnchor(editor.CEditorDesk);
    }

    public CAnchor CSoundingAnchorOpen(long reflex)
    {
        _cSoundingAnchorReflex = reflex;
        IReadOnlyList<CAnchorRow> rows = (_cSoundingAnchorDesk.CDeskTenure?.LTenureAnchorScan(reflex) ?? [])
            .Select(static row => new CAnchorRow(
                row.LAnchorRowFanqie.LFanqieRowId,
                row.LAnchorRowFanqie.LFanqieRowSummary,
                row.LAnchorRowHeld,
                row.LAnchorRowEstimated))
            .ToList();
        return new CAnchor(rows, rows.Count == 0);
    }

    public void CSoundingAnchorSet(long fanqie, bool anchored)
    {
        if (_cSoundingAnchorReflex is long reflex)
        {
            _cSoundingAnchorDesk.CDeskQuill?.LQuillAnchorSet(reflex, fanqie, anchored);
        }
    }

    public void CSoundingAnchorClose()
    {
        _cSoundingAnchorReflex = null;
    }
}
