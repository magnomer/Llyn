# LSourceReader.cs

## `internal static class LSourceReader`

Resilient page reader for HTML pronunciation sources.
Tries each candidate URL in turn and, per URL, retries transient failures (timeout, 408, 429, 5xx) with a short backoff.
Non-transient responses (403, 404) fall through to the next URL rather than being retried.
Returns the first successful body, or `null` when every URL is exhausted.

## Inline notes

### `request.Headers.TryAddWithoutValidation(header.Key, header.Value);`

Some are restricted headers (Referer).
Skip validation so they are sent verbatim.

### `retryable = true;`

A per-request timeout (HttpClient.Timeout), not a user cancellation: retry.

### `return status == HttpStatusCode.RequestTimeout`

408 429 5xx
