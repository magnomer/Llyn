using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFanqieSource
{
    private const string LFanqieSourceToken = "{word}";

    private const RegexOptions LFanqieSourceLoose =
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    private static readonly Regex LFanqieSourceTable = new("<table[^>]*>(.*?)</table>", LFanqieSourceLoose);

    private static readonly Regex LFanqieSourceLine = new("<tr[^>]*>(.*?)</tr>", LFanqieSourceLoose);

    private static readonly Regex LFanqieSourceHead = new("<th[^>]*>(.*?)</th>", LFanqieSourceLoose);

    private static readonly Regex LFanqieSourceCell = new("<td[^>]*>(.*?)</td>", LFanqieSourceLoose);

    private static readonly Regex LFanqieSourceStrike = new("<s(?:\\s[^>]*)?>.*?</s>", LFanqieSourceLoose);

    private static readonly Regex LFanqieSourceAnchor = new("<a[\\s>]", LFanqieSourceLoose);

    private static readonly Regex LFanqieSourceTag = new("<[^>]+>", RegexOptions.CultureInvariant);

    private static readonly TimeSpan LFanqieSourcePatience = TimeSpan.FromSeconds(2);

    public static async Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LFanqieSourceFind(
        HttpClient client,
        LFanqieBook book,
        string character,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        string? body = await LFanqieBodyRead(client, book, character, cancellation).ConfigureAwait(false);
        if (body is null)
        {
            return ([], false);
        }

        try
        {
            if (!string.IsNullOrEmpty(book.LFanqieBookBusy)
                && Regex.IsMatch(body, book.LFanqieBookBusy, RegexOptions.CultureInvariant, LFanqieSourcePatience))
            {
                return ([], false);
            }

            LFanqieShape shape = LFanqieShape.LFanqieShapeCreate(book, character);
            IReadOnlyList<LFanqieRow> found = shape.LFanqieShapeLine is null
                ? LFanqieTableScan(shape, body)
                : LFanqieMatchScan(shape, body);
            return (LFanqieOriginSet(book, character, found), true);
        }
        catch (RegexMatchTimeoutException)
        {
            return ([], true);
        }
    }

    private static async Task<string?> LFanqieBodyRead(
        HttpClient client, LFanqieBook book, string character, CancellationToken cancellation)
    {
        List<KeyValuePair<string, string>> fields = [];
        foreach (KeyValuePair<string, string> field in book.LFanqieBookForm)
        {
            fields.Add(new KeyValuePair<string, string>(
                field.Key, field.Value.Replace(LFanqieSourceToken, character, StringComparison.Ordinal)));
        }

        try
        {
            using HttpRequestMessage request = fields.Count == 0
                ? new HttpRequestMessage(
                    HttpMethod.Get,
                    book.LFanqieBookUrl.Replace(
                        LFanqieSourceToken, Uri.EscapeDataString(character), StringComparison.Ordinal))
                : new HttpRequestMessage(HttpMethod.Post, book.LFanqieBookUrl)
                {
                    Content = new FormUrlEncodedContent(fields),
                };
            using HttpResponseMessage response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return string.Empty;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException)
        {
            return null;
        }
    }

    private static IReadOnlyList<LFanqieRow> LFanqieOriginSet(
        LFanqieBook book, string character, IReadOnlyList<LFanqieRow> found)
    {
        List<LFanqieRow> rows = [];
        foreach (LFanqieRow row in found)
        {
            rows.Add(row with
            {
                LFanqieRowCharacter = character,
                LFanqieRowBook = book.LFanqieBookName,
                LFanqieRowPosition = rows.Count,
                LFanqieRowSource = book.LFanqieBookSource,
            });
        }

        return rows;
    }

    private static IReadOnlyList<LFanqieRow> LFanqieMatchScan(LFanqieShape shape, string body)
    {
        List<LFanqieRow> rows = [];
        foreach (Match match in shape.LFanqieShapeLine!.Matches(body))
        {
            string rime = match.Groups["rime"].Value.Trim();
            string heading = match.Groups["heading"].Value.Trim();
            string division = match.Groups["division"].Value.Trim();
            string tone = match.Groups["tone"].Value.Trim();
            string rounded = match.Groups["rounded"].Value;
            string text = string.Join(' ', LFanqiePartScan(
                match.Groups["initial"].Value.Trim(),
                heading.Length == 0 ? rime : rime + '[' + heading + ']',
                division.Length == 0 ? tone : division + '等' + tone));
            rows.Add(new LFanqieRow(
                string.Empty,
                string.Empty,
                0,
                text,
                match.Groups["initial"].Value.Trim(),
                rime,
                heading,
                division,
                tone,
                rounded.Length > 0 && (shape.LFanqieShapeRounded?.IsMatch(rounded) ?? false),
                LFanqieRowSpelling: match.Groups["spelling"].Value.Trim()));
        }

        return rows;
    }

    private static IReadOnlyList<LFanqieRow> LFanqieTableScan(LFanqieShape shape, string body)
    {
        List<LFanqieRow> rows = [];
        foreach (Match table in LFanqieSourceTable.Matches(body))
        {
            List<LFanqieColumn> columns = [];
            foreach (Match line in LFanqieSourceLine.Matches(table.Groups[1].Value))
            {
                string inner = line.Groups[1].Value;
                MatchCollection heads = LFanqieSourceHead.Matches(inner);
                if (heads.Count > 0)
                {
                    columns.Clear();
                    foreach (Match head in heads)
                    {
                        columns.Add(LFanqieColumnRead(shape, LFanqieTextNormalize(head.Groups[1].Value)));
                    }

                    continue;
                }

                rows.AddRange(LFanqieLineScan(shape, columns, inner));
            }
        }

        return rows;
    }

    private static LFanqieColumn LFanqieColumnRead(LFanqieShape shape, string text)
    {
        Match match = shape.LFanqieShapeColumn?.Match(text) ?? Match.Empty;
        return match.Success
            ? new LFanqieColumn(text, match.Groups["division"].Value, match.Groups["tone"].Value)
            : new LFanqieColumn(text, string.Empty, string.Empty);
    }

    private static IEnumerable<LFanqieRow> LFanqieLineScan(
        LFanqieShape shape, IReadOnlyList<LFanqieColumn> columns, string inner)
    {
        MatchCollection cells = LFanqieSourceCell.Matches(inner);
        if (cells.Count < 2)
        {
            yield break;
        }

        string initial = LFanqieTextNormalize(cells[0].Groups[1].Value);
        for (int index = 1; index < cells.Count; index++)
        {
            LFanqieColumn column = index < columns.Count ? columns[index] : new LFanqieColumn("", "", "");
            foreach (string group in LFanqieGroupScan(shape, cells[index].Groups[1].Value))
            {
                if (shape.LFanqieShapeMarker.IsMatch(group))
                {
                    yield return LFanqieGroupRead(shape, initial, column, group);
                }
            }
        }
    }

    private static IEnumerable<string> LFanqieGroupScan(LFanqieShape shape, string cell)
    {
        return shape.LFanqieShapeSplit is null ? [cell] : shape.LFanqieShapeSplit.Split(cell);
    }

    private static LFanqieRow LFanqieGroupRead(LFanqieShape shape, string initial, LFanqieColumn column, string group)
    {
        Match head = shape.LFanqieShapeHead?.Match(group) ?? Match.Empty;
        string division = column.LFanqieColumnDivision.Length > 0
            ? column.LFanqieColumnDivision
            : head.Success ? head.Groups["division"].Value : string.Empty;
        string text = string.Join(' ', LFanqiePartScan(initial, LFanqieCellRead(group), column.LFanqieColumnText));

        Match spelling = shape.LFanqieShapeSpelling?.Match(group) ?? Match.Empty;

        return new LFanqieRow(
            string.Empty,
            string.Empty,
            0,
            text,
            initial,
            head.Success ? head.Groups["rime"].Value.Trim() : string.Empty,
            head.Success ? head.Groups["heading"].Value.Trim() : string.Empty,
            division.Trim(),
            column.LFanqieColumnTone,
            shape.LFanqieShapeRounded?.IsMatch(group) ?? false,
            LFanqieRowSpelling: spelling.Success ? LFanqieTextNormalize(spelling.Groups[1].Value) : string.Empty);
    }

    private static IEnumerable<string> LFanqiePartScan(params string[] parts)
    {
        foreach (string part in parts)
        {
            if (part.Length > 0)
            {
                yield return part;
            }
        }
    }

    private static string LFanqieCellRead(string cell)
    {
        Match anchor = LFanqieSourceAnchor.Match(cell);
        string head = anchor.Success ? cell[..anchor.Index] : cell;
        return LFanqieTextNormalize(LFanqieSourceStrike.Replace(head, string.Empty));
    }

    private static string LFanqieTextNormalize(string raw)
    {
        string plain = WebUtility.HtmlDecode(LFanqieSourceTag.Replace(raw, string.Empty));
        return string.Join(' ', plain.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
