# CArticulationAid.cs
Hash: `5537e1b6d6f0b0ce`

## `public sealed record CArticulationAid(CArticulation CArticulationAidVowel, CArticulation CArticulationAidConsonant)`

Both IPA charts of the input aid, ready to build in one go.
The aid builds both charts at the same moment, so one read hands over both.

**Parameters**

- `CArticulationAidVowel`: the vowel chart, rows by tongue height and columns by backness.
- `CArticulationAidConsonant`: the pulmonic consonant chart, rows by manner and columns by place.
