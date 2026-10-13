using System;

namespace Llyn.UIDeportment;

[Flags]
internal enum QLookCue
{
    QLookCueBase = 0,

    QLookCueHover = 1 << 0,

    QLookCuePress = 1 << 1,

    QLookCueFocus = 1 << 2,

    QLookCueChecked = 1 << 3,

    QLookCueDisabled = 1 << 4,

    QLookCueHorizontal = 1 << 5,

    QLookCueEmpty = 1 << 6,

    QLookCueOpened = 1 << 7,

    QLookCueHighlight = 1 << 8,

    QLookCueSelected = 1 << 9,

    QLookCueDrag = 1 << 10,

    QLookCueBare = 1 << 11,

    QLookCueMute = 1 << 12,

    QLookCueChosen = 1 << 13,

    QLookCueIdle = 1 << 14,

    QLookCuePending = 1 << 15,

    QLookCueMarked = 1 << 16,

    QLookCueFaded = 1 << 17,

    QLookCueRefused = 1 << 18,

    QLookCueFetching = 1 << 19,

    QLookCuePlaying = 1 << 20,
}
