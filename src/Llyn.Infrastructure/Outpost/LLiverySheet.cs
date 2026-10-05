using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LLiverySheet : LLivery
{
    private const string LLiveryImageHead = "<img src=\"data:";

    private const long LLiveryImageCeiling = 24L * 1024L * 1024L;

    private readonly LTheme _lLiveryTheme;

    public LLiverySheet(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        _lLiveryTheme = theme;
    }

    public string LLiveryRead()
    {
        string css = LSheetStyle.LSheetStyleFormat(_lLiveryTheme, ".llyn")
            .Replace("`", string.Empty, StringComparison.Ordinal);

        return "Llyn writes this note and overwrites any edit made to it here.\n\n```css\n" + css + "\n```\n";
    }

    public LLiveryNote LLiveryFormat(LPortraitPage page, string style)
    {
        ArgumentNullException.ThrowIfNull(page);

        StringBuilder sheet = new StringBuilder();
        sheet.Append(LLiveryMarkFormat(style)).Append("\n<div class=\"llyn\">\n");
        LSheet.LSheetBodyAppend(sheet, page);
        sheet.Append("</div>");

        Dictionary<string, LParcel> parcels = new Dictionary<string, LParcel>(StringComparer.Ordinal);
        string body = LLiveryImageApply(sheet.ToString(), parcels);

        return new LLiveryNote(LLiveryLineApply(body), [.. parcels.Values]);
    }

    public string LLiveryMarkFormat(string style)
    {
        LLiveryStyleCheck(style);
        return "<style>@import \":/" + style + "\";</style>";
    }

    public string LLiveryIdFormat(string seed)
    {
        ArgumentNullException.ThrowIfNull(seed);
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(seed)))[..32];
    }

    public string LLiveryDigestFormat(
        LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(parcels);

        StringBuilder text = new StringBuilder();
        text.Append(note.LOutpostNoteTitle).Append('\0')
            .Append(note.LOutpostNoteFolder).Append('\0')
            .Append(note.LOutpostNoteBody).Append('\0');
        foreach (string tag in tags.Order(StringComparer.Ordinal))
        {
            text.Append(tag).Append('\u0001');
        }

        text.Append('\0');
        foreach (LParcel parcel in parcels)
        {
            text.Append(parcel.LParcelId).Append('\u0001');
        }

        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
    }

    private static void LLiveryStyleCheck(string style)
    {
        bool valid = style is { Length: 32 };

        foreach (char letter in style ?? string.Empty)
        {
            valid &= letter is (>= '0' and <= '9') or (>= 'a' and <= 'f');
        }

        if (!valid)
        {
            throw new ArgumentException("The style note id must be 32 lowercase hex characters.", nameof(style));
        }
    }

    private static string LLiveryImageApply(string body, Dictionary<string, LParcel> parcels)
    {
        StringBuilder note = new StringBuilder(body.Length);
        int done = 0;
        int start = body.IndexOf(LLiveryImageHead, StringComparison.Ordinal);

        while (start >= 0)
        {
            int open = start + "<img src=\"".Length;
            int close = body.IndexOf('"', open);
            if (close < 0)
            {
                break;
            }

            string address = WebUtility.HtmlDecode(body.Substring(open, close - open));
            if (LPortraitAsset.LPortraitAssetLoad(address) is LPortraitAsset asset)
            {
                if (!asset.LPortraitAssetMedia.StartsWith("image/", StringComparison.Ordinal)
                    || asset.LPortraitAssetData.LongLength > LLiveryImageCeiling)
                {
                    int end = body.IndexOf('>', close);
                    if (end < 0)
                    {
                        break;
                    }

                    note.Append(body, done, start - done).Append("<span class=\"blank\"></span>");
                    done = end + 1;
                    start = body.IndexOf(LLiveryImageHead, done, StringComparison.Ordinal);
                    continue;
                }

                string id = Convert.ToHexStringLower(SHA256.HashData(asset.LPortraitAssetData))[..32];
                string title = "image" + asset.LPortraitAssetSuffix;
                parcels.TryAdd(id, new LParcel(id, title, asset.LPortraitAssetMedia, asset.LPortraitAssetData));
                note.Append(body, done, open - done).Append(":/").Append(id);
                done = close;
            }

            start = body.IndexOf(LLiveryImageHead, close, StringComparison.Ordinal);
        }

        return note.Append(body, done, body.Length - done).ToString();
    }

    private static string LLiveryLineApply(string body)
    {
        string flat = body.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        StringBuilder note = new StringBuilder(flat.Length);

        foreach (string line in flat.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                note.Append("&#10;");
                continue;
            }

            if (note.Length > 0)
            {
                note.Append('\n');
            }

            note.Append(line);
        }

        return note.Append('\n').ToString();
    }
}
