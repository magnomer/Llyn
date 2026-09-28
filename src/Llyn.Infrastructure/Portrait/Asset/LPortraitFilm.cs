using System;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LPortraitFilm
{
    public static string? LPortraitFilmRead(string location)
    {
        if (string.IsNullOrWhiteSpace(location)
            || !Uri.TryCreate(location.Trim(), UriKind.Absolute, out Uri? address)
            || address.IsFile)
        {
            return null;
        }

        return LVideo.LVideoFilmRead(address);
    }
}
