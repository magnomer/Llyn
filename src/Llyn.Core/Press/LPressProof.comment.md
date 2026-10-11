# LPressProof.cs
Hash: `d0f82653948a3c31`

## `public interface LPressProof : IDisposable`

The pages of one layout, kept so each page can be drawn when it scrolls into view.
A new setting that changes layout yields a new proof.
The old one is disposed once no page draw still reads it.
Pages are counted from zero here, as a list of drawn pages is.

## `int LPressProofCount { get; }`

How many pages the layout produced.
It is known without drawing any page.

## `LPressPaper LPressProofRead(int page);`

The size of `page` as laid out, in inches.
The canvas shapes its sheets from the first page's size rather than from the setting.
So rounding never makes a sheet and its page differ.

## `Task<LPressImage> LPressProofDraw(int page, int resolution);`

Draws `page` as a picture at `resolution` dots per inch of its own paper.
The resolution is a document unit, so no screen geometry reaches Core.
The picture's pixel size lives in the PNG itself.
