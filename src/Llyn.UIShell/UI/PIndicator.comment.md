# PIndicator.cs

## Inline notes

### `internal static class PIndicator`

Builds the scrollbar templates in code-owned XAML. WPF requires the track to be named "PART_Track"; keeping that framework contract here follows the same boundary as PField's "PART_ContentHost" and prevents framework names from entering the audited XAML naming surface.
