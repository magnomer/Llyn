using System;
using System.Collections.Generic;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSourceFactory
{
    public static IReadOnlyList<LSource> LSourceFactoryCreate(
        IReadOnlyList<LSourceSpec> specs, HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(specs);
        ArgumentNullException.ThrowIfNull(client);

        List<LSource> sources = new(specs.Count);
        foreach (LSourceSpec spec in specs)
        {
            sources.Add(new LSourceGeneric(spec, client));
        }

        return sources;
    }
}
