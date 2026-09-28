using System;

namespace Llyn.Core;

public sealed record LVideo(
    long LVideoId,
    LStateValue LVideoLocation,
    LStateValue LVideoSpan)
{
    private const int LVideoFilmLength = 11;

    public LStateValue LVideoLocation { get; init; } = LVideoLocation ?? LStateValue.LStateValueUnspecified;

    public LStateValue LVideoSpan { get; init; } = LVideoSpan ?? LStateValue.LStateValueUnspecified;

    public static string? LVideoFilmRead(Uri address)
    {
        ArgumentNullException.ThrowIfNull(address);

        string house = address.Host.ToLowerInvariant();
        if (house.StartsWith("www.", StringComparison.Ordinal))
        {
            house = house[4..];
        }

        if (string.Equals(house, "youtu.be", StringComparison.Ordinal))
        {
            return LVideoFilmCheck(address.AbsolutePath.Trim('/'));
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
                return LVideoFilmCheck(path[opening.Length..].Trim('/'));
            }
        }

        foreach (string field in address.Query.TrimStart('?').Split('&'))
        {
            if (field.StartsWith("v=", StringComparison.OrdinalIgnoreCase))
            {
                return LVideoFilmCheck(field[2..]);
            }
        }

        return null;
    }

    private static string? LVideoFilmCheck(string film)
    {
        if (film.Length != LVideoFilmLength)
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
