# PDisplayBulletin.cs

## `public partial class PDisplay`

The reading view's answer to the engine's announcements.
It redraws only what the announcement touched, and leaves the rest of the page as drawn.

## `internal void PDisplayObserverAttach()`

The owner calls it once its deportment has restored the display's vista.
Attaches one observer per subject through the deportment to the vista it holds, so no handler sorts announcements.
The subjects that name an entry are attached to the chosen row, so another entry's announcement never arrives.
An announcement about any other record is answered whatever its id.
A card embeds Situations, Registers, Tags, Examples and Sources by reference.
A Situation renamed in its own tab must redraw the chip here.
A picture it shares relocated there must redraw the picture here.
A flipped setting re-reads the entry too, so every reading swaps to the form now picked.
Typing in any editor announces its draft, and that subject is not attached, since nothing stored changed.
A script or fanqie fetch is attached whatever the entry asked, since the shown entry may share the character.
A workspace that moved empties the view, since the id it stood on means nothing in the new database.

## `private void PDisplayFavoriteUpdate()`

A mark set from another tab moves the heart.

## `private void PDisplayGraspUpdate()`

A grasp set from another tab moves its mark.

## `private void PDisplayFrequencyUpdate()`

A frequency the engine finished fetching fills the chip alone, because nothing else on the page changed.

## `private void PDisplayParadigmUpdate()`

A paradigm the engine finished fetching fills the inflection table alone.

## `private void PDisplayReflexUpdate()`

A reflex fill that stored the entry's rows re-reads the entry and redraws the reflex lines alone.

## `private void PDisplayScriptUpdate()`

A script fetch that stored a character's pictures redraws the script box alone, whichever entry asked.

## `private void PDisplayFanqieUpdate()`

A fanqie fetch that stored a character's rows redraws the fanqie box alone, in the same way.

## `private void PDisplayEntryUpdate()`

A headword written elsewhere is read back and redrawn, so two views of one entry never disagree.
An entry that is gone leaves the unselected notice, because there is nothing left to show.
Nothing chosen means nothing to redraw.
