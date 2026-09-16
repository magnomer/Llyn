# TAnatomy.cs

## `public sealed class TAnatomy`

Covers the anatomy the Classical Chinese pack cuts from a reflex reading, over the shipped `anatomy.json`.
A Korean reading falls into compatibility jamo with its bracketed variant dropped, an open syllable with no coda.
A Japanese reading is cut by mora, so `りむ` keeps `mu` as its coda and `りゃく` keeps `ku`.
A Sinitic reading keeps its tie bar and nasal mark with the onset and its length mark with the vowel.
The tone comes out as digits, a sandhi pair hyphenated, and an unreleased stop stays whole in the coda.
A language no rule names, and an entry of a language without rules, leaves every part blank.
It also covers the seams: a save stores it, a text request cuts it, a language change recuts it.
