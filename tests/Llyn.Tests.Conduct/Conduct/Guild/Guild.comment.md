# Guild

This folder holds the gates for the authors panel and for the byline that offers Authors.
They cover roll, autograph, mode, union, oeuvre, byline dropdown, scribe and vista.

## `TByline`

Covers the byline the source editor opens under a typed credit.
The credit row it opens a blank row under hands its place and stored id.
Typing in a field without the keyboard opens nothing, and typing with it offers the Authors not yet credited.
Each offered name comes split around the word, case-blind, keeping the name's own case.
The word loses its outer spaces, and a blank word closes the byline again.
At most eight Authors are offered.
Down and Up move the lit row and wrap at either end.
Escape closes the byline and drops the lit row.
Enter on a lit row credits its Author and marks the draft changed.
Enter with no lit row credits the typed name, and closes the byline.
Escape over an offered byline only closes it, and a second Escape reverts the field.
Picking the Author the field already credits only reverts the typed text, and a press on no row reverts nothing.
A find that fails shows `Source.FindFailed` once and offers nothing.
A search over a draft the workspace change made stale shows `Source.FindFailed` once and offers nothing.
The two notice tests build the imprint over a fake envoy that records each notice.

## `TGuild`

Covers how the authors panel lists, chooses and shows Authors, end to end on a real workspace.
The roll leads with the uncredited row, and a query narrows it.
A roll that drops the chosen Author closes the panel.
No order keeps the ordering, and a hidden kind marks the roll filtered.
With nothing chosen the vita is nobody's, and a chosen Author shows its vita, fellows and Sources.
An Author click records the station it leaves before the Author opens.
A Source click shows the colophon.
A confirmed delete removes the Author, a workspace notice empties the panel, and an Author notice refreshes the roll.
A print on the author side prints nothing.
The autograph desk lives in `TGuildScribe`, and the union of two Authors in `TGuildUnion`.

## `TGuildPrepare`

Builds the panel over the atelier, and the panel restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each answer at once.
Every test here that builds a guild goes through it, except where a test needs its own marshal.

## `TGuildAutograph`

Covers the autograph's Conduct side of the authors panel on a real workspace.
A typed name on a fresh Author is stored and chosen, and with no tenure held it changes nothing.
Opening a stored Author to read clears nothing, and writing it tells the driver once to clear the union.
A draft notice runs through the marshal the panel is built with and shows the held name once.
The draft is persisted before the notice, so the deferred name raises no second notice.
Only the first test sets the edit delay to zero.

## `TGuildMode`

Covers how the authors panel carries its side and mode across clicks, on a real workspace.
It builds each panel through `TGuild.TGuildPrepare`.
A click on no row changes nothing on either list.
A click while writing keeps the autograph over a stored Author and drops it for the orphan row.
A stored leave answer saves the draft before the clicked Author opens.
The mode toggle and the delete do nothing while a Source is in front.
Turning back to reading drops the held draft and shows the vita.

## `TGuildRoll`

Covers the guild's one roll answer on a real workspace.
The rows, the empty verdict and the vita arrive together, so the view reads once.
A query matching nothing answers empty.
A chosen Author gone from the roll closes the panel before the vita is read, so the vita is nobody's.
Each row carries its icon key and its citation count, and each fellow its shared count.

## `TGuildScribe`

Covers the authors panel's autograph desk, how it opens and how it is left, on a real workspace.
It builds each panel through `TGuild.TGuildPrepare`.
A click asks once for unsaved work, and a kept answer stays put and records no station.
A jump from another panel opens the Author without asking.
Writing with no stored Author closes the panel.
A restored scribe mode opens the autograph only over a stored Author.
A blank name is refused through the envoy, and a named one is stored and chosen.
Only the save test sets the edit delay to zero.

## `TGuildUnion`

Covers the union that folds the written Author into a kept one, on a real workspace.
It builds each panel through `TGuild.TGuildPrepare`.
The union lists other Authors and folds the written one into the kept one on a yes.
A no keeps both Authors.
The union question shows the written and kept names trimmed, as the engine reads them.

## `TGuildVista`

Covers the authors panel's vista restore and its observers on a real workspace.
A second restore carries the held roll query and the held oeuvre query into the fresh vistas.
After a second restore an Author notice raises the roll once, through the marshal, from the fresh vista only.
A create alone restores once, so the same notice after only a create raises once too.
A query change on the oeuvre vista raises the oeuvre rows once through the marshal.
The menu offers four orderings.

## `TOeuvre`

Covers the oeuvre's gates and reads end to end on a real workspace.
The rows are the Sources crediting the chosen Author, and nothing before the vistas are restored.
A chosen Source that leaves the list closes the oeuvre panel once.
The empty text is plain with no Author, vacant with one, and unmatched while narrowed.
The comb query narrows the Sources, and it carries over when fresh vistas are restored.
The tally and the colophon word the usage of the chosen Source through the engine.
Opening a Source on the oeuvre panel raises its colophon.
An author row words its work count through the engine.
Most tests build the oeuvre over a fresh roll vista and oeuvre vista, as the guild restores them.

## Stamp

Commit: `2f9a06b8eb95`
- `TByline.cs` `56eade5948ec66cb`
- `TGuild.cs` `0a14cdf002b0278e`
- `TGuildAutograph.cs` `b6c5e2ce42a02875`
- `TGuildMode.cs` `8274056122aa7caa`
- `TGuildRoll.cs` `3dc687227a7b24e2`
- `TGuildScribe.cs` `2e89b5a9fc71ec97`
- `TGuildUnion.cs` `0fd0484c73c1f0d4`
- `TGuildVista.cs` `5a6b95948aa2c1fa`
- `TOeuvre.cs` `b542dc705657e7e8`
