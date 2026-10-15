using System.Globalization;
using Llyn.Core;

namespace Llyn.Tests;

public sealed class TDepotFake : LDepot
{
    private int _tDepotFakeSaves;

    public Dictionary<string, LDeckSheet> TDepotFakeFiles { get; } = new(StringComparer.Ordinal);

    public Dictionary<string, string> TDepotFakeDigest { get; } = new(StringComparer.Ordinal);

    public HashSet<(string TDepotFakeCode, string TDepotFakeName)> TDepotFakeMedia { get; } = [];

    public Dictionary<string, string> TDepotFakeNames { get; } = new(StringComparer.Ordinal);

    public List<string> TDepotFakeCopied { get; } = [];

    public List<string> TDepotFakeDeleted { get; } = [];

    public Task LDepotSave(string folder, string code, LDeckSheet sheet, bool basic, CancellationToken cancellation)
    {
        TDepotFakeFiles[code] = sheet;
        _tDepotFakeSaves++;
        TDepotFakeDigest[code] = "written-" + _tDepotFakeSaves.ToString(CultureInfo.InvariantCulture);
        return Task.CompletedTask;
    }

    public string? LDepotDigestRead(string folder, string code) => TDepotFakeDigest.GetValueOrDefault(code);

    public string? LDepotMediaResolve(string source) => TDepotFakeNames.GetValueOrDefault(source);

    public bool LDepotMediaCheck(string folder, string code, string name) => TDepotFakeMedia.Contains((code, name));

    public void LDepotMediaCopy(string source, string folder, string code, string name)
    {
        TDepotFakeMedia.Add((code, name));
        TDepotFakeCopied.Add(name);
    }

    public void LDepotMediaDelete(string folder, string code, string name)
    {
        TDepotFakeMedia.Remove((code, name));
        TDepotFakeDeleted.Add(name);
    }
}
