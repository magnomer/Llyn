# CDisplay.cs
Hash: `945003299cfa7515`

## `public sealed class CDisplay`

The reading view's header and card area: the lectern's gates, its reads and the change events its drivers answer.
The display builds one over itself, so every driver over that display hears the same events.
The sound half's gates and reads stand on [CDisplaySound](CDisplaySound.comment.md), split by role.
They read the header here.
The rules it shares with the editor's esteem stay on [LDisplay](LDisplay.comment.md), which it calls.
It holds no state beyond the shown header and the mention area.

## `private static readonly CLectern _cDisplayBlank`

The header while nothing is shown, every field empty.

## `private readonly LDisplay _cDisplayRule;`

The display rules this area calls and shares with the editor.

## `private readonly LEntryPort _cDisplayPort;`

The entry port the reads and the stamp go through.

## `private readonly LPhonologyPort _cDisplayPhonology;`

The phonology port the sentence order is read from.

## `private readonly CEnvoy _cDisplayEnvoy;`

The envoy a refused read is shown through.

## `private readonly LSettingsPort _cDisplaySettings;`

The settings port the ready notice and the unknown mark are read from.

## `private CMention? _cDisplayMention;`

The atelier's mention area, null until the navigation is attached.

## `internal CDisplay(LDisplay display, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, CEnvoy envoy)`

Only the display builds its area, over the ports and the envoy the panel handed down.
A refused read is shown through `envoy`, so every driver over this display shows it once.
`settings` reads the ready notice the envoy shows with it.

## `public event Action? CDisplayOpened;`

Raised on the calling thread once an entry opens and `CDisplayShown` holds its header.
The panel and the wing open entries on the UI thread, so a driver answers it directly.

## `internal event Action<string, long>? CDisplayRowChosen;`

Raised with the tab and the record a clicked chip names.
The display hands it to the atelier's navigation, which opens the record there.

## `public event Action? CDisplayClosed;`

Raised once the shown entry closed and playback stopped, so every section empties.

## `public event Action<CBulletin>? CDisplayFavoriteChanged;`

Raised on the engine's thread when the chosen entry's favourite changed.
The favourite, grasp, frequency, paradigm and reflex events count only for the chosen entry.
A driver marshals each onto its own thread before it reads.

## `public event Action<CBulletin>? CDisplayGraspChanged;`

Raised when the chosen entry's grasp step changed.

## `public event Action<CBulletin>? CDisplayFrequencyChanged;`

Raised when the chosen entry's frequency changed.

## `public event Action<CBulletin>? CDisplayParadigmChanged;`

Raised when the chosen entry's inflection changed.

## `public event Action<CBulletin>? CDisplayReflexChanged;`

Raised when the chosen entry's reflexes changed.

## `public event Action<CBulletin>? CDisplayScriptChanged;`

Raised for any entry's script bulletin.

## `public event Action<CBulletin>? CDisplayFanqieChanged;`

Raised for any entry's fanqie bulletin.

## `public event Action<CBulletin>? CDisplayEntryChanged;`

Raised when the shown draft may have changed, so a driver hands it to `CDisplayEntryResonate`.
It counts the chosen entry's own bulletins, and any example, situation, reference, author, tag, register or settings bulletin.
Each of those can change what the draft shows.

## `public event Action<CBulletin>? CDisplayWorkspaceChanged;`

Raised when the workspace was swapped, so a driver hands it to `CDisplayWorkspaceResonate`.

## `public CLectern CDisplayShown { get; private set; }`

The header of the shown entry, or the blank header while nothing is shown.

## `public int CDisplayGraspStep`

The last grasp step, which the star control takes as its limit so it names no engine constant.

## `private long? LDisplayChosen`

The id of the entry the rules say is chosen, or null.

## `internal void LDisplayVistaAttach()`

Subscribes the plan of which subjects refresh which part on the display's new vista.
Script, fanqie and workspace bulletins count for any entry, since their sections redraw whole.

## `public void CDisplayPanelAttach(CPanel panel)`

Follows a panel.
Each draft it loads opens here, and its clearing closes the view.
The view's deportment names the panel, since a view may follow a panel or none.

## `private void LDisplayDraftOpen(LDraft draft)`

Opens the content of a draft a followed panel loaded.

## `internal void LDisplayEntryOpen(LEntryDraft? draft)`

Opens a draft in the view, or closes the view for a missing draft.
A vista choosing nothing closes the view too, since the draft then stands on no entry.
The note arrives parsed, so the driver draws its blocks and never parses.
The stamp is read from the stored entry, and a refused read hides it.

## `private (bool, string, string) LDisplayStampRead(long id)`

The stamp of the stored entry.
A refused read shows `Display.StampFailed` once until the user acts, and answers an empty stamp, so the entry still opens.

## `public void CDisplayEntryClose()`

Closes the shown entry, which drops the shown draft and stops this view's play.
A driver calls it when it starts a view afresh, and a followed panel's clearing calls it too.

## `public void CDisplayEntryResonate()`

Reloads the chosen entry's draft on the driver's thread and opens it again.
Nothing chosen reloads nothing, and a refused load leaves the view as it stands.
An entry gone from the store closes the view.

## `public void CDisplayWorkspaceResonate()`

Closes the view on the driver's thread once the workspace was swapped.

## `public bool CDisplayFavoriteRead()`

Whether the chosen entry is a favorite.

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

## `private LEntryDraft? LDisplayShown`

The shown draft the sound half holds, passed unread to the engine.

## `private CLedgerNoticed LDisplayNoticed`

The atelier's repaint memory, held by the display this area stands on.
Every read below that runs on a repaint shows its failure through it, so a lasting fault shows once.
The favorite and grasp gates and the mention lookup answer a user act, so they show every failure.

## `public CLecternCard CDisplayCardRead()`

The shown entry's cards, each ready to paint, with whether their sections show.
The order, the Source lines and the link targets are three reads, each answering a fallback when refused.
Each refusal also shows its own notice, so the page still draws.
The unknown mark is the engine's word, so a frame or a sentence embeds it ready.

## `private IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LDisplayTranslationRead(LEntryDraft shown)`

The link targets of every card of the shown entry, keyed by card id.
A refused read shows `Display.TranslationFailed` once until the user acts, and answers no targets, so the cards still draw.

## `private LSentenceOrder LDisplayOrderRead(string language)`

The sentence order of `language`.
A refused read shows `Display.OrderFailed` once until the user acts, and answers the default order, so the cards still draw.

## `private IReadOnlyDictionary<long, string> LDisplayCitationRead(LEntryDraft shown)`

The ready line of every Source the shown entry cites.
A refused read shows `Display.CitationFailed` once until the user acts, and answers no lines, so the cards still draw.
It is read on every card read, so a Source edited elsewhere reads fresh.

## `public IReadOnlyList<CUsage> CDisplayIncomingRead()`

The usages pointing at the chosen entry, each with the epithet the settings ask for.
The engine names each usage and its epithet in one read.
No entry chosen answers no usages.
A refused read shows `Display.IncomingFailed` once until the user acts, and answers no usages.
Each usage maps through `COeuvre.COeuvreUsageRead`, the one map the vita's citations share.

## `public CLecternEtymology CDisplayEtymologyRead()`

The shown entry's etymology, with whether its field, its read narrative, its links and its section show.
The engine answers one record that names the links and says whether a narrative or a link stands.
Whether the narrative holds words is the etymology draft's rule, so no trim happens here.
A refused read shows `Display.EtymologyFailed` once until the user acts.
It answers no links, and the field then shows by its narrative alone.

## `public bool CDisplayChipOpen(CLeafChip? chip, long? link)`

The gate for a chip clicked on a card or a source link clicked in the etymology.
`chip` is the clicked situation, register or tag chip, and `link` the entry id a link chip names.
A stored chip opens its record in the tab its kind picks.
Any other chip falls to the link, which opens the library tab on its entry.
The engine says which entry a link names, and an empty id names none, so nothing opens.
It answers whether a tab was asked, so the driver marks the click handled.

## `private static string LDisplayTabRead(CSubject subject)`

The tab that opens a chip's record, picked by the chip's kind, by name.

## `internal void LDisplayMentionAttach(CMention mention)`

Holds the atelier's mention area, which opens what a clicked word found.
The composition calls it through `LDisplayNavigationAttach`, so a display built alone finds nothing.

## `public CMentionOffer? CDisplayMentionFind(long sentence, int offset)`

The gate for a word clicked in an example line of the reading view.
The driver names only the line's sentence row and the offset clicked.
The engine reads the row's text, language and Mentions from the shown entry itself.
So no sentence text or Mention travels up through the driver and back.

## `public CMentionOffer? CDisplayEtymologyFind(int offset)`

The gate for a word clicked in the etymology prose of the reading view.
The engine reads the prose and its language from the shown entry, so the driver sends only the offset.

## `private CMentionOffer? LDisplayMentionOpen(Func<LEntryDraft, LMentionResult> find)`

The open shared by both find gates, given the engine find over the shown entry.
It answers null when no mention area is held or nothing is shown.
`LMentionResultOpen` then opens a stored Mention or a sole entry at once.
The answer is the found word's start and the entries the menu offers, empty when one opened.
The reading view asks no leave question of its own, since the navigation asks it on opening.
A refused lookup shows `Mention.FindFailed` and answers null, so the click opens nothing.
The open sits inside the same catch.

## `public (CCompassPart, int)? CDisplayCardFind(long id)`

Which card list of the shown entry holds the card `id`, and at which place.
The engine finds the card, and its list maps by name to the compass part.
It answers null when nothing is shown or the entry has no such card.
