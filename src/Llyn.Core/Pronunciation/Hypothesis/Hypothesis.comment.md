# Hypothesis

This folder holds the user's reconstruction of historical sound and the anchors tying readings to rime placements.
It also holds the pronunciation lookup records.
It stands apart from `Fanqie`, which stores the placements, and from `Anatomy`, which holds the pack record.
Lookup logic lives behind the seam here, never in the UI.

## `LHypothesis`

The reconstruction a language pack names under `hypothesis`, saying how a rime-book placement sounds.
It is the user's own system of initials, finals and tone changes, held as tables in a pack file.
The engine knows nothing of Middle Chinese, only that an initial and a final are looked up and joined.
A pack without the file carries null, and the fanqie box prints the placement alone.
Initials are keyed by the initial's character, such as 疑 to `ng`.
Finals are keyed by rime and division, such as `模 一` to `o`.
A rounded placement first tries the key with `合` appended, such as `寒 一 合` to `wan`.
A rime ending in a capital letter, the 重紐 mark, is tried as written and then without the letter.
Tones are keyed by the tone's character, each holding rows of `LHypothesisTone`.
Places are the articulatory places listed under `place`, in pack order, empty when none is listed.

## `LHypothesisResolve`

The sound of one placement.
The onset and final are joined, then the tone's first matching row is applied.
The row's rewrites run over the joined syllable, and its class becomes the sound's class.
A tone outside the table, or with no row taking the onset, keeps the joined syllable with a blank class.
The answer is null when the initial or the final is not in the tables.
The box then falls back to the placement text.

## `LHypothesisInitialFind`

The onset alone, without final or tone, for a rime category page's reading column.
Null when the placement names no initial or the table lacks it.

## `LHypothesisFinalFind`

The final alone, without initial or tone, for a category page's reading column.
It looks the final up, rounded key first when the placement is rounded, then the plain key.
The rime is tried as written and then without its 重紐 letter.

## `LHypothesisLocusFind`

The first place listing the initial, or null when none does.

## `LHypothesisRankRead`

The position of an initial across every place in pack order.
A rime page sorts its rows by it, so they follow the pack.
It is -1 for an initial no place lists.

## `LHypothesisKeyFormat`

The finals key is the rime, a space and the division, then a space and `合` when rounded.
A placement without a division keys on the rime alone.

## `LHypothesisLocus`

One articulatory place of the user's reconstruction, such as the labials, and the initials it gathers.
The places section a rime's category page, in the order the pack lists them.
The initials of one place are listed in the order the pack prints them, and that order sorts the rows.
The name, such as `labial`, is printed through a localization key.

## `LHypothesisSound`

What the hypothesis gives for one placement.
It is the reconstructed syllable with its tone rewrites applied, such as `ngoʔ`.
Its class is the tone class key such as `4S`, or empty when the tone had no matching row.

## `LHypothesisTone`

One class of a rime-book tone in the user's reconstruction, such as 4S under 上.
The rows of a tone are tried in order, and the first whose onset pattern matches wins.
The onset is a .NET regex matched against the onset the initial table gave, empty to match every onset.
The rules rewrite the whole syllable in order, onset and final joined.
The class key is printed by the reading view after the reading, such as `1` or `4S`.
The pattern is compiled once when the record is built, so a broken pattern fails while the pack loads.
The compiled form is a private field, so it never enters the record's equality.

## `LHypothesisToneMatch`

Whether the row takes this onset.
It is true for an empty pattern, else the regex's own answer.

## `LHypothesisToneResolve`

Runs every rewrite over the syllable in order and returns the result.

## `Equals` in `LHypothesisTone`

Two rows are equal when their onset, class and rule sequence match.
The synthesized record equality would compare the compiled regex by reference and call twins unequal.

## `LAnchor`

The anchors of a reflex row are the ids of the fanqie rows.
The user tied that reading to them by hand.
Nothing derives an anchor, so a reading counts on a Diwei page only through the placements the user chose.
A row without anchors counts nowhere.
The helpers keep the list sorted and free of repeats, so two lists compare by position.

## `LAnchorNormalize`

The anchors sorted ascending, each once, ids below one dropped, and an empty list for null.

## `LAnchorToggle`

The list with the id added when anchored and removed otherwise, normalized.

## `LAnchorMatch`

Whether two normalized lists hold the same ids in the same order.
The records carrying anchors compare by reference on the list, so the match is done here.

## `LAnchorRowScan`

One mark per stored fanqie row, telling whether the row is anchored and whether its tone class is estimated.
A row the archive has not kept yet has no id and is left out.

## `LAnchorRow`

One stored fanqie row of a character, marked for the anchor dropdown of one reflex row.
Held is true when the reflex row anchors this fanqie row.
Estimated is true when the fanqie row carries a tone class the estimate reads.

## `LAnchorCheck`

Whether a reflex row may carry anchors at all.
Only a single-glyph headword with fanqie rows has placements to tie a reading to.

## `LAnchorTextFormat`

The readings of the anchored stored rows, each once, joined with the separator.
An empty text when `LAnchorCheck` refuses the rows and the headword.
A spoken row gives its slashed reading, and any other row gives its summary.

## `LSeeker`

The pronunciation lookup contract.
A requester starts a lookup and subscribes to its results through an `LReceiver`.
All lookup logic lives behind this seam, never in the UI.

## `LSeekerStart`

Starts a lookup for a word, streaming results to the receiver.
The returned task completes when every source has finished or the lookup is cancelled.
It carries the whole set of candidates that were streamed, ordered by their sources.
A caller that wants to keep the answer therefore need not collect the stream a second time.
A cancelled lookup returns nothing, because the task ends by throwing instead.

## `LCandidate`

What one lookup source had to say about a headword.
Every source produces at least one, so the menu shows a row for each and none is silently absent.
A source that returned several varieties produces one per variety, each carrying its tag.
A source that failed is as much a result as one that answered, and the user is told which happened.
The source is the name its language pack declares, such as `Cambridge`.
Source identity is a config-driven name, never a compile-time enum, so the lookup stays language-agnostic.
The phonetic form is bare, without brackets, or null when the source gave none.
The order is the source's position in the pack's `pronunciation` list.
Sources answer at whatever speed the network gives them, so arrival order is meaningless to the reader.
The position travels with the candidate, so the menu can show the order the pack declares.
With no phonetic form, the reached flag separates two cases.
The dictionary does not carry the word, or the dictionary is down.
The variety matches one the pack declares.
It is empty when the source gave none, so the menu shows the row untagged.
The respelling is the form recast through the pack's respelling groups, or null when none ran.
The original form stays in the phonetic field, so the menu can show either and the pick stores both.

## `LAnswer`

What one source gave back for a word.
It separates a source that answered with nothing from one that never answered at all.
A menu can then say "No entry" where it means that, and "Failed to retrieve" where a host was unreachable.
Returning a bare list could not tell the two apart.
A dead source read as a word no dictionary carries.
The same record serves the reader below it, where the value is a page body rather than a transcription.
A pronunciation source may return several readings, one per variety the page lists.
An audio source or a page reader returns one untagged reading holding an address or a body.
Reached false means every URL failed, by a timeout, a refused connection, or a 5xx that outlived its retries.
Reached true with no readings means the source answered and simply had nothing for this word.

## Factories of `LAnswer`

The lost answer means the source was never reached.
The blank answer means the source answered and had nothing.
The string factory makes one untagged reading holding the value.
Audio harvest and the page reader take that form, since neither carries a variety.

## `LAnswerValue`

The first reading's form, or null when there is none.
It serves the callers that need a single string, such as audio harvest.

## `LAnswerEmpty`

Whether there is no reading to take, whatever the source's fate was.

## `LReading`

One transcription a source returned for a word, tagged with the regional variety it belongs to.
A page that lists a British and an American form yields two readings, not one joined string.
The tag travels with the form from the source to the candidate the menu shows.
The variety matches one the language pack declares, and is empty when the source does not say.
The phonetic form is bare, without enclosing brackets.

## `LReadingNormalize`

The built-in, language-agnostic cleanup every reading passes through before it becomes a candidate.
It runs whatever the respelling switch says, so the trove caches cleaned text.
It NFC-normalizes, drops every syllable boundary and every whitespace character, and trims.
A boundary is the full stop, the middle dot Cambridge writes, or the hyphenation point.
Stress marks, length marks, tie bars, and delimiters survive.
So `ˈdɪf.ər.əns` and `ˈdɪf ər əns` both become `ˈdɪfərəns`.
A multi-word headword loses its inner space by design.
Site quirks this step cannot know are left to the pack's `cleanup` groups.

## `LReadingScan`

Expands a source's readings so nothing untagged reaches the menu in a language that declares varieties.
Both the lookup and the harvest call it, so a transcription and a recording fan out the same way.
A pack without varieties returns the readings untouched, so Spanish keeps its untagged rows.
Otherwise every untagged reading becomes one reading per declared variety, in pack order, all carrying the same text.
A variety the same answer already tags is skipped, so Cambridge's British, American, and untagged rows never double up.
Tagged readings pass through unchanged, even one naming a variety the pack does not declare.
That mismatch is the pack author's to fix, and the UI label makes it visible.
Tagged readings come first in answer order, then the fan-out rows, so a page's own tags always lead.
An empty answer stays empty here and still yields the single no-value row.
It is static and takes no source, so a test can drive it directly.

## Stamp

Commit: `c2f335c331fe`
- `LAnchor.cs` `7054ca273c51f91d`
- `LAnchorRow.cs` `4665863180f042a8`
- `LAnswer.cs` `271b9a9f3a96cc59`
- `LCandidate.cs` `839e308b66a8f73c`
- `LHypothesis.cs` `920118ac81cd5b51`
- `LHypothesisLocus.cs` `ea88078b2a91068d`
- `LHypothesisSound.cs` `46d96739a8bb4e71`
- `LHypothesisTone.cs` `d0dbcdd3cbf6fd01`
- `LReading.cs` `d6aea69d0a952cf3`
- `LSeeker.cs` `2fe65bdc22766bf7`
