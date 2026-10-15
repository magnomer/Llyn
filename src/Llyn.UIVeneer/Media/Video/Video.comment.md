# Video

This folder holds the video row in its two forms: the read-only card row and the writing row.
The writing row is merged by both the entry editor and the situation editor, and each fills the named parts.

## `PDisplayVideo.xaml`

`Display.Card.Video` is folded away by the look sheet while the card carries no video.
`Display.Card.VideoLine` plays a clip in the same screen the writing side embeds, in the frame a picture takes.
The frame turns the engine's row into a video, and the line fill sets the screen from it.

## `PVideoTemplate.xaml`

`Theme.Video.Preview` is the frame around the Screen, and the fill hides it while the location names nothing to play.
`Theme.Video.Row` holds the Screen, the location and timestamp fields with a browse button, and the remove handle in the gutter.
The two fields are named after the values they show, so a text handler knows which one it heard.
The handle panel starts hidden and shows while the pointer or the focus is inside the list.

## Stamp

Commit: `2f9a06b8eb95`
- `PDisplayVideo.xaml` `db7051c75d922d0c`
- `PVideoTemplate.xaml` `730cd4c8fe81dee8`
