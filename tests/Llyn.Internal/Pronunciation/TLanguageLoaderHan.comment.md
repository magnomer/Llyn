# TLanguageLoaderHan.cs

## `public sealed class TLanguageLoaderHan`

Covers the pack loader reading the Han-language sections: the script styles, the rime books and the hypothesis.
A `script` list loads its styles in written order with their form fields, groups, prefix, rewrite rules and gloss pattern.
A style row missing its match pattern is skipped alone, and the optional keys read as empty or zero.
A `fanqie` list loads its books in written order with their form fields, marker, busy pattern and interval.
A book row missing its match pattern is skipped alone, and the optional keys read as null or zero.
A book row without a source is labelled by its book name.
Two rows may share a book under two sources.
A `hypothesis` object loads its initial and final tables and its tone rules.
A section missing a table loads as null.
A `hypothesis` file name loads the tables from that file in the pack folder.
A missing file, or a name reaching outside the folder, loads as null.
The shipped Classical Chinese pack is read for the real sections, and throwaway packs for the edge cases.

## `private static LLanguage TLanguageHanLoad(string json)`

Loads a throwaway pack written from the given JSON and removes it again.
