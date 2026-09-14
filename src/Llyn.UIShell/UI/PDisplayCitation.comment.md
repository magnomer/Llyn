# PDisplayCitation.cs

## `public partial class PDisplay`

The bylines the reading view writes beside quoted sentences.
They are read once per shown entry and handed to the converter the example template binds through.

## `private void PDisplayCitationShow()`

Reads the byline of every Source and puts it where the example template finds it.
A failed read leaves the examples without bylines rather than failing the view.
It runs before the cards are handed over, and again on every bulletin that reloads the entry.
So a Source edited elsewhere reads fresh.

## `private PCitationConverter PDisplayCitationRead()`

The converter the example template binds through, held in the view's resources.
