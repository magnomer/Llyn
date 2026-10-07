# TLookPart.cs
Hash: `acb778ddff8b2e79`

## `public sealed class TLookPart`

Covers how a driver finds a named part inside an item's template.
The list lives on its own STA thread, since a WPF control demands one.

## `public void LookPartFind_TemplateSelector_FindsThePartOfTheChosenTemplate()`

A chip list picks its templates through a selector, so the presenter's own template stays empty.
The part is still found in the template the selector chose.
Without that, every edited chip showed an empty frame with no text and no eraser.
