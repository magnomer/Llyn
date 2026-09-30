using System;
using System.Globalization;

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

    public static (TimeSpan, TimeSpan?) LVideoSpanRead(string span)
    {
        ArgumentNullException.ThrowIfNull(span);

        string[] parts = span.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        TimeSpan from = parts.Length > 0 && LVideoMomentParse(parts[0]) is TimeSpan start ? start : TimeSpan.Zero;
        TimeSpan? until = parts.Length > 1 ? LVideoMomentParse(parts[1]) : null;
        return (from, until);
    }

    private static TimeSpan? LVideoMomentParse(string moment)
    {
        string[] fields = moment.Split(':', StringSplitOptions.TrimEntries);
        if (fields.Length is < 2 or > 3)
        {
            return null;
        }

        TimeSpan parsed = TimeSpan.Zero;
        foreach (string field in fields)
        {
            if (!int.TryParse(field, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
            {
                return null;
            }

            parsed = (parsed * 60) + TimeSpan.FromSeconds(value);
        }

        return parsed;
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
