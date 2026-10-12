# vocabulary.json
Hash: `c520c8e788f90369`

## file

Display vocabulary for Korean.
It holds only the order of the two frame fields an example row shows.
It has no `registers`, `parts`, `features`, `values`, `paradigms` or `retired` section.
Each loader reads a missing section as empty, so Korean offers no register presets and no part-of-speech presets.
A user may still type any register or part of speech, which is stored as typed.
Add a language by creating `languages/<Name>/vocabulary.json`.

## `exampleOrder`

It states which of the two frame fields an example row writes first, `particle` or `dependence`.
It states nothing either may hold.
Korean lists `dependence` first and `particle` second, so the dependence is written before the particle.
Without both names the default order applies.
