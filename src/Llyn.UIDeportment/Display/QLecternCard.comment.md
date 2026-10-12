# QLecternCard.cs
Hash: `e28c8917b89e8475`

## `public sealed class QLecternCard`

The reading-card driver consumes only card, route and sound areas, keeping engine records outside presentation.
Card clicks reach Conduct gates rather than changing displayed collections.
Word offers leave through `QLecternMentionNotice`, so the driver needs no window reference.
A "see" click leaves through `QLecternSeeNotice`, so the driver needs no contents reference.

## `public QLecternCard(FrameworkElement surface, CDisplayCard area, CDisplayRoute route, CDisplaySound sound)`

Contract lookups bind both card lists and their sections to one surface.
Bubbling click and mention handlers cover cards generated inside templates.
The surface resources carry shared example typography.

## `internal event Action<PMention, CMentionOffer?>? QLecternMentionNotice;`

A word offer travels with its originating mention control, preserving the menu's anchor.

## `internal event Action<long>? QLecternSeeNotice;`

A "see" line's collocation id, raised after its row is asked open.
The lectern hands it to the contents spotlight, which owns scrolling the page to a card's home.

## `public void QLecternExampleRefine()`

Example typography enters surface resources before card templates render on display opening.

## `public void QLecternGlossRefine()`

Gloss typography follows the same resource-based presentation boundary.

## `public void QLecternCardRefine()`

Ready cards determine both lists and section visibility.
The owning lectern invokes this on opening, closing and fold notifications.

## `private void QLecternLeafRefine(CLecternCard card)`

Clearing each item source before replacement forces templates to redraw even when list length is unchanged.
Meaning peeks use meaning text, while collocation rows carry their phrase.
The collocation list holds only `CLecternCardOther`, the collocations with no link to a meaning of the entry.
Linked ones come nested inside their home meaning's item.

## `private void QLecternChipObserve(object sender, RoutedEventArgs e)`

Ready chip origin or translation identity reaches the route gate.
The event is handled only when that gate answers true.

## `private void QLecternHingeObserve(object sender, RoutedEventArgs e)`

Only a `ToggleButton` carrying a reading card can request a fold.
The reading card templates hold no other toggle with that data context.
The gate verdict goes to `QLook.QLookCheckedRefine`, which puts a refused hinge back.
An accepted fold redraws later through the display's fold notice.

## `private void QLecternSeeObserve(object sender, RoutedEventArgs e)`

Only a button carrying a `QLeafSee` line asks to open a row.
The line's collocation id reaches the fold gate as an unfold, since no stored fold means an open row.
The event is handled only when that gate answers true, and an accepted unfold redraws through the fold notice.
The notice is raised either way, so a refused save still takes the reader to the row's home.

## `private void QLecternMentionObserve(object? sender, PMentionArgument e)`

Sentence identity and raw click values reach one route gate unchanged.
Its offer is raised with the original mention control.
