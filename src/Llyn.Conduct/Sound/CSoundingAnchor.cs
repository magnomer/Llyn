using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Conduct;

public sealed class CSoundingAnchor
{
    private readonly CDesk _cSoundingAnchorDesk;

    private readonly CTimbre _cSoundingAnchorTimbre;

    private long? _cSoundingAnchorReflex;

    internal CSoundingAnchor(CDesk desk, CTimbre timbre)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(timbre);

        _cSoundingAnchorDesk = desk;
        _cSoundingAnchorTimbre = timbre;
    }

    public static CSoundingAnchor CSoundingAnchorCreate(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        return new CSoundingAnchor(editor.CEditorDesk, editor.CEditorTimbre);
    }

    public CAnchor CSoundingAnchorOpen(long reflex)
    {
        _cSoundingAnchorReflex = reflex;
        IReadOnlyList<CAnchorRow> rows = (_cSoundingAnchorTimbre.LTimbreQuill?.LQuillAnchorScan(reflex) ?? [])
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
            _cSoundingAnchorTimbre.LTimbreQuill?.LReflexAnchorSet(reflex, fanqie, anchored);
        }
    }

    public void CSoundingAnchorClose()
    {
        _cSoundingAnchorReflex = null;
    }
}
