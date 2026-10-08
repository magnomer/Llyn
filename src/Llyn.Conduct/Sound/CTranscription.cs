using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTranscription
{
    private readonly CDesk _cTranscriptionDesk;

    internal CTranscription(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cTranscriptionDesk = desk;
    }

    public CTranscriptionSheet CTranscriptionRead()
    {
        if (_cTranscriptionDesk.CDeskDraft.CDeskDraftTenure is not LTenure held
            || new LQuillTranscription(held).LQuillTranscriptionRead() is not LTranscriptionSheet sheet)
        {
            return new CTranscriptionSheet(false, false, []);
        }

        IReadOnlyList<CTranscriptionDraft> drafts = CTranscriptionDraft.CTranscriptionDraftRead(
            sheet.LTranscriptionSheetRows.Select(static row => row.LTranscriptionRowDraft).ToList());
        return new CTranscriptionSheet(
            sheet.LTranscriptionSheetShown,
            sheet.LTranscriptionSheetFree,
            sheet.LTranscriptionSheetRows
                .Select((row, index) => new CTranscriptionRow(
                    drafts[index],
                    row.LTranscriptionRowSchemes
                        .Select(static scheme => new CScheme(scheme.LSchemeRowName, scheme.LSchemeRowTaken))
                        .ToList()))
                .ToList());
    }

    public void CTranscriptionSet(long transcription, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _cTranscriptionDesk.CDeskDraft.CDeskDraftTranscription?.LQuillTranscriptionSet(transcription, text);
    }

    public void CTranscriptionSchemeSet(long transcription, string scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        _cTranscriptionDesk.CDeskDraft.CDeskDraftTranscription?.LTranscriptionSchemeSet(transcription, scheme);
    }

    public void CTranscriptionAdd(long? transcription)
    {
        _cTranscriptionDesk.CDeskDraft.CDeskDraftTranscription?.LQuillTranscriptionAdd(transcription ?? 0);
    }

    public void CTranscriptionRemove(long transcription)
    {
        _cTranscriptionDesk.CDeskDraft.CDeskDraftTranscription?.LQuillTranscriptionRemove(transcription);
    }
}
