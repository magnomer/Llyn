# Media

This folder holds the tables of the independent Image and Video and the associations that reach them.

## `LSchemaImage`

Creates the independent Image and the three associations that reach it.
The Image carries its own opaque id and the location it is loaded from, and nothing about it is identity.
The association cascades with the card or Situation and never with the Image.
So an Image survives every detach, and the store refuses to delete one anything still points at.

## `LSchemaVideo`

Creates the independent Video and its three associations, shaped exactly like the Image tables.
A Video is kept exactly like an Image.
It carries its own opaque id and the location it plays from.
The association cascades with the card or Situation and never with the Video.
So a Video survives every detach, and the store refuses to delete one anything still points at.

## Stamp

Commit: `c2f335c331fe`
- `LSchemaImage.cs` `fc247d892e283225`
- `LSchemaVideo.cs` `2efb072b3d190c61`
