# CDisplayCard.cs
Hash: `5c82cee2a988cacc`

## `public sealed class CDisplayCard`

The reading view's card reads and its card fold gate.
It reads the cards, the usages pointing at the entry, and the etymology.
It also folds or unfolds one card of the chosen entry.
Only [CDisplay](CDisplay.comment.md) builds one, over its own rules, and hands it out.
So every driver over one display reads the same shown draft.
It holds no state of its own.

## `private readonly LDisplay _cDisplayRule;`

The display rules the shown draft, the chosen entry, the media port and the repaint memory come from.

## `private readonly LCardPort _cDisplayCardPort;`

The card port the link targets, usages, etymology and folds are read through.
A fold is also saved and deleted through it.

## `private readonly LReferencePort _cDisplayReferencePort;`

The reference port the Source lines are read through.

## `private readonly LExamplePort _cDisplayExamplePort;`

The example port each sentence line is composed through.

## `private readonly LSentencePort _cDisplaySentence;`

The sentence port the sentence order is read from.

## `private readonly LSettingsPort _cDisplaySettings;`

The settings port the ready notice and the unknown mark are read from.

## `private readonly CEnvoy _cDisplayEnvoy;`

The envoy a refused read is shown through.

## `internal CDisplayCard(LDisplay display, LCardPort cards, LReferencePort references, LExamplePort examples, LSentencePort sentences, LSettingsPort settings, CEnvoy envoy)`

Only the display builds it, over its rules and the ports the atelier handed down.
It takes the rules whole, since the shown draft and the chosen entry change after it is built.

## `private LEntryDraft? LDisplayShown`

The shown draft the sound half holds, passed unread to the engine.

## `private CLedgerNoticed LDisplayNoticed`

The atelier's repaint memory, held by the display's rules.
Every read here runs on a repaint and shows its failure through it, so a lasting fault shows once.

## `public CLecternCard CDisplayCardRead()`

The shown entry's cards, each ready to paint, with whether their sections show.
The order, the Source lines, the link targets and the folds are four reads, each answering a fallback when refused.
Each refusal also shows its own notice, so the page still draws.
The unknown mark is the engine's word, so a frame or a sentence embeds it ready.
The folds are one read for the whole entry, never one read per card.

## `public bool CDisplayFoldToggle(long cardId, bool folded)`

The user folded or unfolded one card of the chosen entry.
It makes one save or delete call and answers whether the port returned without throwing.
Ignoring a nonpositive card id still yields true, without confirming a stored row change.
The port raises the fold bulletin, and `CDisplay.CDisplayFoldChanged` then makes the driver read the cards again.
So this view and every other view on the entry repaint from the store alike.
The card id is the leaf's own `CLeafId`, handed back unread.
An unsaved card writes nothing, since the clerk below refuses its id.
No entry chosen writes nothing and answers false.
A refused write shows `Fold.SaveFailed` every time, since the user acted, and answers false.
The caller puts its chevron back on false, since no bulletin will repaint it.
Folding is no edit, so it never dirties a draft and never enters undo.

## `private IReadOnlySet<long> LDisplayFoldRead()`

The ids of every folded card of the chosen entry, Meanings and Collocations together.
No entry chosen answers an empty set.
A refused read shows `Fold.ReadFailed` through repaint memory.
An empty set keeps the cards drawable.

## `private IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LDisplayTranslationRead(LEntryDraft shown)`

The link targets of every card of the shown entry, keyed by card id.
A refused read shows `Display.TranslationFailed` through repaint memory.
No targets keeps the cards drawable.

## `private LSentenceOrder LDisplayOrderRead(string language)`

The sentence order of `language`.
A refused read shows `Display.OrderFailed` through repaint memory.
The default order keeps the cards drawable.

## `private IReadOnlyDictionary<long, string> LDisplayCitationRead(LEntryDraft shown)`

The ready line of every Source the shown entry cites.
A refused read shows `Display.CitationFailed` through repaint memory.
No citation lines keeps the cards drawable.
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
