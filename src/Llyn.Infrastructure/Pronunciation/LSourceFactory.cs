using System;
using System.Collections.Generic;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Builds the live <see cref="LSource"/> set for a language pack. Every ordinary source becomes a
/// data-driven <see cref="LSourceGeneric"/>; this is the seam where a future source needing logic a
/// language pack cannot express would be wired to a hand-written handler instead. The orchestrators
/// receive the finished set and stay language-agnostic.
/// </summary>
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
