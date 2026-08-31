using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// One extraction attempt within a source, declared by a language pack. A source tries its attempts
/// in order and keeps the first non-empty value. An attempt fetches each of its URLs in turn
/// (resilient reader with transient retry) and applies a single extraction strategy to the response.
/// </summary>
/// <param name="LSourceAttemptUrls">URL templates to try in order; each <c>{word}</c> placeholder is
/// replaced with the URL-escaped headword.</param>
/// <param name="LSourceAttemptStrategy">How to extract the value: <c>"regex"</c> (capture group on
/// the raw body), <c>"json"</c> (follow <see cref="LSourceAttemptPath"/>, then optionally a regex),
/// or <c>"span"</c> (the tolerant nested-span IPA reader).</param>
/// <param name="LSourceAttemptPattern">The regex applied to the body (regex/span) or to the JSON
/// value (json). <c>null</c> when the strategy needs none.</param>
/// <param name="LSourceAttemptGroup">The regex capture group to read; <c>0</c> for the whole match.</param>
/// <param name="LSourceAttemptPath">Dot-separated JSON path for the <c>"json"</c> strategy; each
/// segment is a literal property name. <c>null</c> for other strategies.</param>
/// <param name="LSourceAttemptGuard">Optional regex (with <c>{word}</c>) that must match the body
/// for the attempt to count; guards against pages served for an unrelated word.</param>
/// <param name="LSourceAttemptPhonetic">When <c>true</c>, the value is a phonetic form: decode
/// entities and strip enclosing IPA delimiters. Left <c>false</c> for audio URLs.</param>
/// <param name="LSourceAttemptHeaders">Extra request headers sent with every URL of this attempt,
/// or <c>null</c> for none. Some sources reject requests without them (for example Naver needs a
/// <c>Referer</c>). Kept in the pack as data so a new source's headers need no code.</param>
/// <param name="LSourceAttemptPrefix">Base URL prepended to a relative extracted value to make it
/// absolute (for example Cambridge audio <c>src</c> is site-relative), or <c>null</c> when the
/// value is already absolute.</param>
public sealed record LSourceAttempt(
    IReadOnlyList<string> LSourceAttemptUrls,
    string LSourceAttemptStrategy,
    string? LSourceAttemptPattern,
    int LSourceAttemptGroup,
    string? LSourceAttemptPath,
    string? LSourceAttemptGuard,
    bool LSourceAttemptPhonetic,
    IReadOnlyDictionary<string, string>? LSourceAttemptHeaders,
    string? LSourceAttemptPrefix);
