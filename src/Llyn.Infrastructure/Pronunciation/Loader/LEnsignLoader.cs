using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Infrastructure;

internal sealed class LEnsignLoader
{
    private const string LEnsignSuffix = ".svg";
    private const string LEnsignFolder = "flags";
    private const string LEnsignAddress = "https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/";

    private readonly string _lEnsignRoot;

    private readonly HttpClient _lEnsignClient;

    public LEnsignLoader(string root, HttpClient client)
    {
        _lEnsignRoot = root;
        _lEnsignClient = client;
    }

    public async Task<string?> LEnsignFileRead(string code, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        string key = LEnsignCodeNormalize(code);
        string directory = Path.Combine(_lEnsignRoot, LEnsignFolder);
        string path = Path.Combine(directory, key + LEnsignSuffix);

        try
        {
            Directory.CreateDirectory(directory);
            if (File.Exists(path))
            {
                return path;
            }

            byte[] svg = await _lEnsignClient
                .GetByteArrayAsync(LEnsignAddress + key + LEnsignSuffix, cancellation)
                .ConfigureAwait(false);
            await LWorkspaceRoot.LWorkspaceFileSave(path, svg, cancellation).ConfigureAwait(false);
            return path;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException
            or IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    public string? LEnsignFileFind(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        string path = Path.Combine(_lEnsignRoot, LEnsignFolder, LEnsignCodeNormalize(code) + LEnsignSuffix);
        return File.Exists(path) ? path : null;
    }

    private static string LEnsignCodeNormalize(string code)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            code = code.Replace(invalid, '_');
        }

        return code.Trim().ToLowerInvariant();
    }
}
