using System;
using System.Collections.Generic;
using System.IO;
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

    internal const string LLiveryAudioHead = "<audio controls class=\"llyn-audio\" src=\"";

    private const long LLiveryAudioCeiling = 32L * 1024L * 1024L;

    internal const string LLiveryVideoHead = "<video controls class=\"llyn-video\" src=\"";

    private const long LLiveryVideoCeiling = 128L * 1024L * 1024L;

    private readonly LTheme _lLiveryTheme;

    public LLiverySheet(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        _lLiveryTheme = theme;
    }

    public string LLiveryRead()
    {
        string css = LLiveryStyle.LLiveryStyleFormat(_lLiveryTheme)
            .Replace("`", string.Empty, StringComparison.Ordinal);

        return "Llyn writes this note and overwrites any edit made to it here.\n\n```css\n" + css + "\n```\n";
    }

    public LLiveryNote LLiveryFormat(
        LLiveryPage page,
        string style,
        Func<long, string> note,
        Func<string, string, string, string> link,
        Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentNullException.ThrowIfNull(lookup);

        StringBuilder sheet = new StringBuilder();
        sheet.Append(LLiveryMarkFormat(style)).Append("\n<div class=\"llyn\">\n\n");
        LLiveryHeader.LLiveryHeaderAppend(sheet, page, lookup);
        LLiverySound.LLiverySoundAppend(sheet, page, lookup, _lLiveryTheme);
        LLiveryHeader.LLiveryChipAppend(sheet, page, lookup);
        LLiveryRime.LLiveryRimeAppend(sheet, page, link, lookup);
        LLiveryScript.LLiveryScriptAppend(sheet, page, lookup);
        LLiveryCard.LLiveryCardAppend(sheet, page, note, lookup);
        LLiveryEtymology.LLiveryEtymologyAppend(sheet, page, note, lookup);
        sheet.Append("</div>\n");

        Dictionary<string, LParcel> parcels = new Dictionary<string, LParcel>(StringComparer.Ordinal);
        string body = LLiveryImageApply(sheet.ToString(), parcels);
        body = LLiveryMediaApply(body, parcels, LLiveryAudioHead, "</audio>", LLiveryAudioLoad);
        body = LLiveryMediaApply(body, parcels, LLiveryVideoHead, "</video>", LLiveryVideoLoad);

        return new LLiveryNote(body, [.. parcels.Values]);
    }

    public LLiveryNote LLiveryFormat(
        LLiveryStem stem, string style, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(stem);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        return LLiverySheetBuild(style, sheet => LLiveryXiesheng.LLiveryXieshengAppend(sheet, stem, note, lookup));
    }

    public LLiveryNote LLiveryFormat(
        LLiveryDiwei diwei, string style, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(diwei);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        return LLiverySheetBuild(style, sheet => LLiveryYunjing.LLiveryYunjingAppend(sheet, diwei, note, lookup));
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

    private LLiveryNote LLiverySheetBuild(string style, Action<StringBuilder> append)
    {
        StringBuilder sheet = new StringBuilder();
        sheet.Append(LLiveryMarkFormat(style)).Append("\n<div class=\"llyn\">\n\n");
        append(sheet);
        sheet.Append("</div>\n");

        return new LLiveryNote(sheet.ToString(), []);
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

                    note.Append(body, done, start - done).Append("<span class=\"llyn-blank\"></span>");
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

    private static string LLiveryMediaApply(
        string body, Dictionary<string, LParcel> parcels, string head, string tail, Func<string, LParcel?> load)
    {
        StringBuilder note = new StringBuilder(body.Length);
        int done = 0;
        int start = body.IndexOf(head, StringComparison.Ordinal);

        while (start >= 0)
        {
            int open = start + head.Length;
            int close = body.IndexOf('"', open);
            int end = close < 0 ? -1 : body.IndexOf(tail, close, StringComparison.Ordinal);
            if (end < 0)
            {
                break;
            }

            string path = WebUtility.HtmlDecode(body.Substring(open, close - open));
            if (load(path) is LParcel parcel)
            {
                parcels.TryAdd(parcel.LParcelId, parcel);
                note.Append(body, done, start - done)
                    .Append('[').Append(parcel.LParcelTitle).Append("](:/").Append(parcel.LParcelId).Append(')');
            }
            else
            {
                int lead = start > 0 && body[start - 1] == ' ' ? start - 1 : start;
                note.Append(body, done, lead - done);
            }

            done = end + tail.Length;

            start = body.IndexOf(head, end, StringComparison.Ordinal);
        }

        return note.Append(body, done, body.Length - done).ToString();
    }

    private static LParcel? LLiveryAudioLoad(string path)
    {
        string suffix = Path.GetExtension(path).ToLowerInvariant();
        string media = suffix switch
        {
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".m4a" => "audio/mp4",
            ".flac" => "audio/flac",
            ".opus" => "audio/opus",
            _ => string.Empty,
        };

        return LLiveryFileLoad(path, media, "audio" + suffix, LLiveryAudioCeiling);
    }

    private static LParcel? LLiveryVideoLoad(string path)
    {
        string suffix = Path.GetExtension(path).ToLowerInvariant();
        string media = suffix switch
        {
            ".mp4" or ".m4v" => "video/mp4",
            ".webm" => "video/webm",
            ".ogv" => "video/ogg",
            ".mov" => "video/quicktime",
            _ => string.Empty,
        };

        return LLiveryFileLoad(path, media, "video" + suffix, LLiveryVideoCeiling);
    }

    private static LParcel? LLiveryFileLoad(string path, string media, string title, long ceiling)
    {
        try
        {
            if (media.Length == 0
                || !Path.IsPathFullyQualified(path)
                || !File.Exists(path)
                || new FileInfo(path).Length > ceiling)
            {
                return null;
            }

            byte[] data = File.ReadAllBytes(path);
            string id = Convert.ToHexStringLower(SHA256.HashData(data))[..32];
            return new LParcel(id, title, media, data);
        }
        catch (Exception failure) when (failure is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
