# LParcel.cs
Hash: `1ad3b2f7ad138c3e`

## `public sealed record LParcel(string LParcelId, string LParcelTitle, string LParcelMime, byte[] LParcelBytes);`

One attachment pushed to Joplin as a resource that notes can link to.
Its id derives from the file content, so a changed image gets a new id.
The note body that references the id changes too, so the note is pushed again.
That is why `LOutpostParcelSave` may skip an id that already exists.
The id is fixed, so a note body can name it before the upload happens.

**Parameters**

- `LParcelId` — The resource's fixed Joplin id, 32 lowercase hex characters.
- `LParcelTitle` — The name Joplin shows for the resource.
- `LParcelMime` — The media type Joplin records for the bytes.
- `LParcelBytes` — The file content to upload.
