# LQuillPronunciation.cs
Hash: `705a829d2b1a3d36`

## `public sealed class LQuillPronunciation`

The readings of the held entry: its primary pronunciation, its accent rows and their flags.
Its reads ask the engine's pack lookups, and each edit builds exactly one request.
The pack decides whether a typed reading is a respelling, so every driver writes the same request.

## `private readonly LTenure _lQuillPronunciationTenure;`

The tenure every request is built for and handed to, and whose engine answers the pack lookups.

## `public LQuillPronunciation(LTenure tenure)`

Builds the quill over one tenure, which it never swaps.

## `public bool LQuillFlaggedCheck()`

Whether the draft's language shows its varieties as flags.
An ended tenure or an empty language answers false.

## `public LGlyphBlock? LQuillGlyphRead()`

The held draft's transcription rows split by its pack's glyph section, as the editor lists them.
An ended tenure holds no draft, so it answers nothing.

## `public string LQuillPronunciationRead()`

The primary reading as the field shows it, respelled when the settings ask and the draft's pack respells.
An ended tenure or a draft without a reading answers empty.

## `public LAccentSheet? LQuillAccentRead()`

The live draft's accent sheet with every row after the primary, notated or not.
It is null while no draft is held.
The language clerk resolves each row's printed form.

## `public async Task<LAccentSheet?> LQuillAccentLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Loads the flags the live draft's accent sheet draws into `store`, then answers a fresh sheet.
A pack that labels varieties by name answers null at once and builds no sheet.
Nothing loads while no draft is held, and the answer is null.
The sheet is read again after the load.
A language changed meanwhile answers null instead of the fresh sheet.

## `public void LQuillPronunciationSet(string text)`

Defers the typed reading as a respelling or as a phonetic one, whichever the pack shows.
The choice is the pack's, so every driver's field writes the same request.

## `public void LQuillAccentSet(long accent, string text)`

Defers the text typed on the accent row `accent` as a respelling or a phonetic reading.
The pack's respelling rule picks the form, as for the primary field, so the row's printed form matches.

## `public void LQuillPronunciationAdd(long pronunciation)`

Adds a blank pronunciation after the row `pronunciation`, or after the primary for id zero.
The reading clerk places it, so every driver adds at the same place.

## `public void LQuillPronunciationRemove(long pronunciation)`

Drops the row `pronunciation`, or the primary for id zero.

## `public Uri? LQuillAudioResolve(long pronunciation)`

The address of the row's recording, or null when the draft holds no such row.
A file gone from disk also answers null and clears the row's audio, so its play button goes.

## `public void LQuillIpaSet(string text)`

Defers the typed phonetic reading of the primary pronunciation.

## `public void LQuillRespellingSet(string text)`

Defers the typed respelling of the primary pronunciation.

## `public void LQuillVarietySet(bool primary, long pronunciation, string variety)`

Names the variety a stored reading was heard in, at once.
The primary mark targets whatever primary the draft now holds, read after its reading was written.
The clerk mints the primary row on its first write.
So the id is read here rather than when the menu opened.
Any other call targets the row it names.
A missing reading or a blank variety changes nothing.
