using System;
using System.IO;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LLeaseFile : LLease
{
    private const string LLeaseFileName = "workspace.lease";

    private readonly string _lLeaseFilePath;
    private FileStream? _lLeaseFileStream;

    public LLeaseFile(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lLeaseFilePath = Path.Combine(root, LLeaseFileName);
    }

    public void LLeaseOpen()
    {
        if (_lLeaseFileStream is not null)
        {
            return;
        }

        try
        {
            _lLeaseFileStream = new FileStream(
                _lLeaseFilePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                1,
                FileOptions.DeleteOnClose);
        }
        catch (IOException exception) when ((exception.HResult & 0xFFFF) is 32 or 33)
        {
            throw new LLeaseFault(exception);
        }
    }

    public void LLeaseClose()
    {
        _lLeaseFileStream?.Dispose();
        _lLeaseFileStream = null;
    }
}
