# PLangcodeIndicator.cs

## `internal static class PLangcodeIndicator`

Resolves a language pack's cached SVG flag into a frozen drawing.
Both the editor's language picker and the read-only entry display use it.
A malformed or unreadable flag becomes no image, which lets each surface show its neutral globe fallback.
