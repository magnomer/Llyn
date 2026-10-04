using System;

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
        _cImageDesk.CDeskEasel?.LEaselImageSet(imageId, location, true);
    }

    public void CImageAdd(long? cardId)
    {
        _cImageDesk.CDeskEasel?.LEaselImageAdd(cardId ?? 0);
    }

    public void CImageRemove(long imageId)
    {
        _cImageDesk.CDeskEasel?.LEaselImageRemove(imageId);
    }

    public void CImageFileSet(long imageId, string? file)
    {
        if (file is null)
        {
            return;
        }

        _cImageDesk.CDeskEasel?.LEaselImageSet(imageId, file, false);
    }
}
