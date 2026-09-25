using System;
using System.Globalization;
using System.IO;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LAuditWriter : LAuditVault
{
    private const string LAuditWriterFile = "audit.log";

    private readonly string _lAuditWriterRoot;

    public LAuditWriter(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lAuditWriterRoot = root;
    }

    public static string LAuditWriterRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        return Path.Combine(root, LAuditWriterFile);
    }

    public string? LAuditRecord(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return LAuditWriterRecord(_lAuditWriterRoot, exception.ToString());
    }

    public static string? LAuditWriterRecord(string root, string note)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(note);

        string path = LAuditWriterRead(root);
        try
        {
            Directory.CreateDirectory(root);
            File.AppendAllText(
                path,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:yyyy-MM-dd HH:mm:ss}{1}{2}{1}{1}",
                    DateTime.UtcNow,
                    Environment.NewLine,
                    note));
            return path;
        }
        catch (Exception written) when (written is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
