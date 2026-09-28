using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LSounding
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LDraftPort _lDraftPort;

    internal LSounding(LPhonologyPort phonology, LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(drafts);

        _lPhonologyPort = phonology;
        _lDraftPort = drafts;
    }

    public event Action? LSoundingChanged;

    public event Action<string, Exception>? LSoundingFailed;

    public IReadOnlyList<CFanqieGroup> LSoundingFanqieRead(long? entry)
    {
        return LSoundingFanqieRead(LSoundingFanqieFind(entry));
    }

    internal IReadOnlyList<LFanqieGroup> LSoundingFanqieFind(long? entry)
    {
        if (entry is not long id)
        {
            return [];
        }

        try
        {
            _lPhonologyPort.LEngineFanqieStart(id);
            return _lPhonologyPort.LEngineFanqieDivide(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public string LSoundingReadingRead(long? entry, string headword)
    {
        return LFanqieGroup.LFanqieReadingFormat(LSoundingFanqieFind(entry), headword);
    }

    internal bool LSoundingAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return _lDraftPort.LEngineAnchorCheck(rows, headword);
    }

    internal string LSoundingAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return _lDraftPort.LEngineAnchorFormat(rows, anchors, headword, separator);
    }

    internal IReadOnlyList<CAnchorRow> LSoundingAnchorScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone)
    {
        return LSplice.LSpliceBuild(
            _lDraftPort.LEngineAnchorScan(rows, anchors, language, reflex, tone),
            static row => new CAnchorRow(
                row.LAnchorRowFanqie.LFanqieRowId,
                row.LAnchorRowFanqie.LFanqieRowSummary,
                row.LAnchorRowHeld,
                row.LAnchorRowEstimated));
    }

    internal IReadOnlyList<LFanqieRow> LSoundingAnchorRead(long? entry)
    {
        List<LFanqieRow> rows = [];
        foreach (LFanqieGroup group in LSoundingFanqieFind(entry))
        {
            rows.AddRange(group.LFanqieGroupRows);
        }

        return rows;
    }

    public void LSoundingFanqieRebuild(long? entry)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineFanqieRebuild(id);
        }
        catch (Exception exception)
        {
            LSoundingFailed?.Invoke("Display.FanqieRebuildFailed", exception);
            return;
        }

        LSoundingChanged?.Invoke();
    }

    public void LSoundingFanqieSet(long? entry, long fanqieId, int rank)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineFanqieSet(id, fanqieId, rank);
        }
        catch (Exception exception)
        {
            LSoundingFailed?.Invoke("Display.FanqieRepresentativeFailed", exception);
            return;
        }

        LSoundingChanged?.Invoke();
    }

    public IReadOnlyList<CScriptGroup> LSoundingScriptRead(long? entry)
    {
        if (entry is not long id)
        {
            return [];
        }

        try
        {
            _lPhonologyPort.LEngineScriptStart(id);
            return LSoundingScriptRead(_lPhonologyPort.LEngineScriptDivide(id));
        }
        catch (Exception)
        {
            return [];
        }
    }

    public void LSoundingScriptRebuild(long? entry)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineScriptRebuild(id);
        }
        catch (Exception exception)
        {
            LSoundingFailed?.Invoke("Display.ScriptRebuildFailed", exception);
            return;
        }

        LSoundingChanged?.Invoke();
    }

    public IReadOnlyList<CParadigmSlot> LSoundingParadigmRead(long? entry)
    {
        return LSoundingParadigmRead(LSoundingParadigmFind(entry));
    }

    internal string LSoundingLanguageRead(long? entry)
    {
        return LParadigm.LParadigmLanguageRead(LSoundingParadigmFind(entry));
    }

    private IReadOnlyList<LParadigmSlot> LSoundingParadigmFind(long? entry)
    {
        if (entry is not long id)
        {
            return [];
        }

        try
        {
            return _lPhonologyPort.LEngineParadigmShow(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    internal static IReadOnlyList<CFanqieGroup> LSoundingFanqieRead(IReadOnlyList<LFanqieGroup> groups)
    {
        return LSplice.LSpliceBuild(
            groups,
            static group => new CFanqieGroup(
                group.LFanqieGroupHeading,
                group.LFanqieGroupLabel,
                group.LFanqieGroupSource,
                group.LFanqieGroupStems,
                LSplice.LSpliceBuild(group.LFanqieGroupRows, LSoundingRowRead)));
    }

    private static CFanqieRow LSoundingRowRead(LFanqieRow row)
    {
        return new CFanqieRow(
            row.LFanqieRowId,
            row.LFanqieRowRepresentative,
            row.LFanqieRowMarked,
            row.LFanqieRowPrimary,
            row.LFanqieRowOrder,
            row.LFanqieRowClosed,
            row.LFanqieRowSlashed,
            row.LFanqieRowLabel,
            row.LFanqieRowInitial,
            row.LFanqieRowCell,
            row.LFanqieRowBracketed,
            row.LFanqieRowKnotted,
            row.LFanqieRowMedial,
            row.LFanqieRowGraded,
            row.LFanqieRowTone,
            row.LFanqieRowSpelling,
            row.LFanqieRowRemainder);
    }

    internal static IReadOnlyList<CScriptGroup> LSoundingScriptRead(IReadOnlyList<LScriptGroup> groups)
    {
        return LSplice.LSpliceBuild(
            groups,
            static group => new CScriptGroup(
                group.LScriptGroupHeading,
                group.LScriptGroupStyle,
                group.LScriptGroupGloss,
                LSplice.LSpliceBuild(group.LScriptGroupImages, LSoundingImageRead)));
    }

    private static CScriptImage LSoundingImageRead(LScriptImage image)
    {
        return new CScriptImage(image.LScriptImageData, image.LScriptImageCaption, image.LScriptImageEpoch);
    }

    internal static IReadOnlyList<CParadigmSlot> LSoundingParadigmRead(IReadOnlyList<LParadigmSlot> slots)
    {
        return LSplice.LSpliceBuild(LParadigmRow.LParadigmRowScan(slots), LSoundingSlotRead);
    }

    private static CParadigmSlot LSoundingSlotRead(LParadigmRow row)
    {
        return new CParadigmSlot(
            row.LParadigmRowPart,
            row.LParadigmRowName,
            row.LParadigmRowFirst.LParadigmSlotInflection?.LInflectionText,
            row.LParadigmRowFirst.LParadigmSlotUncertain);
    }

    internal static CFrequency? LSoundingFrequencyRead(IReadOnlyList<LFrequency> rows, string once)
    {
        return LDisplay.LDisplayFrequencyCheck(rows)
            ? new CFrequency(LDisplay.LDisplayBandResolve(rows), LDisplay.LDisplaySourceFormat(rows, once))
            : null;
    }
}
