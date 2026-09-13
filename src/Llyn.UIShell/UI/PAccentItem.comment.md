# PAccentItem.cs

## `internal sealed class PAccentItem : INotifyPropertyChanged`

One pronunciation row after the primary, as the editor and the reading view show it.
It carries the draft row's id, its variety, and the IPA the field is bound to.
The variety is shown as a flag when the pack draws varieties as flags, and as a label otherwise.
The flag may arrive after the row, so it notifies when it lands.

## `public string PAccentItemLabel`

The variety's localized name, blank while a flag stands for it.
Blank is what collapses the label, so a row never shows both.

## `public ImageSource? PAccentItemFlag`

The variety's flag, or null while none is known.
Setting it also announces the label, since the label yields to it.

## `public string PAccentItemIpa`

The IPA as typed or as the draft holds it.
It announces only a real change, so a render writing the same text stays silent.

## `internal static PAccentItem PAccentItemCreate(PWindow host, string language, bool flagged, LPronunciationDraft spoken)`

Builds the row for one draft pronunciation under the language it belongs to.

## `internal static string PAccentLabelFormat(PWindow host, string variety)`

A variety's label is its localized `Variety.*` text when one exists and its raw name otherwise.
An unnamed variety has no label.

## `internal static ImageSource? PAccentFlagFind(string language, bool flagged, string variety)`

Looks a variety's flag up in the ensign store, only in flag mode and only for a named variety.
The store may not hold it yet, and null then says so.

## `internal void PAccentFlagUpdate(string language, bool flagged)`

Looks the flag up again after the store has been filled.
