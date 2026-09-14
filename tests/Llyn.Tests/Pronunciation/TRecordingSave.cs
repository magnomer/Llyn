using System.Net;
using System.Net.Http;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TRecordingSave
{
    [Fact]
    public async Task RecordingSave_TaggedRecording_NamesFileWithVariety()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/tomato__gb_2.mp3", 0, true, "British");

        string path = await TInterface.TWorkspaceRecordingSave(
            recording, "tomato", "English", workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.Equal(Path.Combine(workspace.TWorkspaceFolder, "audio", "English"), Path.GetDirectoryName(path));
        Assert.Matches("^tomato\\.British\\.[0-9a-f]{8}\\.mp3$", Path.GetFileName(path));
        Assert.True(File.Exists(path));
        Assert.Empty(Directory.GetFiles(Path.GetDirectoryName(path)!, "*.tmp"));
    }

    [Fact]
    public async Task RecordingSave_HeadwordsDifferingByCase_NamesFilesApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording upper = TInterface.TRecordingCreate("Duden", "https://example.test/Weg.mp3", 0, true, string.Empty);
        LRecording lower = TInterface.TRecordingCreate("Duden", "https://example.test/weg.mp3", 0, true, string.Empty);

        string first = await TInterface.TWorkspaceRecordingSave(
            upper, "Weg", "German", workspace.TWorkspaceFolder, client, CancellationToken.None);
        string second = await TInterface.TWorkspaceRecordingSave(
            lower, "weg", "German", workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.False(string.Equals(first, second, StringComparison.OrdinalIgnoreCase));
        Assert.True(File.Exists(first));
        Assert.True(File.Exists(second));
    }

    [Fact]
    public async Task RecordingSave_DeviceNameHeadword_PrefixesTheFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/con.mp3", 0, true, string.Empty);

        string path = await TInterface.TWorkspaceRecordingSave(
            recording, "con", "English", workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.StartsWith("_con.", Path.GetFileName(path), StringComparison.Ordinal);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task RecordingSave_UnknownExtension_FallsBackToMp3()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/play.php?id=1", 0, true, string.Empty);

        string path = await TInterface.TWorkspaceRecordingSave(
            recording, "tomato", "English", workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.EndsWith(".mp3", path, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RecordingSave_UntaggedRecording_NamesFileByWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording recording = TInterface.TRecordingCreate(
            "Naver", "https://example.test/009123.mp3?token=1", 0, true, string.Empty);

        string path = await TInterface.TWorkspaceRecordingSave(
            recording, "tomato", "English", workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.Matches("^tomato\\.[0-9a-f]{8}\\.mp3$", Path.GetFileName(path));
    }

    [Fact]
    public async Task RecordingPrepare_SameAddressTwice_ReusesCachedFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording recording = TInterface.TRecordingCreate(
            "Naver", "https://example.test/us/002731.mp3?token=1", 0, true, "American");

        string first = await TInterface.TWorkspaceRecordingPrepare(
            recording, workspace.TWorkspaceFolder, client, CancellationToken.None);
        using FileStream held = new(first, FileMode.Open, FileAccess.Read, FileShare.None);
        string second = await TInterface.TWorkspaceRecordingPrepare(
            recording, workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.Equal(first, second);
        Assert.Equal(Path.Combine(workspace.TWorkspaceFolder, "temp"), Path.GetDirectoryName(first));
        Assert.EndsWith(".mp3", first, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RecordingPrepare_SameLeafDifferentHosts_NamesFilesApart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using HttpClient client = TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK);
        LRecording one = TInterface.TRecordingCreate("Oxford", "https://one.test/tomato.mp3", 0, true, string.Empty);
        LRecording other = TInterface.TRecordingCreate("Naver", "https://other.test/tomato.mp3", 1, true, string.Empty);

        string first = await TInterface.TWorkspaceRecordingPrepare(
            one, workspace.TWorkspaceFolder, client, CancellationToken.None);
        string second = await TInterface.TWorkspaceRecordingPrepare(
            other, workspace.TWorkspaceFolder, client, CancellationToken.None);

        Assert.NotEqual(first, second);
    }
}
