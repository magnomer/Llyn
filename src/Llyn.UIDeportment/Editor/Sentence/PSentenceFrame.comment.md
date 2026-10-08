# PSentenceFrame.cs
Hash: `9666ee246efc010d`

## `internal sealed class PSentenceFrame : INotifyPropertyChanged`

The frame a sentence row is read under, a marker and a role.
Nothing offers a value for either, because nothing ships one.
Which of the two is written first is the language pack's to say and never the frame's.
The frame is told the order and holds only what it was told.
Its row re-raises every change it announces.

## `internal PSentenceFrame(CStateWording particle, CStateWording dependence)`

The frame of one row, built from the marker and the role of the row's first draft.
It starts in the default order, marker first, until the language pack's order arrives.

## `public CStateWording PSentenceFrameParticle`

The marker as the draft holds it, set only from the draft.

## `public CStateWording PSentenceFrameDependence`

The role as the draft holds it, set only from the draft.

## `public bool PSentenceFrameVisible`

Whether the row shows its frame at all.
A row that carries a marker or a role always shows one, because the reading view draws it.
A row carrying neither shows one only when the card has been asked for it.

## `public bool PSentenceFrameWritten`

Whether the frame stands on what the row holds rather than on the card being asked for one.
The card's switch stands down while it does, because a frame already written cannot be opened or closed by asking.
It gives its room back rather than keeping it.
A frame field says something unless Conduct words it muted.
It then holds text or the unknown mark.

## `public string PSentenceFrameGap`

The separator drawn between the marker and the role inside the frame.
It is one space when both are written and nothing otherwise.
The writing view spaces the frame exactly as the reading view does.

## `public CSentenceOrder PSentenceFrameOrder`

The slots the language pack states for the marker and the role.
The row painter turns each slot into a grid column, so the frame holds no column of its own.
Until the pack's order arrives it holds `CSentenceOrder.CSentenceOrderPlain`, so the frame states no order itself.

## `internal void PSentenceFrameApply(CSentenceOrder order)`

Puts the marker and the role in the places the language pack states.

## `internal void PSentenceFrameShow(CStateWording particle, CStateWording dependence)`

Takes the marker and the role from a later draft, only where the value changed.
A change also announces visibility, the written verdict and the gap, since all three read both fields.

## `public event PropertyChangedEventHandler? PropertyChanged;`

Announces each changed member, which the row re-raises as its own change.
