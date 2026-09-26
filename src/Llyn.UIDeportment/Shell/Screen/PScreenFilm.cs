using System;

namespace Llyn.UIDeportment;

public partial class PScreen
{
    private const int PScreenFilmLength = 11;

    internal static string? PScreenFilmRead(Uri address)
    {
        ArgumentNullException.ThrowIfNull(address);

        string house = address.Host.ToLowerInvariant();
        if (house.StartsWith("www.", StringComparison.Ordinal))
        {
            house = house[4..];
        }

        if (string.Equals(house, "youtu.be", StringComparison.Ordinal))
        {
            return PScreenFilmCheck(address.AbsolutePath.Trim('/'));
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
                return PScreenFilmCheck(path[opening.Length..].Trim('/'));
            }
        }

        foreach (string field in address.Query.TrimStart('?').Split('&'))
        {
            if (field.StartsWith("v=", StringComparison.OrdinalIgnoreCase))
            {
                return PScreenFilmCheck(field[2..]);
            }
        }

        return null;
    }

    private static string? PScreenFilmCheck(string film)
    {
        if (film.Length != PScreenFilmLength)
        {
            return null;
        }

        foreach (char letter in film)
        {
            if (!char.IsAsciiLetterOrDigit(letter) && letter != '-' && letter != '_')
            {
                return null;
            }
        }

        return film;
    }
}
