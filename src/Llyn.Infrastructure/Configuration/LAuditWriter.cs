using System;
using System.Globalization;
using System.IO;

namespace Llyn.Infrastructure;

public static class LAuditWriter
{
    private const string LAuditWriterFile = "audit.log";

    public static string LAuditWriterRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        return Path.Combine(root, LAuditWriterFile);
    }

    public static string? LAuditWriterRecord(string root, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return LAuditWriterRecord(root, exception.ToString());
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
