# LPortraitPort.cs

## `public interface LPortraitPort`

The slice of the engine a panel sees when it prints, exports or imports.
Printing and exporting portray the record a vista chose, so the caller hands the vista and never the id.
The markup import runs in two steps, reading the cargo and then storing it under declared intakes.
`LEngine` implements it today, and a portrait clerk takes it over when the parts are dismantled.
