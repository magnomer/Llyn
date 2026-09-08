# LSourceReader.cs

## `internal static class LSourceReader`

Resilient page reader for HTML pronunciation sources.
Tries each candidate URL in turn and, per URL, retries transient failures (timeout, 408, 429, 5xx) with a short backoff.
Non-transient responses (403, 404) fall through to the next URL rather than being retried.
It returns an `LAnswer`.
A caller can tell a page that answered with nothing from a host that never answered.
A 404 is an answer: the site is up and has no such word.
An exhausted 5xx or timeout is not, and that is what a menu reports as a failure to retrieve.

## `public static Task<LAnswer> LSourceReaderRead(HttpClient client, IReadOnlyList<string> urls, IReadOnlyDictionary<string, string>? headers, CancellationToken cancellation)`

The first URL with a body wins.
When none has one, the answer is blank if any URL answered at all, and lost otherwise.
So one unreachable mirror does not condemn a source whose other mirror simply had no entry.

## Inline notes

### `request.Headers.TryAddWithoutValidation(header.Key, header.Value);`

Some are restricted headers (Referer).
Skip validation so they are sent verbatim.

### `catch (OperationCanceledException)`

A per-request timeout (HttpClient.Timeout), not a user cancellation: retry.

### `return status == HttpStatusCode.RequestTimeout`

408 429 5xx
