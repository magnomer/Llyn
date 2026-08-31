using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// A loaded language pack: the language's name and the source definitions declared for it. Loaded
/// from <c>languages/&lt;Lang&gt;/source.json</c>. The engine holds no language-specific facts of
/// its own; everything language-specific arrives through this record.
/// </summary>
/// <param name="LLanguageName">The language's name, matching its folder under <c>languages/</c>.</param>
/// <param name="LLanguageFlag">The pack's flag as an ISO 3166-1 alpha-2 country code (e.g. <c>gb</c>),
/// or <c>null</c> when the pack declares none. The image itself is not shipped: the engine downloads
/// the matching flag from the flag-icons set on demand and caches it in the workspace.</param>
/// <param name="LLanguageSources">The source definitions declared for the language.</param>
public sealed record LLanguage(
    string LLanguageName,
    string? LLanguageFlag,
    IReadOnlyList<LSourceSpec> LLanguageSources);
