# LSheetStyle.cs

## `public static class LSheetStyle`

The page's whole appearance, built from the display's theme.

## `public static string LSheetStyleRead(LTheme theme)`

Colours become custom properties so one declaration feeds every rule.
Sizes, margins and radii are the display's own values in device-independent pixels.
Backgrounds are forced to print, because a card without its ground is not the card seen.
Cards are kept off page breaks, which the panel never has to think about.
