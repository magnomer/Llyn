using System;
using System.Collections.Generic;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSourceFactory
{
    public static IReadOnlyList<LSource> LSourceFactoryCreate(LLanguage language, HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(client);

        List<LSource> sources = new(language.LLanguageSources.Count);
        foreach (LSourceSpec spec in language.LLanguageSources)
        {
            sources.Add(new LSourceGeneric(spec, client));
        }

        return sources;
    }
}
