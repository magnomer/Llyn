# PSCustoms.xaml

The declaration shown before imported entries enter the workspace.
One row per entry in the file: number, headword, language, the way it enters, and the stored entry joined.
Mode and target are ordinary dropdowns, so a reader changes one row without touching the rest.
The mode rows are named, and the deportment tags them with the engine's own mode values.
The target rows show the candidate's headword and its id, since candidates share one headword by definition.
The loss column says what a replaced entry gives up, and stays blank for any other mode.
Accept is the default and Cancel answers the escape key, as in the leave dialog.
The same window carries a second face: the omissions the import reported, one line and text per row.
That face has one Close button.
Only one face is visible at a time, so the window keeps one size and place through the import.
