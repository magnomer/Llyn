using System;
using System.Collections.Generic;
using System.IO;
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
}
