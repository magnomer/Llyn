using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The shell engine: the single boundary the UI shell talks to. The UI sends a request here and
/// subscribes through an <see cref="LReceiver"/>; all lookup logic (source fan-out,
/// fetching, parsing) lives behind this engine, so none of it sits in the UI shell.
///
/// This class is also the composition root for the lookup feature: it owns the shared
/// <see cref="HttpClient"/> and wires the concrete sources to the orchestrator. It was chosen as
/// the request/subscribe boundary because it is the UI-facing engine layer — the UI shell then
/// depends only on <c>Llyn.ShellEngine</c> and <c>Llyn.Core</c>, never on Application or
/// Infrastructure directly.
/// </summary>
public sealed class LEngine : IDisposable
{
    private readonly HttpClient _lEngineClient;
    private readonly LSeeker _lEngineLookup;

    public LEngine()
    {
        _lEngineClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        // Cambridge (and some Wiktionary edge caches) reject requests without a browser-like agent.
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");

        _lEngineLookup = new LLookup(
        [
            new LSourceWikipedia(_lEngineClient),
            new LSourceCambridge(_lEngineClient)
        ]);
    }

    /// <summary>
    /// Starts a pronunciation lookup for <paramref name="word"/> and streams results to
    /// <paramref name="receiver"/>. The returned task completes when every source has finished.
    /// </summary>
    public Task LEnginePronunciationFind(string word, LReceiver receiver, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        return _lEngineLookup.LSeekerStart(word, receiver, cancellation);
    }

    public void Dispose()
    {
        _lEngineClient.Dispose();
    }
}
