using System;

namespace Llyn.Conduct;

public sealed record CVideoDraft(
    long CVideoDraftId,
    CStateValue CVideoDraftLocation,
    CStateValue CVideoDraftSpan,
    bool CVideoDraftEmpty,
    CScreen? CVideoDraftScreen,
    TimeSpan CVideoDraftFrom,
    TimeSpan? CVideoDraftUntil);
