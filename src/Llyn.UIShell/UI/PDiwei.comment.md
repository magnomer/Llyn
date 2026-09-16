# PDiwei.xaml

## `<UserControl x:Class="Llyn.UIShell.PDiwei"`

The category page of the rime table: a headword line for the category and the characters placed under it.
It stands where the reading view stands and is shown in its place while a category is open.
The headword line carries the key, the kind chip and the language chip, as an entry's line would.
On an initial's page the sections list the placements by division, each row a rime and its characters.
On a rime's page they list them by the initial's articulatory place, each row an initial and its characters.
Under each section heading the tally lines print, one per borrowing language and kind of reading.
Each line lays its parts out in a row, each part followed by its raised character count.
Clicking a part opens a popup listing those characters, each a chip opening its entry as the plate's chips do.
A switch at the right end of every section heading picks IPA or respelling for those lines.
The switches repeat one state, so any of them flips all, and they show only while respelling is on.
