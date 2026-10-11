# LPressStatus.cs
Hash: `8d7ae2d160666638`

## `public enum LPressStatus`

How a print request ended.
An unavailable printer is common and expected, so it is an answer rather than an exception.

- `LPressStatusDone`: the job was handed to the printer.
- `LPressStatusOffline`: the printer was unknown or unavailable.
- `LPressStatusError`: the job failed for another reason.
