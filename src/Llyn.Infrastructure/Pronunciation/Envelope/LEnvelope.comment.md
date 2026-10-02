# LEnvelope.cs
Hash: `90ac78a15d520460`

## `internal static class LEnvelope`

Reads the parts of a fetch request a pack row declares beside its address.
The script, fanqie, shengfu and reflex rows post a form, and source attempts and reflex rows send headers.

## `private const string LEnvelopePayload = "form";`

The key under which a row declares the fields it posts.

## `private const string LEnvelopeHeader = "headers";`

The key under which a row declares its request headers.

## `public static IReadOnlyDictionary<string, string> LEnvelopePayloadRead(JsonElement row)`

The `form` object as posted fields, string values only, in written order.
A row without a form posts nothing but still asks the address.

## `public static IReadOnlyDictionary<string, string>? LEnvelopeHeaderRead(JsonElement row)`

The request headers a row declares under `headers`, string values only.
An absent or empty block reads as `null`, and the client's default headers alone are sent.
