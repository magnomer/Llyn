namespace Llyn.Core;

public sealed record LCourtLink(
    string LCourtLinkId,
    string LCourtLinkOwner,
    string LCourtLinkTarget,
    string LCourtLinkHeadword,
    string LCourtLinkLanguage);
