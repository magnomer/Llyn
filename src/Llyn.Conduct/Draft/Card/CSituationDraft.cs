using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSituationDraft(
    long CSituationDraftId,
    CStateValue CSituationDraftTitle,
    CStateValue CSituationDraftKind,
    CStateValue CSituationDraftDescription,
    IReadOnlyList<CImageDraft> CSituationDraftImage,
    IReadOnlyList<CVideoDraft> CSituationDraftVideo)
{
    public CStateWording CSituationDraftWording => CStateWording.LStateWordingRead(CSituationDraftTitle, null);
}
