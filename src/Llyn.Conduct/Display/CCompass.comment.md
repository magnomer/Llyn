# CCompass.cs
Hash: `9603d322a2615d76`

## `public sealed class CCompass`

The reading view's floating contents.
It reads the rows the contents list and the place of a card on the page.
Only [CDisplay](CDisplay.comment.md) builds one, over its own rules, and hands it out.
The parts it names are [CCompassPart](CCompassPart.comment.md), and each row is a [CCompassRow](CCompassRow.comment.md).
It holds no state of its own.

## `private readonly LDisplaySound _cCompassVoice;`

The rules' sound half, which holds the shown draft the rows and the card place are read from.

## `private readonly CLedgerNoticed _cCompassNoticed;`

The atelier's repaint memory, held by the display's rules.
The compass reads on a repaint, so a lasting name fault shows once.

## `private readonly LEntryPort _cCompassEntryPort;`

The entry port that finds which list of the shown draft holds a card.

## `private readonly LVistaPort _cCompassVistaPort;`

The vista port the twin names are resolved through.

## `private readonly LCardPort _cCompassCardPort;`

The card port that answers where each collocation of the shown draft sits.

## `private readonly LSettingsPort _cCompassSettings;`

The settings port a name fault notice is shown with.

## `private readonly CEnvoy _cCompassEnvoy;`

The envoy a refused name lookup is shown through.

## `internal CCompass(LDisplay display, LEntryPort entries, LVistaPort vistas, LCardPort cards, LSettingsPort settings, CEnvoy envoy)`

Only the display builds it, over its rules and the ports the atelier handed down.
It keeps the rules' sound half and repaint memory, since both live as long as the rules.

## `public (CCompassPart, int)? CCompassCardFind(long id)`

Which card list of the shown entry holds the card `id`, and at which place.
The engine finds the card, and its list maps by name to the compass part.
A linked collocation answers the place of the top-level meaning holding its home, where the page shows its row.
Its home is its first link to a meaning the draft still holds.
The engine's collocation layout answers the home, so the compass only finds the meaning listing the card.
A collocation with no such link is placed among the Other collocations, in their own order.
The place is counted in the order `CFolio.CFolioOrderRead` sets, as the reading view's cards are.
So the spotlight lands on the card the page shows, even when the engine list is out of order.
It answers null when nothing is shown or the entry has no such card.

## `public IReadOnlyList<CCompassRow> CCompassRead(IReadOnlyList<CCompassPart> parts, Func<string, string> lookup)`

The read behind the floating contents, one row per shown part and one per card under it.
A meaning with collocations adds one count line.
The driver names the parts that stand on the page, in page order, and its own text lookup.
The collocation part lists only the collocations with no live link, which the page shows under Other collocations.
With none, the part gives no row at all, as the page then hides its Other section.
With nothing shown, every part still gives its title row.
The homes and the Other list come from one engine read of the collocation layout.
The compass shows no flags, so it hands that read an empty target map.
The collocations homed at a top-level meaning give one muted line one level below it, with that meaning's card place.
The line reads "N collocations", worded with `Display.CollocationCountOne` for one and `Display.CollocationCount` for more.
It carries no number, since it names no card.
The cards of each list come in the order `CFolio.CFolioOrderRead` sets, as the page shows them.
So a row's card place is the index of the same card on the page.
Conduct chooses every wording key and the driver's lookup turns it into text.
A card is named by its own title, else by its kind, and an uncertain title by the unknown mark.
An Other collocation's kind is `Display.CollocationSingle`.
Its number is carried apart from its name, since a number is not part of a sentence.
A card's title goes through the one state map, and its number is copied as the engine holds it.
The full card map is not run, since the contents need no card's lists or links.
The engine then numbers the names two rows share, so the contents never show two rows alike.
Count lines skip that rule, since a count is not a name.
Two meanings may hold as many collocations, and their lines stay unnumbered.

## `private static string LCompassKeyRead(CCompassPart part)`

The wording key of each part, mapped by name so an unknown part throws.
The collocation part uses `Display.CollocationOther`.

## `private IReadOnlyList<string> LCompassNameResolve(IReadOnlyList<string> labels)`

The compass wordings made distinct by the engine's twin rule.
A refused lookup is shown once through the repaint memory and answers the plain wordings.
