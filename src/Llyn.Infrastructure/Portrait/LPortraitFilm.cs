using System;

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

        string house = address.Host.ToLowerInvariant();
        if (house.StartsWith("www.", StringComparison.Ordinal))
        {
            house = house[4..];
        }

        if (string.Equals(house, "youtu.be", StringComparison.Ordinal))
        {
            return LPortraitFilmCheck(address.AbsolutePath.Trim('/'));
        }

        if (!string.Equals(house, "youtube.com", StringComparison.Ordinal)
            && !string.Equals(house, "youtube-nocookie.com", StringComparison.Ordinal))
        {
            return null;
        }

        string path = address.AbsolutePath;
        string[] openings = ["/embed/", "/shorts/", "/v/", "/live/"];
        foreach (string opening in openings)
        {
            if (path.StartsWith(opening, StringComparison.OrdinalIgnoreCase))
            {
                return LPortraitFilmCheck(path[opening.Length..].Trim('/'));
            }
        }

        foreach (string field in address.Query.TrimStart('?').Split('&'))
        {
            if (field.StartsWith("v=", StringComparison.OrdinalIgnoreCase))
            {
                return LPortraitFilmCheck(field[2..]);
            }
        }

        return null;
    }

    private static string? LPortraitFilmCheck(string film)
    {
        if (film.Length == 0)
        {
            return null;
        }

        foreach (char letter in film)
        {
            if (!char.IsLetterOrDigit(letter) && letter != '-' && letter != '_')
            {
                return null;
            }
        }

        return film;
    }
}
