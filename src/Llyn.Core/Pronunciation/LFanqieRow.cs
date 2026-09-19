using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LFanqieRow(
    string LFanqieRowCharacter,
    string LFanqieRowBook,
    int LFanqieRowPosition,
    string LFanqieRowText,
    string LFanqieRowInitial = "",
    string LFanqieRowRime = "",
    string LFanqieRowHeading = "",
    string LFanqieRowDivision = "",
    string LFanqieRowTone = "",
    bool LFanqieRowRounded = false,
    string? LFanqieRowSource = null,
    string LFanqieRowSpelling = "",
    string LFanqieRowReading = "",
    string LFanqieRowClass = "",
    long LFanqieRowId = 0,
    string LFanqieRowLabel = "",
    string LFanqieRowSummary = "")
{
    private const string LFanqieRowUnrounded = "開";

    private const string LFanqieRowMark = "合";

    private const string LFanqieRowSuffix = "等";

    public string LFanqieRowSource { get; init; } = LFanqieRowSource ?? LFanqieRowBook;

    public string LFanqieRowReading { get; init; } = LFanqieRowReading ?? string.Empty;

    public string LFanqieRowClass { get; init; } = LFanqieRowClass ?? string.Empty;

    public string LFanqieRowLabel { get; init; } = LFanqieRowLabel ?? string.Empty;

    public string LFanqieRowSummary { get; init; } = LFanqieRowSummary ?? string.Empty;

    public bool LFanqieRowStored => LFanqieRowId > 0;

    public bool LFanqieRowParted => LFanqieRowInitial.Length > 0 || LFanqieRowRime.Length > 0;

    public bool LFanqieRowClassed => LFanqieRowClass.Length > 0;

    public bool LFanqieRowSpoken => LFanqieRowReading.Length > 0;

    public string LFanqieRowSlashed => LFanqieRowSpoken ? '/' + LFanqieRowReading + '/' : string.Empty;

    public string LFanqieRowBracketed =>
        LFanqieRowHeading.Length == 0 ? string.Empty : '[' + LFanqieRowHeading + ']';

    public string LFanqieRowGraded =>
        LFanqieRowDivision.Length == 0 ? string.Empty : LFanqieRowDivision + LFanqieRowSuffix;

    public string LFanqieRowMedial =>
        !LFanqieRowParted ? string.Empty : LFanqieRowRounded ? LFanqieRowMark : LFanqieRowUnrounded;

    public string LFanqieRowCell => LDiwei.LDiweiRimeFormat(LFanqieRowRime, LFanqieRowDivision, LFanqieRowRounded);

    public string LFanqieRowRemainder => LFanqieRowParted ? string.Empty : LFanqieRowText;

    public bool LFanqieRowMatch(string character, LFanqieBook book)
    {
        ArgumentNullException.ThrowIfNull(book);

        return string.Equals(LFanqieRowCharacter, character, StringComparison.Ordinal)
            && string.Equals(LFanqieRowBook, book.LFanqieBookName, StringComparison.Ordinal)
            && string.Equals(LFanqieRowSource, book.LFanqieBookSource, StringComparison.Ordinal);
    }

    public static IReadOnlyList<string> LFanqieCharacterScan(IReadOnlyList<LFanqieRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<string> characters = [];
        foreach (LFanqieRow row in rows)
        {
            if (!characters.Contains(row.LFanqieRowCharacter))
            {
                characters.Add(row.LFanqieRowCharacter);
            }
        }

        return characters;
    }

    public static IReadOnlyList<LFanqieRow>? LFanqieRowScan(
        IReadOnlyList<LFanqieRow> rows, string character, LFanqieBook book)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<LFanqieRow> found = [];
        foreach (LFanqieRow row in rows)
        {
            if (row.LFanqieRowMatch(character, book))
            {
                found.Add(row);
            }
        }

        return found.Count == 0 ? null : found;
    }

    public LFanqieRow LFanqieRowFormat(string pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        string label = !LFanqieRowClassed
            ? string.Empty
            : pattern.Length == 0
                ? LFanqieRowClass
                : string.Format(CultureInfo.CurrentCulture, pattern, LFanqieRowClass);
        List<string> parts = [LFanqieRowBook];
        if (LFanqieRowSpoken)
        {
            parts.Add(LFanqieRowSlashed);
        }

        if (label.Length > 0)
        {
            parts.Add(label);
        }

        if (LFanqieRowInitial.Length > 0)
        {
            parts.Add(LFanqieRowInitial);
        }

        string cell = LFanqieRowCell;
        if (cell.Length > 0)
        {
            parts.Add(cell);
        }
        else if (LFanqieRowText.Length > 0)
        {
            parts.Add(LFanqieRowText);
        }

        return this with { LFanqieRowLabel = label, LFanqieRowSummary = string.Join(' ', parts) };
    }
}
