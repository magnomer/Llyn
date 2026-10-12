# LPortraitRole.cs
Hash: `9c2b6cdd51509e4a`

## `public enum LPortraitRole`

What one section of a page likeness is, so a writer draws it by role and never by shape.
A writer that guessed from depth, position and line count drew a situation and a tag the same way.
The role names the display shape the section came from, and every writer keeps one style per role.

- `LPortraitRoleBand` - a headed group at the top of the page: readings, forms, meanings, other collocations, the note.
- `LPortraitRoleCard` - a numbered card headed by its own title.
- `LPortraitRoleKind` - a numbered meaning card with no title, headed by its kind word.
- `LPortraitRolePhrase` - the expression of a card, one line drawn as a phrase.
- `LPortraitRoleScene` - situation chips.
- `LPortraitRoleTone` - register chips.
- `LPortraitRoleBridge` - linked entries, such as translations and mentions.
- `LPortraitRoleQuote` - one example sentence with its frame, glosses, source and mentions.
- `LPortraitRoleLabel` - tag chips, and the language chip of an example.
- `LPortraitRoleUsage` - one entry that links here: its headword and language as the link, the referring card as the line.
- `LPortraitRoleList` - the collocations homed under a meaning, headed by their count, holding one row each.
- `LPortraitRoleRow` - one collocation row: the phrase as heading, the meaning as line, then every filled field.
