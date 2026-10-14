using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LDepotFolder : LDepot
{
    private const string LDepotFolderText = "Text 1";
    private const string LDepotFolderExtension = ".txt";
    private const string LDepotFolderMedia = " Media";
    private const string LDepotFolderPending = ".tmp";
    private const string LDepotFolderSetting = "*";
    private const string LDepotFolderBar = "|";
    private const int LDepotFolderDigest = 16;

    public async Task LDepotSave(
        string folder, string code, LDeckSheet sheet, bool basic, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        string path = LDepotFolderResolve(folder, code)
            ?? throw new ArgumentException("The deck code must name one file in the folder.", nameof(code));
        Directory.CreateDirectory(folder);
        byte[] content = new UTF8Encoding(false).GetBytes(LDepotFolderFormat(sheet, basic));
        await LWorkspaceRoot.LWorkspaceFileSave(path, content, cancellation).ConfigureAwait(false);
    }

    public LDeckSheet? LDepotRead(string folder, string code)
    {
        string? path = LDepotFolderResolve(folder, code);
        if (path is null || !File.Exists(path))
        {
            return null;
        }

        using StreamReader reader = new(path, Encoding.UTF8, true);
        return LDepotFolderParse(reader.ReadToEnd());
    }

    public string? LDepotDigestRead(string folder, string code)
    {
        string? path = LDepotFolderResolve(folder, code);
        if (path is null || !File.Exists(path))
        {
            return null;
        }

        return Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
    }

    public string? LDepotMediaResolve(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        if (!File.Exists(source))
        {
            return null;
        }

        using FileStream stream = File.OpenRead(source);
        string digest = Convert.ToHexStringLower(SHA256.HashData(stream));
        return digest[..LDepotFolderDigest] + Path.GetExtension(source).ToLowerInvariant();
    }

    public bool LDepotMediaCheck(string folder, string code, string name) =>
        File.Exists(LDepotPathResolve(folder, code, name));

    public void LDepotMediaCopy(string source, string folder, string code, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        string path = LDepotPathResolve(folder, code, name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string pending = path + LDepotFolderPending;
        File.Copy(source, pending, true);
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }

    public void LDepotMediaDelete(string folder, string code, string name)
    {
        string path = LDepotPathResolve(folder, code, name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static string LDepotFolderFormat(LDeckSheet sheet, bool basic)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        int lead = LDepotLeadFind(sheet.LDeckSheetHeader);
        StringBuilder builder = new();
        foreach ((string key, string value) in sheet.LDeckSheetSettings)
        {
            LDepotLineFormat(builder, [LDepotFolderSetting, key, value], basic);
        }

        LDepotLineFormat(builder, LDepotColumnMove(sheet.LDeckSheetHeader, lead), basic);
        foreach (IReadOnlyList<string> row in sheet.LDeckSheetRows)
        {
            LDepotLineFormat(builder, LDepotColumnMove(row, lead), basic);
        }

        return builder.ToString();
    }

    public static LDeckSheet LDepotFolderParse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<(string, string)> settings = [];
        List<string>? header = null;
        List<IReadOnlyList<string>> rows = [];
        int index = 0;
        while (index < text.Length)
        {
            List<string> cells = LDepotLineParse(text, ref index);
            if (cells.Count == 1 && cells[0].Length == 0)
            {
                continue;
            }

            if (header is null && cells[0] == LDepotFolderSetting)
            {
                settings.Add((
                    cells.ElementAtOrDefault(1) ?? string.Empty,
                    cells.ElementAtOrDefault(2) ?? string.Empty));
            }
            else if (header is null)
            {
                header = cells;
            }
            else
            {
                rows.Add(cells);
            }
        }

        return new LDeckSheet(settings, header ?? [], rows);
    }

    private static string? LDepotFolderResolve(string folder, string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentNullException.ThrowIfNull(code);

        return LDepotNameCheck(code) ? Path.Combine(folder, code + LDepotFolderExtension) : null;
    }

    private static string LDepotPathResolve(string folder, string code, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(name);

        if (!LDepotNameCheck(code) || !LDepotNameCheck(name))
        {
            throw new ArgumentException("The deck code and the media name must each name one file.", nameof(name));
        }

        return Path.Combine(folder, code + LDepotFolderMedia, name);
    }

    private static bool LDepotNameCheck(string name) =>
        name.Trim().Length > 0
        && name != "."
        && name != ".."
        && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;

    private static int LDepotLeadFind(IReadOnlyList<string> header)
    {
        for (int index = 0; index < header.Count; index++)
        {
            if (string.Equals(header[index], LDepotFolderText, StringComparison.Ordinal))
            {
                return index;
            }
        }

        throw new ArgumentException("The deck header must name the Text 1 column.", nameof(header));
    }

    private static List<string> LDepotColumnMove(IReadOnlyList<string> cells, int lead)
    {
        List<string> moved = [.. cells];
        if (lead > 0 && lead < moved.Count)
        {
            string text = moved[lead];
            moved.RemoveAt(lead);
            moved.Insert(0, text);
        }

        return moved;
    }

    private static void LDepotLineFormat(StringBuilder builder, IReadOnlyList<string> cells, bool basic)
    {
        for (int index = 0; index < cells.Count; index++)
        {
            if (index > 0)
            {
                builder.Append('\t');
            }

            builder.Append(LDepotCellFormat(cells[index], basic));
        }

        builder.Append('\n');
    }

    private static string LDepotCellFormat(string cell, bool basic)
    {
        if (basic)
        {
            return cell.Replace("\r\n", LDepotFolderBar, StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Replace("\n", LDepotFolderBar, StringComparison.Ordinal)
                .Replace('\t', ' ');
        }

        if (cell.IndexOfAny(['"', '\t', '\r', '\n']) < 0)
        {
            return cell;
        }

        return "\"" + cell.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }

    private static List<string> LDepotLineParse(string text, ref int index)
    {
        List<string> cells = [];
        StringBuilder cell = new();
        bool quoted = false;
        while (index < text.Length)
        {
            char current = text[index++];
            if (quoted)
            {
                if (current != '"')
                {
                    cell.Append(current);
                }
                else if (index < text.Length && text[index] == '"')
                {
                    cell.Append('"');
                    index++;
                }
                else
                {
                    quoted = false;
                }
            }
            else if (current == '"' && cell.Length == 0)
            {
                quoted = true;
            }
            else if (current == '\t')
            {
                cells.Add(cell.ToString());
                cell.Clear();
            }
            else if (current == '\n')
            {
                break;
            }
            else if (current != '\r')
            {
                cell.Append(current);
            }
        }

        cells.Add(cell.ToString());
        return cells;
    }
}
