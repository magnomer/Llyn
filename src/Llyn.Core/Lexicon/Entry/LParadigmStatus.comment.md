# LParadigmStatus.cs

## `public enum LParadigmStatus`

What stands in a paradigm row for its inflected form.
`LParadigmClerk.LParadigmClerkCheck` answers it, so no shell decides it again.

## `LParadigmStatusText,`

The row holds a written form.

## `LParadigmStatusUnknown,`

The row is marked as having no known form.

## `LParadigmStatusPending,`

The row has no form yet and the inflections are still being fetched.

## `LParadigmStatusLost,`

The row has no form, nothing is being fetched, and the morphology setting is on.

## `LParadigmStatusAbsent,`

The row has no form, and the morphology setting is off or the written form is empty.
