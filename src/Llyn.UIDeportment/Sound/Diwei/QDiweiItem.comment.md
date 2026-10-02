# QDiweiItem.cs
Hash: `1e5db46e6a6657f1`

## `internal sealed class QDiweiItem`

One section of a category page as the template binds it, copied from the Conduct section.
Its label, lines, tally lines and switch state are all decided below, so nothing is computed here.

## `internal static void QDiweiItemRefine(FrameworkElement container, object item, string? _)`

Fills a section of `Theme.Diwei.Section`: heading, IPA and respelling switch, tally lines and placement lines.
The switch shows only while respelling is on, and each half wears its chosen style while its set shows.
The tally and placement lists are attached to their own fills.

## `private static void QDiweiChoiceRefine(FrameworkElement container, string name, Style sheet)`

Gives one half of the switch the theme style its caller pulled by contract ID.
The chosen styles `Theme.Diwei.IpaChosen` and `Theme.Diwei.RespellingChosen` carry the accent ink.

## `internal static IReadOnlyList<QDiweiItem> QDiweiItemBuild(IReadOnlyList<CDiweiSection> sections)`

Wraps each section of the page, in the page's order.
