# CDisplayCard.cs
Hash: `f0a0586ff8aa8b25`

## `public sealed class CDisplayCard`

The reading view's card reads.
It reads the cards, the usages pointing at the entry, and the etymology.
Only [CDisplay](CDisplay.comment.md) builds one, over its own rules, and hands it out.
So every driver over one display reads the same shown draft.
It holds no state of its own.

## `private readonly LDisplay _cDisplayRule;`

The display rules the shown draft, the chosen entry, the media port and the repaint memory come from.

## `private readonly LEntryPort _cDisplayPort;`

The entry port the link targets, Source lines, usages and etymology are read through.

## `private readonly LPhonologyPort _cDisplayPhonology;`

The phonology port the sentence order is read from.

## `private readonly LSettingsPort _cDisplaySettings;`

The settings port the ready notice and the unknown mark are read from.

## `private readonly CEnvoy _cDisplayEnvoy;`

The envoy a refused read is shown through.

## `internal CDisplayCard(LDisplay display, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, CEnvoy envoy)`

Only the display builds it, over its rules and the ports the atelier handed down.
It takes the rules whole, since the shown draft and the chosen entry change after it is built.

## `private LEntryDraft? LDisplayShown`

The shown draft the sound half holds, passed unread to the engine.

## `private CLedgerNoticed LDisplayNoticed`

The atelier's repaint memory, held by the display's rules.
Every read here runs on a repaint and shows its failure through it, so a lasting fault shows once.

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
