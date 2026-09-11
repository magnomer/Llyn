namespace Llyn.Core;

public sealed record LCourtLink(
    long LCourtLinkId,
    long LCourtLinkOwner,
    long LCourtLinkTarget,
    string LCourtLinkHeadword,
    string LCourtLinkLanguage);
