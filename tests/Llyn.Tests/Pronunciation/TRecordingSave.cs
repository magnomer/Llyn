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

        Assert.Equal(Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "tomato.British.mp3"), path);
        Assert.True(File.Exists(path));
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

        Assert.Equal(Path.Combine(workspace.TWorkspaceFolder, "audio", "English", "tomato.mp3"), path);
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
