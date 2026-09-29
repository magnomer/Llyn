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
}
