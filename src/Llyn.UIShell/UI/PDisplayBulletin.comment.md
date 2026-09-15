# PDisplayBulletin.cs

## `public partial class PDisplay`

The reading view's answer to the engine's announcements.
It redraws only what the announcement touched, and leaves the rest of the page as drawn.

## `private void PDisplayBulletinHandle(LBulletin bulletin)`

What the view does when the engine announces that stored data changed.
It answers for the entry it stands on, and ignores an announcement about another Entry.
An announcement about any other record is answered whatever its id.
A card embeds Situations, Registers, Tags, Examples and Sources by reference.
A Situation renamed in its own tab must redraw the chip here.
A picture it shares relocated there must redraw the picture here.
Typing in any editor announces its draft, and that is the one subject left alone, since nothing stored changed.
A mark set from another tab moves the heart.
A frequency the engine finished fetching fills the chip alone, because nothing else on the page changed.
A script fetch that stored a character's pictures redraws the script box alone, whichever entry asked.
A fanqie fetch that stored a character's rows redraws the fanqie box alone, in the same way.
The shown entry may share the character.
A headword written elsewhere is read back and redrawn, so two views of one entry never disagree.
An entry that is gone leaves the unselected notice, because there is nothing left to show.
A workspace that moved empties the view, since the id it stood on means nothing in the new database.
