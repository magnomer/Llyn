# anatomy.json

Declares how a reflex reading is cut into its anatomy: onset, vowel, coda and tone.

## `anatomy`

Each row names one reflex language or a list of them, and its `ipa` object holds the rules.
`decompose` controls Unicode decomposition before the ordered rewrite pairs run.
`rewrite` pairs are regular-expression substitutions applied in order to normalize transcription.
`match` captures onset, vowel, and coda, with tone captured when the language rules include it.
