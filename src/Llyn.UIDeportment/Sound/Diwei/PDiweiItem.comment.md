# PDiweiItem.cs

## `internal sealed class PDiweiItem`

One section of a category page as the template binds it, copied from the engine's section.
Its label, lines, tally lines and switch state are all decided below, so nothing is computed here.

## `internal static void PDiweiItemApply(FrameworkElement container, object item, string? _)`

Fills a section of `Theme.Diwei.Section`: heading, IPA and respelling switch, tally lines and placement lines.
The switch shows only while respelling is on, and lights the set the section shows.
The tally and placement lists are attached to their own fills.

## `private static void PDiweiChoiceApply(FrameworkElement container, string name, bool chosen)`

Lights one half of the switch in the accent while it is the chosen set.

## `internal static IReadOnlyList<PDiweiItem> PDiweiItemBuild(IReadOnlyList<LDiweiSection> sections)`

A plain copy loop over the composed sections.
