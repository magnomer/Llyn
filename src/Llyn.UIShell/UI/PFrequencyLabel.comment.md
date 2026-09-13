# PFrequencyLabel.cs

## `internal static class PFrequencyLabel`

The two strings a frequency chip shows, shared by the reading view and the editor.
One entry must read the same in both modes, so neither surface words the chip itself.

## `internal static string PFrequencyLabelFormat(LFrequency frequency)`

The band when the pack maps one, else the raw answer.

## `internal static string PFrequencySourceFormat(LFrequency frequency, string unit)`

The source and the raw answer, so the band is never the only thing said.
An answer the record calls numeric is followed by the unit, since a bare figure says nothing about its scale.
A coded answer such as a Longman list mark stands alone, because no unit fits it.
