# LSpeechClerk.cs
Hash: `1215322af9ec046e`

## `public static class LSpeechClerk`

The rules of an entry's parts of speech as the user builds the list.
The committed parts are chips, and the text still being typed is one pending part.
Names match without regard to case, so one part of speech never shows twice.

## `public static IReadOnlyList<LSpeechDraft> LSpeechParse(IReadOnlyList<LSpeechDraft> held, string? typed)`

The committed parts with the trimmed typed text appended as a custom part.
A blank text or a name already held adds nothing.

## `public static string? LSpeechPendingRead(IReadOnlyList<LSpeechDraft> held, string? typed)`

The trimmed typed text when it appends a part to the chips, else null.

## `public static IReadOnlyList<LSpeechDraft> LSpeechChipRead(IReadOnlyList<LSpeechDraft> shown, string? pending)`

The draft's parts as chips, cleaned.
The last part is left out when it is the pending custom one.

## `public static bool LSpeechTypedCheck(string? typed)`

Whether the typed text names a part of speech once trimmed.

## `public static IReadOnlyList<LSpeechDraft>? LSpeechAdd(`

The committed parts with the trimmed name appended, or null for a blank name.
A name already held answers the list unchanged, so the caller still sends it once.
`declare` answers the catalog value, and a refused declaration keeps the name as a custom part.

## `public static LSpeechOffer LSpeechFind(`

The catalog's parts whose name holds the trimmed typed text, each marked when the chips already hold it.
A blank text matches every part.
The offer shows only while the text is not blank and some part matches.

## `public static IReadOnlyList<LSpeechDraft> LSpeechRemove(IReadOnlyList<LSpeechDraft> held, string? name)`

The committed parts without the one shown under this exact name.

## `public static (IReadOnlyList<LSpeechDraft> LSpeechHeld, string LSpeechTyped) LSpeechSettle(`

Keeps the committed parts and the typed text while the draft shows exactly what they build.
Otherwise the draft changed from elsewhere, so its parts become the committed ones and the typed text clears.

## `private static IReadOnlyList<LSpeechDraft> LSpeechNormalize(IReadOnlyList<LSpeechDraft> shown)`

The draft's parts as chips, trimmed, never blank, and each name once.
