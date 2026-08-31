using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// The language-pack definition of one source: its name, the kind of value it yields, and the
/// ordered extraction attempts that make it work. This is pure data loaded from
/// <c>languages/&lt;Lang&gt;/source.json</c>; the generic source runner turns it into a live
/// <see cref="LSource"/>, so no source-specific code is needed for an ordinary source.
/// </summary>
/// <param name="LSourceSpecName">The source's display name, for example <c>"Cambridge"</c>.</param>
/// <param name="LSourceSpecKind">The value kind: <c>"pronunciation"</c> or <c>"audio"</c>.</param>
/// <param name="LSourceSpecAttempts">The extraction attempts, tried in order until one yields a value.</param>
public sealed record LSourceSpec(
    string LSourceSpecName,
    string LSourceSpecKind,
    IReadOnlyList<LSourceAttempt> LSourceSpecAttempts);
