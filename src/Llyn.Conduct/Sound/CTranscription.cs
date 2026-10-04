using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

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
        if (_cTranscriptionDesk.CDeskTenure?.LTenureTranscriptionRead() is not LTranscriptionSheet sheet)
        {
            return new CTranscriptionSheet(false, false, []);
        }

        IReadOnlyList<CTranscriptionDraft> drafts = CSounding.CSoundingTranscriptionRead(
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

        _cTranscriptionDesk.CDeskQuill?.LQuillTranscriptionSet(transcription, text);
    }

    public void CTranscriptionSchemeSet(long transcription, string scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        _cTranscriptionDesk.CDeskQuill?.LQuillSchemeSet(transcription, scheme);
    }

    public void CTranscriptionAdd(long? transcription)
    {
        _cTranscriptionDesk.CDeskQuill?.LQuillTranscriptionAdd(transcription ?? 0);
    }

    public void CTranscriptionRemove(long transcription)
    {
        _cTranscriptionDesk.CDeskQuill?.LQuillTranscriptionRemove(transcription);
    }
}
