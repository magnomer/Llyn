# LImage.cs

## `public sealed record LImage(`

One Image — independent data owned by nothing. No Entry, Meaning, or Collocation contains an Image; any number of Meanings and Collocations *reference* it instead, and the order an Image appears in lives on each reference rather than here. `LImageId` is the identity — an opaque, program-generated stable id; `LImageLocation` says where the picture is read from and is never identity, so correcting a path leaves the id and every reference to it untouched.

The location is one field for two kinds of place: a file on this machine and an address on the web. The store keeps the text it was given and resolves nothing, because a path that is unreachable today may be reachable tomorrow and a row that named a picture is not the same as a row that never named one.

**Parameters**

- `LImageId` — Opaque, program-generated stable id.
- `LImageLocation` — Where the picture is read from — a file path or a web address — and what is known about it. An Image that is there but unreadable is not an Image that was never written.
