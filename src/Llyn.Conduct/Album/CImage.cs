using System;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed class CImage
{
    private readonly CDesk _cImageDesk;

    internal CImage(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cImageDesk = desk;
    }

    public void CImageLocationSet(long imageId, string location)
    {
        _cImageDesk.CDeskDraft.CDeskDraftEasel?.LEaselImageSet(imageId, location, true);
    }

    public void CImageAdd(bool collocated, long? cardId)
    {
        _cImageDesk.CDeskDraft.CDeskDraftEasel?.LEaselImageAdd(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId ?? 0);
    }

    public void CImageRemove(bool collocated, long? cardId, long imageId)
    {
        _cImageDesk.CDeskDraft.CDeskDraftEasel?.LEaselImageRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId ?? 0, imageId);
    }

    public void CImageFileSet(long imageId, string? file)
    {
        if (file is null)
        {
            return;
        }

        _cImageDesk.CDeskDraft.CDeskDraftEasel?.LEaselImageSet(imageId, file, false);
    }
}
