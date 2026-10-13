# Articulation

This folder holds the phonology panel and the articulation aid, the charts of sounds that help input.
The aid stands apart because the panel that hosts it shows and hides it whole.
It owns no fold control.
Folding it is the panel's layout decision.

## `PArticulation`

The shell behind `PArticulation.xaml`, placed by the phonology panel.
Its constructor only calls `InitializeComponent()`.
The glyph chip carries no hover or pressed trigger.
`QLookSound` lights it, once `QArticulation` registers these styles.
One glyph is one borderless chip, and one header is one quiet label.
A chart heading carries the helper ink, the one place the helper hue is written rather than laid down.
A chart of a hundred bordered boxes would read as a grid of controls rather than a table of sounds.
So the chip carries no outline until the pointer is over it.
The search dropper and the catalog rows behave the same.
The glyph is not focusable, so pressing it leaves the caret where the reader put it.
That is what lets the character land at the caret rather than at the end.

## Aid band

The band takes the helper ground rather than the panel surface.
So an aid is never mistaken for the work it assists.
It runs edge to edge like the seams.
The left inset puts its charts where the ordering bar and the catalog rows begin.
Every side of that space is padding rather than margin.
That is because a margin under a coloured band would leave a white strip above the seam.

## `PArticulationRack`

The two charts stand side by side while the window is wide enough for both.
A rack of two rows and two columns holds them.
The code moves the consonant chart between the two places.
The consonant table must stay whole, because its place and manner grid is what makes it readable.
So it takes a row of its own rather than losing its right edge to the vowel chart beside it.

## `Articulation.Chart`

Each chart is a card of the shared card shape.
So the two tables read as two objects rather than one long sheet.
The card hugs its table on the left.
That keeps the narrow vowel chart from stretching to the consonant chart's width.
The two cards meet at the top.
The vowel chart is one row shorter, and stretching it would leave an empty band under its table.
A card keeps the working surface and takes the helper edge.
So a chart reads as a white sheet laid on the helper ground.
The band holds the slim scroll lane rather than the full gutter.
So the space under the charts matches the space above.
The lane still scrolls sideways for a window too narrow for the consonant table alone.

## `PVowel`

The table is empty in markup and filled in code.
Its shape is a chart, not a layout, so it is written where the chart is written.

## `PPhonology`

The shell behind `PPhonology.xaml`, the phonology panel the main window places.
Its constructor only calls `InitializeComponent()`.

## Seams

The seams part the pronunciation catalog from the entry it opens.
The row seam runs under the ordering bar and the command row and is bled past the panel margin.
The second row seam stands only while the articulation aid is unfolded.
That is because a folded aid leaves no row for it to close.
The column seam sits in the middle of the gutter and reaches the foot of the window.
Neither seam encloses anything, since a line marks a division and a box would claim an object.

## `PSequence`

The ordering button and the search field are one control over the inventory column.
The library panel joins its own the same way.
The articulation fold stands outside this bar, because it shapes the editor rather than the catalog.
The ordering picker is the shared `PChoiceOrder`, placed as `PPhonologyOrder`.
The language filter is the shared `PChoiceFilter`, placed as `PPhonologyFilter`.

## `PArticulationHelper` and `PArticulationSeam`

The fold control of the input aid stands beside the ordering and the search.
The three controls that shape what the panel shows are read as one row.
It carries the height, the corner and the marked label of the bar beside it.
So the row reads as one control.
The aid is two full charts, so it is folded away until asked for.
The seam and the aid start folded, because the toggle starts unchecked.
`QArticulation` shows both when the toggle is checked, since the markup carries no fold binding.

## `PPhonologyRail`

The action row on the broader side, the shared `PPanelRail`, the same row the library panel carries.
One slot sits between save and export, and it holds whichever pair the mode asks for.
Reading shows the pair that walks the window's trail of records.
Writing shows the pair that walks the chronicle of the editor in front.
The mode toggle stands there rather than inside the display, because the display is shared.

## Aid placement

The input aid spans both columns and is bled to both panel edges like the seams.
Its ground is a band across the panel rather than a box inside it.
It serves the pronunciation field on the right, and the reader reads the catalog on the left.
Neither column owns it.

## `PInventory`

A row reads as the pronunciation first and the word after it, which is the order this panel browses in.
The word is quieter than the pronunciation, because the pronunciation is what is being looked for.
The catalog uses the shared `Theme.Catalog.Frame`, `Theme.Catalog.Scroll` and `Theme.Catalog.Row` styles.
The scroll bar takes its lane only when the list overflows, and the rows then narrow.

## Stamp

Commit: `c2f335c331fe`
- `PArticulation.xaml` `fcb4d7e02766ce3b`
- `PArticulation.xaml.cs` `7f4f9a2f0988b815`
- `PPhonology.xaml` `4c2070af2e07035a`
- `PPhonology.xaml.cs` `36bc90764f916acb`
