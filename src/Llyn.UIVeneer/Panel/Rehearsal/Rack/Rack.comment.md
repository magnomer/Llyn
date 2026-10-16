# Rack

The deck list with the listing, and the deck editor.
Its markup carries no hook.
The Deportment driver `QDeck` fills the grain box, the rows, their icons and their menus.
The Deportment driver `QCue` fills the listing and its row parts.
The Deportment driver `QCharter` fills the deck editor `PCharter`.

## `PDeck`

The shell behind `PDeck.xaml`, placed by the main window.
Its constructor only calls `InitializeComponent()`.

## `PCharter`

The deck editor of the chosen deck, placed by `PDeck` and shelled as `PDeck` is.
It has one markup block for reading and editing, copying `PAutograph` against its read side `PVita`.
  Each value sits in a bare input at the place and size of its text.
  So no separate form appears for editing.
  The captions, headings and choice texts are markup words, and the driver sets only values.
The direction rows are a list in Conduct's order.
  The driver sets each row's caption from the key Conduct chose.
Each face of a direction holds five side boxes, the five sides of the phone's card.

## Layout

The deck list takes the narrow first column, under a bar with the grain box and the two create buttons.
  The create buttons are icons tipped `Deck.New` and `Deck.FolderNew`.
  The grain box takes the rest, so the bar fits the column.
The export line sits under the list.
Each deck row ends in two order buttons, tipped `Deck.OrderUp` and `Deck.OrderDown`.
The wide column holds the listing, under a bar with the free cue and remove buttons.
The listing is a list box with extended selection, so remove takes every selected row.
  Its rows are `Theme.Catalog.Item` rows, the catalog row of the other panels.
Each listing row holds the flag toggle, the line over the orphan notice, the excluded toggle and two order buttons.
  The toggles are icon toggles, and the order icons are 16 pixels.
  The order buttons reuse the tips `Deck.OrderUp` and `Deck.OrderDown`.
The cell editor `PJotter` sits under the listing in the same frame, its height capped so the listing keeps room.
  It is collapsed until one free flashcard is selected.
The third column at the right holds three views under a strip of three tab buttons in the bar row.
  The buttons `PDeckAccession`, `PDeckStencil` and `PDeckCharter` name the Add, Card and Deck views.
Each view sits alone in its own sheet, and the driver shows one sheet at a time.
  The sheets `PDeckAccessionSheet`, `PDeckStencilSheet` and `PDeckCharterSheet` hold `PAccession`, `PStencil` and `PCharter`.
  A sheet keeps the view's own visibility apart, so each visibility has one writer.
The deck editor `PCharter` and the template editor `PStencil` stay collapsed while no deck is chosen.

## Stamp

Commit: `121691f60459`
- `PCharter.xaml` `afe22729ff3fc023`
- `PCharter.xaml.cs` `cb85fb7159da24e0`
- `PDeck.xaml` `9c54107a68f5afaf`
- `PDeck.xaml.cs` `c929dd801a891d3a`
