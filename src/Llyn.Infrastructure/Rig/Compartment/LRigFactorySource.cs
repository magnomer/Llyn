using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactorySource
{
    public static LRigSource LRigSourceBuild(HttpClient client, string root) =>
        new(
            new LSourceFactoryHttp(client),
            new LFanqieSourceHttp(client),
            new LShengfuSourceHttp(client),
            new LReflexSourceHttp(client),
            new LScriptSourceHttp(client),
            new LRecordingArchive(root, client));
}
