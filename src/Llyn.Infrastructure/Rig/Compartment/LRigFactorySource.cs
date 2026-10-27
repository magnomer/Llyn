using System;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LRigFactorySource
{
    private static readonly TimeSpan LRigSourcePatience = TimeSpan.FromSeconds(2);

    public static LRigSource LRigSourceBuild(HttpClient client, string root, LClock clock) =>
        new(
            new LSourceFactoryHttp(client),
            new LFanqieSourceHttp(client, LRigSourcePatience),
            new LShengfuSourceHttp(client, LRigSourcePatience),
            new LReflexSourceHttp(client, LRigSourcePatience),
            new LScriptSourceHttp(client, LRigSourcePatience),
            new LRecordingArchive(root, client, clock));
}
