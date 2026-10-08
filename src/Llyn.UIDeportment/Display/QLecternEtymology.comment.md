# QLecternEtymology.cs
Hash: `6e4de0055e2e5d2c`

## `public sealed class QLecternEtymology`

The reading view's etymology section, drawing the narrative and its source links.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and subscribes its clicks.
The lectern subscribes its redraw to the display's open and close.
It knows no window, so a word's offer leaves through `QLecternEtymologyNotice`.

## `public QLecternEtymology(FrameworkElement surface, CDisplayCard area, CDisplayRoute route)`

Pulls the etymology field and its section from `surface`.
The card area answers the etymology read, and the route area every link and word gate.
The etymon command is bound on `surface`, since a source link raises it from inside the field's template.

## `internal event Action<PMention, CMentionOffer?>? QLecternEtymologyNotice;`

Carries a clicked word's offer and the prose control it sits under.
The view that owns the lectern wires it to the mention menu's `QMentionOfferRefine`, which paints the menu.

## `public void QLecternEtymologyRefine()`

Sets the field's narrative and source links from the etymology `CDisplayEtymologyRead` answers.
The lectern subscribes it to both open and close, since a closed display answers a hidden etymology.
It hands the field the ready verdicts for its read narrative and its row of links.
Then it sets the field's and the section's visibility.

## `private void QLecternEtymonObserve(object sender, ExecutedRoutedEventArgs e)`

Hears a click on a source link and hands `CDisplayChipOpen` the entry id it carries.

## `private void QLecternEtymologyObserve(object? sender, PMentionArgument e)`

Hears a word clicked in the etymology prose and reads the raw click values.
It hands them unchanged to its one gate, `CDisplayEtymologyFind`.
The offer leaves through `QLecternEtymologyNotice` under the prose.
