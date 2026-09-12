# LSentenceLoader.cs

## `public static class LSentenceLoader`

Reads the one thing a language pack says about a Sentence's frame: which field is written first.
The pack states an order and nothing else.
No marker and no role is ever shipped, so nothing here reads a value either could hold.

## `public static LSentenceOrder LSentenceLoaderLoad(string language)`

The order stated by `languages/<language>/vocabulary.json`, under `exampleOrder`.
A pack that is missing, unknown, or silent on the key leaves the marker written first.
That fallback is a layout choice the form needs to draw at all, and never a claim about the language.
A key naming only one of the two fields is not an order, so it is left as none stated.
