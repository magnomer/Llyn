# CCompass.cs
Hash: `bbba0058301e553a`

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

## `private readonly LEntryPort _cCompassPort;`

The entry port the twin names are resolved through.

## `private readonly LSettingsPort _cCompassSettings;`

The settings port the ready notice is read from.

## `private readonly CEnvoy _cCompassEnvoy;`

The envoy a refused name lookup is shown through.

## `internal CCompass(LDisplay display, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)`

Only the display builds it, over its rules and the ports the atelier handed down.
It keeps the rules' sound half and repaint memory, since both live as long as the rules.

## `public (CCompassPart, int)? CCompassCardFind(long id)`

Which card list of the shown entry holds the card `id`, and at which place.
The engine finds the card, and its list maps by name to the compass part.
The place is counted in the order `CFolio.CFolioOrderRead` sets, as the reading view's cards are.
So the spotlight lands on the card the page shows, even when the engine list is out of order.
It answers null when nothing is shown or the entry has no such card.

## `public IReadOnlyList<CCompassRow> CCompassRead(IReadOnlyList<CCompassPart> parts, Func<string, string> lookup)`

The read behind the floating contents, one row per shown part and one per card under it.
The driver names the parts that stand on the page, in page order, and its own text lookup.
The cards of each list come in the order `CFolio.CFolioOrderRead` sets, as the page shows them.
So a row's card place is the index of the same card on the page.
Conduct chooses every wording key and the driver's lookup turns it into text.
A card is named by its own title, else by its kind, and an uncertain title by the unknown mark.
Its number is carried apart from its name, since a number is not part of a sentence.
A card's title goes through the one state map, and its number is copied as the engine holds it.
The full card map is not run, since the contents need no card's lists or links.
The engine then numbers the names two rows share, so the contents never show two rows alike.

## `private static string LCompassKeyRead(CCompassPart part)`

The wording key of each part, mapped by name so an unknown part throws.

## `private IReadOnlyList<string> LCompassNameResolve(IReadOnlyList<string> labels)`

The compass wordings made distinct by the engine's twin rule.
A refused lookup is shown once through the repaint memory and answers the plain wordings.
