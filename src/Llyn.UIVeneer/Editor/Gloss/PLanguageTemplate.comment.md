# PLanguageTemplate.xaml
Hash: `0e92873e942cc0c6`

The language selector row of the editor, as markup alone.
The editor's markup merges it, and the editor's fill fills the named parts.

## `Theme.Language.Row`

Shows a language flag and name, with an empty circle when no flag exists.
The circle starts hidden, and the fill shows it for a language without a flag.
The flag and name parts carry the names the corpus speaker row uses, so one fill serves both.
Clicking the row delegates speaker selection to the editor.
