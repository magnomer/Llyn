using System;
using System.Collections.Generic;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LSourceFactoryHttp : LSourceFactory
{
    private readonly HttpClient _lSourceFactoryClient;

    public LSourceFactoryHttp(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _lSourceFactoryClient = client;
    }

    public IReadOnlyList<LSource> LSourceFactoryCreate(IReadOnlyList<LSourceSpec> specs)
    {
        ArgumentNullException.ThrowIfNull(specs);

        List<LSource> sources = new(specs.Count);
        foreach (LSourceSpec spec in specs)
        {
            sources.Add(new LSourceGeneric(spec, _lSourceFactoryClient));
        }

        return sources;
    }
}
