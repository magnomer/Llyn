# CDisplay.cs

## `public sealed class CDisplay`

The reading view's area: the lectern's gates, its reads and the change events its drivers answer.
The display builds one over itself, so every driver over that display hears the same events.
The rules it shares with the editor's esteem stay on [LDisplay](LDisplay.comment.md), which it calls.
It holds no state beyond the shown header, since the shown draft stays in the sound half.

## `private static readonly CLectern _cDisplayBlank`

The header while nothing is shown, every field empty.

## `private static readonly CLecternAccent _cDisplayMute`

The pronunciation block while nothing is shown or the pack refused, with no reading, row or flag.

## `internal CDisplay(LDisplay display, LEntryPort entries, LPhonologyPort phonology, CEnvoy envoy)`

Only the display builds its area, over the ports and the envoy the panel handed down.
A refused read is shown through `envoy`, so every driver over this display shows it once.

## `public event Action? CDisplayOpened;`

Raised on the calling thread once an entry opens and `CDisplayShown` holds its header.
The panel and the wing open entries on the UI thread, so a driver answers it directly.

## `public event Action? CDisplayClosed;`

Raised once the shown entry closed and playback stopped, so every section empties.

## `public event Action<CBulletin>? CDisplayFavoriteChanged;`

Raised on the engine's thread when the chosen entry's favourite changed.
The favourite, grasp, frequency, paradigm and reflex events count only for the chosen entry.
A driver marshals each onto its own thread before it reads.

## `public event Action<CBulletin>? CDisplayEntryChanged;`

Raised when the shown draft may have changed, so a driver hands it to `CDisplayEntryResonate`.
It counts the chosen entry's own bulletins, and any example, situation, reference, author, tag, register or settings bulletin.
Each of those can change what the draft shows.

## `public event Action<CBulletin>? CDisplayWorkspaceChanged;`

Raised when the workspace was swapped, so a driver hands it to `CDisplayWorkspaceResonate`.

## `public CLectern CDisplayShown { get; private set; }`

The header of the shown entry, or the blank header while nothing is shown.

## `public int CDisplayGraspStep => _cDisplayRule.LDisplayGraspStep;`

The last grasp step, which the star control takes as its limit so it names no engine constant.

## `internal void LDisplayVistaAttach()`

Subscribes the plan of which subjects refresh which part on the display's new vista.
Script, fanqie and workspace bulletins count for any entry, since their sections redraw whole.
The subject and bulletin maps are the display's, which reuse `CPanel.CPanelSubjectRead` and `CAtelier.CAtelierBulletinRead`.

## `public void CDisplayPanelAttach(CPanel panel)`

Follows a panel: each draft it loads opens here, and its clearing closes the view.
The view's deportment names the panel, since a view may follow a panel or none.

## `internal void LDisplayEntryOpen(LEntryDraft? draft)`

Opens a draft in the view, or closes the view for a missing draft.
A vista choosing nothing closes the view too, since the draft then stands on no entry.
The stamp is read from the stored entry, and a refused read hides it.

## `public void CDisplayEntryClose()`

Closes the shown entry, which drops the shown draft and stops this view's play.
A driver calls it when it starts a view afresh, and a followed panel's clearing calls it too.

## `public void CDisplayEntryResonate()`

Reloads the chosen entry's draft on the driver's thread and opens it again.
Nothing chosen reloads nothing, and a refused load leaves the view as it stands.
An entry gone from the store closes the view.

## `public void CDisplayWorkspaceResonate()`

Closes the view on the driver's thread once the workspace was swapped.

## `public bool CDisplayFavoriteToggle(bool marked)`

The gate for the heart: stores `marked` on the chosen entry, then answers the stored value.
A refused mark thus answers the old value, and the display reports the failure.

## `public CGrasp CDisplayGraspRead()`

The chosen entry's grasp step with its wording.

## `public string CDisplayGraspRead(int step)`

The wording of any step, for the step under the pointer, or empty while nothing is chosen.

## `public CGrasp CDisplayGraspSet(int step)`

The gate for a star the reader pressed on the chosen entry, answering the stored stars.
A press on the standing step comes back cleared, since the display's one rule owns that.

## `public CFrequency? CDisplayFrequencyRead(Func<string, string> lookup)`

The chosen entry's frequency chip, or null when none stands.
Conduct chooses the key of the one-off wording, and the driver's lookup turns it into text.

## `public void CDisplayPlaybackCancel()`

The gate for a view going away: it stops this view's own play.
A sound another view started plays on.

## `public static bool CDisplayNarrativeCheck(bool editable, string text)`

Whether the read face of the narrative stands: only on the read side, and only with words in it.
Whether text holds words is the engine's rule, so no trim happens here.

## `public static bool CDisplayEtymonCheck(bool editable, int count)`

Whether the row of source links shows: always while editable, else only with a link.

## `private LEntryDraft? LDisplayShown`

The shown draft the sound half holds, passed unread to the engine.

## `public CLecternAccent CDisplayAccentRead()`

The shown entry's pronunciation block, ready to draw.
The engine answers the readings, the brackets and the pack's verdicts in one read.
Conduct only chooses each variety's label key through `CSounding.CSoundingVarietyRead`.
Nothing shown answers the mute block, and a refused read shows `Sound.LoadFailed` and answers it too.

## `public async Task<CLecternAccent?> CDisplayEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

The load a driver starts once an entry opened, for the flags its pronunciation block draws.
The engine picks the varieties and loads nothing for a pack that names them.
`store` is the driver's own image store, handed the rows through the one flag map.
It answers the block again once the flags are in, so the driver repaints its rows.
Another entry shown meanwhile wins, so a late load answers null and paints nothing.
A refused load answers null too, since a missing flag falls back to its name.

## `private static CLecternAccent LDisplayAccentRead(LAccentSheet sheet)`

Maps the engine's block into the Conduct record, reading no rule.

## `public CLecternCard CDisplayCardRead()`

What the card templates read beside the shown entry's cards.
The order, the bylines and the link names are three reads, each answering empty when refused.
The page thus still draws, as the sentence frame's read does in the editor.

## `private CSentenceOrder LDisplayOrderRead(string language)`

The sentence order of `language`, or the default order when the read is refused.

## `private IReadOnlyDictionary<long, string> LDisplayCitationRead()`

The byline of every Source, or none when the read is refused.
It is read on every open, so a Source edited elsewhere reads fresh.

## `private IReadOnlyList<CTranslationTarget> LDisplayTargetRead(LEntryDraft shown)`

The headwords the shown draft's cards and etymology link to, or none when the read is refused.

## `public IReadOnlyList<CUsage> CDisplayIncomingRead()`

The usages pointing at the chosen entry, each with the epithet the settings ask for.
The engine names each usage and its epithet in one read.
No entry chosen answers no usages.
A refused read shows `Display.IncomingFailed` and answers no usages.
Each usage maps through `COeuvre.COeuvreUsageRead`, the one map the vita's citations share.

## `public CLecternEtymology CDisplayEtymologyRead()`

The shown entry's etymology, with whether its field and its section show.
The engine names the links and says whether a narrative or a link stands.
A refused link read answers no links, and the field then shows by its narrative alone.

## `public bool CDisplayChipOpen(object? chip, long? link, Func<long, bool> entrySeam, Func<long, bool> situationSeam, Func<long, bool> registerSeam, Func<long, bool> tagSeam)`

The gate for a chip clicked on a card or a source link clicked in the etymology.
`chip` is what the clicked chip carries, and `link` the entry id a link chip names.
The engine says which stored record the click names, and a record never saved names none.
The record's kind then picks the panel that opens it, through the driver's seam for that panel.
It answers whether a panel was asked, so the driver marks the click handled.

## `public CMentionResult? CDisplayMentionFind(string text, string language, int offset, IReadOnlyList<CMentionMark>? mentions)`

The gate for a word clicked in a sentence of the reading view.
The engine reads what stands at `offset`, in the shown entry's language when the text names none.
A refused lookup shows `Mention.FindFailed` and answers null, so the click opens nothing.

## `public (CCompassPart, int)? CDisplayCardFind(long id)`

Which card list of the shown entry holds the card `id`, and at which place.
The engine finds the card, and its list maps by name to the compass part.
It answers null when nothing is shown or the entry has no such card.
