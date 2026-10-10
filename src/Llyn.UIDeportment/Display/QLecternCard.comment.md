# QLecternCard.cs
Hash: `837d0f7554408f19`

## `public sealed class QLecternCard`

The reading-card driver consumes only card, route and sound areas, keeping engine records outside presentation.
Card clicks reach Conduct gates rather than changing displayed collections.
Word offers leave through `QLecternMentionNotice`, so the driver needs no window reference.

## `public QLecternCard(FrameworkElement surface, CDisplayCard area, CDisplayRoute route, CDisplaySound sound)`

Contract lookups bind both card lists and their sections to one surface.
Bubbling click and mention handlers cover cards generated inside templates.
The surface resources carry shared example typography.

## `internal event Action<PMention, CMentionOffer?>? QLecternMentionNotice;`

A word offer travels with its originating mention control, preserving the menu's anchor.

## `public void QLecternExampleRefine()`

Example typography enters surface resources before card templates render on display opening.

## `public void QLecternGlossRefine()`

Gloss typography follows the same resource-based presentation boundary.

## `public void QLecternCardRefine()`

Ready cards determine both lists and section visibility.
The owning lectern invokes this on opening, closing and fold notifications.

## `private void QLecternLeafRefine(CLecternCard card)`

Clearing each item source before replacement forces templates to redraw even when list length is unchanged.
Meaning peeks use meaning text, while collocation peeks use expression text.

## `private void QLecternChipObserve(object sender, RoutedEventArgs e)`

Ready chip origin or translation identity reaches the route gate.
The event is handled only when that gate answers true.

## `private void QLecternHingeObserve(object sender, RoutedEventArgs e)`

Only a `ToggleButton` carrying a reading card can request a fold.
The reading card templates hold no other toggle with that data context.
The gate verdict goes to `QLook.QLookCheckedRefine`, which puts a refused hinge back.
An accepted fold redraws later through the display's fold notice.

## `private void QLecternMentionObserve(object? sender, PMentionArgument e)`

Sentence identity and raw click values reach one route gate unchanged.
Its offer is raised with the original mention control.
