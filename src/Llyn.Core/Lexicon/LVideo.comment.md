# LVideo.cs

## `public sealed record LVideo(`

One Video — independent data owned by nothing.
No Entry, Meaning, or Collocation contains a Video.
Any number of Meanings and Collocations *reference* it instead.
The order a Video appears in lives on each reference rather than here.
`LVideoId` is the identity, an opaque and program-generated stable id.
`LVideoLocation` says where the film is read from and is never identity.
`LVideoSpan` says which stretch of it is worth watching and is never identity either.
Correcting a path or a span leaves the id and every reference to it untouched.

A Video is shaped exactly like an Image because it is kept exactly like one.
The location holds a file on this machine or an address on the web.
The store keeps the text it was given and resolves nothing.
A row that named a film is not the same as a row that never named one.

**Parameters**

- `LVideoId` — Opaque, program-generated stable id.
- `LVideoLocation` — Where the film is read from, a file path or a web address.
  It also carries what is known about that location.
  A Video that is there but unknown is not a Video that was never written.
- `LVideoSpan` — The stretch worth watching, written as `mm:ss - mm:ss`.
  The store keeps the text and reads no moments out of it.
  A span that says nothing is a film watched whole.
