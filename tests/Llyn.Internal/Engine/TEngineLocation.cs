using System;
using System.IO;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineLocation
{
    [Theory]
    [InlineData("\\\\host\\share\\x.png")]
    [InlineData("//host/share/x.png")]
    [InlineData("file://host/share/x.png")]
    [InlineData("\\\\?\\C:\\x.png")]
    [InlineData("\\\\.\\PhysicalDrive0")]
    [InlineData("ftp://host/x.png")]
    [InlineData("\\x.png")]
    public void LocationResolve_HostOrDevicePath_ReturnsNull(string location)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineLocationResolve(location));
    }

    [Fact]
    public void LocationResolve_RelativeInsideWorkspace_ReturnsWorkspaceFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Uri? resolved = engine.TEngineLocationResolve("media\\x.png");

        Assert.NotNull(resolved);
        Assert.True(resolved.IsFile);
        Assert.Equal(Path.Combine(workspace.TWorkspaceFolder, "media", "x.png"), resolved.LocalPath);
    }

    [Fact]
    public void LocationResolve_RelativeLeavingWorkspace_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineLocationResolve("..\\x.png"));
        Assert.Null(engine.TEngineLocationResolve("media\\..\\..\\x.png"));
    }

    [Fact]
    public void LocationRead_MissingFile_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineLocationRead("media\\x.png"));
    }

    [Fact]
    public void LocationRead_PresentFile_ReturnsWorkspaceFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string path = Path.Combine(workspace.TWorkspaceFolder, "media", "x.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, [0]);

        Uri? read = engine.TEngineLocationRead("media\\x.png");

        Assert.Equal(path, Assert.IsType<Uri>(read).LocalPath);
    }

    [Fact]
    public void LocationRead_WebAddress_ReturnsAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("https", Assert.IsType<Uri>(engine.TEngineLocationRead("https://example.test/x.png")).Scheme);
    }

    [Fact]
    public void LocationResolve_DriveAndWebAddress_ReturnsAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Uri? drive = engine.TEngineLocationResolve("C:\\media\\x.png");
        Uri? local = engine.TEngineLocationResolve("file:///C:/media/x.png");
        Uri? web = engine.TEngineLocationResolve("https://example.test/x.png");

        Assert.Equal("C:\\media\\x.png", Assert.IsType<Uri>(drive).LocalPath);
        Assert.Equal("C:\\media\\x.png", Assert.IsType<Uri>(local).LocalPath);
        Assert.Equal("https", Assert.IsType<Uri>(web).Scheme);
    }
}
