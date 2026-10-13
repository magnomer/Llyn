# Sound

This folder creates the tables for how words sound and are written in other scripts.
Some tables hang off an Entry, such as pronunciation, transcription, reflex and frequency.
Others hang off no entry, because a character is shared by every entry written with it.
Those are fanqie, script, shengfu, stem and diwei.
The Register is independent too, and cards reach it through links.

## `LSchemaPronunciation`

Creates the pronunciations an Entry owns, their syllables and their recordings.
An entry carries an ordered list keyed by its own id and placed by (entry_parent, position).
The first row is the primary one every summary shows.
Its id only counts upward and is never given to a later pronunciation once the row is gone.
A revision names a deleted pronunciation by that id.
A reused number would point the history at a living row.

A row may carry no ipa yet, because a recording is often fetched before the reading is typed.
The respelling is the ipa recast in the pack's convention.
It is kept beside the ipa so the switch only picks which is shown.
It is NULL when the pack has no respelling groups, and NULL throughout in a file from before the column.
The variety says why the row stands beside the others, such as a region.
It is NULL when there is one row.

A pronunciation owns ordered syllables keyed by (pronunciation_parent, position).
A syllable requires only its nucleus.
Its columns are the segments and the tone, never a spelling.
Every other syllable field is optional and stores NULL when absent, which differs from empty.

The pronunciation also owns at most one downloaded recording.
pronunciation_audio has no id, since pronunciation_parent is its primary key.
Its file is stored relative to the workspace folder, so a moved or copied workspace keeps its audio.
Children cascade with their pronunciation, and the pronunciation cascades with its entry.

## `LSchemaTranscription`

Creates the transcriptions an Entry owns.
A transcription is the reading spelled in a named scheme, such as Jyutping, and is never IPA.
IPA lives in pronunciation, which says how the word sounds.
Rows are keyed by their own id, placed by (entry_parent, position), and their id is never reused.
One scheme spells one reading one way, so (entry_parent, scheme) is unique.
The scheme name is stored on the row rather than in a lookup table.
So a row stays readable after its language pack drops or renames the scheme.
A row always carries text, because a blank transcription is no transcription.
The rows cascade when their entry is deleted.

## `LSchemaReflex`

The reflex table holds the readings of an entry in the languages that borrowed its characters.
Each row belongs to one entry through entry_parent and goes with it when the entry is deleted.
The language, kind, romanization, meaning and note are plain text.
main is one when the reading is the one in common use.
owned is one when the user supplied the meaning, and a rebuild must keep it.
respelling is the reading recast in its own language's convention, empty when that language has no groups.
region is the place the reading is taken from.
The eight onset_ipa to tone_respelling columns hold the reading cut into parts, in IPA and in respelling.
They are derived, never typed, and an older database arrives with all eight blank.
The workspace update then cuts every row again from its text under the entry's pack rules.

## `LSchemaReflexNoted`

The first schema version whose reflex rows keep the note apart from the kind.

## `LSchemaReflexParted`

The first schema version whose reflex rows keep romanization, meaning, ownership and note as separate columns.

## `LSchemaReflexSettle`

Drops every reflex row carried from a workspace older than the note.
Such rows were fetched with the pinyin or 훈 folded into the kind or the text.
No rule can unfold them.
The rows are a fetched cache, and an entry without rows is fetched again the next time it is shown.
A workspace before version 72 instead carries its old note into romanization and its old remark into note.

## `LReflexPartApply`

Parts the old note and remark across the new romanization and note columns.
The remark is read from the attached old schema when that column exists.

## `LSchemaFrequency`

Creates the frequency store, one row per source that answered for one entry.
The key is entry and source, so a refetch replaces an entry's rows cleanly.
The raw figure is the ground truth exactly as the source returned it.
The band is the label the pack mapped the figure to at fetch time, so no view resolves it again.
The rows go with their entry when it is deleted.

## `LSchemaFrequencySettle`

Carries the old entry.frequency column of an older workspace into the frequency table during migration.
The old column held source|raw, and the settle splits it on the first bar.
The band is left empty, and the engine resolves and stores it the first time the entry is read.
A workspace whose entry table no longer has the column is left alone.

## `LSchemaFanqie`

Creates the fanqie store, one row per placement of one character in one rime book.
The key is language, character, book, source and position, so a refetch replaces a character's set cleanly.
Two sources of one book each count positions from zero, so the source is part of the key.
The source names the site the row came from, since one book may be read from several.
The text is the line the reading view prints as it stands.
The parts it was built from sit beside it, so a hypothesis can derive a reading without a refetch.
The spelling is the 反切 the site printed, or empty.
The reading and tone class are what the hypothesis derived from the parts, written when the placement is stored.
Both are empty when the hypothesis gave none, and the view prints them as they stand.

## `LSchemaDiwei`

Creates the diwei store of 音韻地位 categories and the links from fanqie rows to them.
A category is keyed by language, kind and key, so 來 as an initial is one row for every character.
A link belongs to a fanqie row and points to a category, one link per part the row carries.
A refetch deletes the character's fanqie rows, and the links go with them by cascade.
A category deleted takes its links too, so a rebuild can start from an empty language.

## `LSchemaScript`

Creates the script store, one row per glyph picture of one character in one style.
The key is language, character, style and position, so a refetch replaces a character's set cleanly.
The picture bytes sit in the row as the original the source drew.
The caption is stored without the chronology the source printed in it, and epoch holds that age's code.
The age is matched once, when the picture is fetched, so nothing is matched or fetched again to print it.

## `LSchemaScriptNoted`

The schema version from which every stored caption already holds its age apart, in epoch.
The settle dates only workspaces stored below it, so no caption is cut twice.

## `LSchemaScriptSettle`

Dates every picture carried from a workspace older than the note whose caption still holds its age.
Such rows were fetched before their age had a code or a label of its own.
Each row is cut by the pack labels of the style it names, under the labels the pack lists today.
Only a row that was cut is written.
No picture is fetched again, because the caption a row already holds is all the cutting needs.
A caption no label matches is left exactly as it was.
A later pack cuts it once it lists that age.

## `LSchemaShengfu`

Creates the phonetic-series store, one row per character of one language.
The key is language and character, because a character has one series per language.
The text is the series as the reading view prints it, with several already joined.
The source names the site the series came from, since one language may be read from several.

## `LSchemaStem`

Creates the tables the phonetic series are browsed by.
A series row is shared by every character the same series names.
A series is unique per language and key, so one key never stands twice in a language.
The shengfu_stem link cascades from both sides, so a dropped character or series leaves no link behind.

## `LSchemaRegister`

Creates the independent Register and the two link tables a Meaning and a Collocation reach it by.
Both links carry a position, so a card holds its registers in order.
The link cascades with the card and never with the Register.
So a Register survives every detach, and the store refuses to delete one anything still points at.

A Register is identified by its name, which is unique.
A pack names the Registers it offers and never numbers them.
Two packs naming the same name name one Register, so a pack cannot claim a Register for its language.
The pack is re-read on every load of its language.
The unique name turns the second read into an upsert of the built-in flag.
It also decides how a merge treats a Register from another workspace.
Such a row is never inserted when the name already exists here.
The import must find the local row by name and point every link at it instead.

## `LSchemaRegisterShift`

The distance every position is moved by while a referrer's marks are renumbered.
Moving them all out of range first keeps two marks from ever meeting on one position.

## `LSchemaRegisterSettle`

Repoints every mark left aimed at a Register the rebuild did not carry across.
An older workspace held one row per language for one name, and the unique name keeps only the first.
A mark on a dropped row is aimed at the surviving row of the same name.
That row is read from the old file.
A card already marked with that row keeps one mark rather than two.
Nothing is lost that the orphan sweep would otherwise have deleted.

## `LSchemaLinkSettle`

Settles one link table, then renumbers every referrer's marks so they run from zero without a gap.
Replacing a doubled mark leaves a hole, and the position index expects none.

## Stamp

Commit: `c2f335c331fe`
- `LSchemaDiwei.cs` `44e5cb3ba99ac99d`
- `LSchemaFanqie.cs` `326325df32301690`
- `LSchemaFrequency.cs` `c9e709dafa8682f3`
- `LSchemaPronunciation.cs` `13f11cf6092ef27a`
- `LSchemaReflex.cs` `a8f17fdac3c368fb`
- `LSchemaRegister.cs` `09b151ad6ef63828`
- `LSchemaScript.cs` `30f069237e5995fd`
- `LSchemaShengfu.cs` `4625f65aa2d80cae`
- `LSchemaStem.cs` `d2f6c201670400ac`
- `LSchemaTranscription.cs` `48785491e41f1877`
