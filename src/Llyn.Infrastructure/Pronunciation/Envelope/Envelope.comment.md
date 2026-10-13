# Envelope

This folder reads the parts of a fetch request that a pack row declares beside its address.
It stands apart from the section loaders because the script, fanqie, shengfu and reflex rows all post a form.
Source attempts and reflex rows also send headers, so one reader serves them all.

## `LEnvelope`

`LEnvelopePayloadRead` returns the `form` object as posted fields.
Only string values are kept, in written order.
A row without a form posts nothing but still asks the address.
`LEnvelopeHeaderRead` returns the request headers a row declares under `headers`, string values only.
An absent or empty block reads as `null`, and the client's default headers alone are sent.

## Stamp

Commit: `c2f335c331fe`
- `LEnvelope.cs` `90ac78a15d520460`
