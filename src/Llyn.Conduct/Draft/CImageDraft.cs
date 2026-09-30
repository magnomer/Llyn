namespace Llyn.Conduct;

public sealed record CImageDraft(long CImageDraftId, CStateValue CImageDraftLocation, bool CImageDraftEmpty)
{
    public CStateWording CImageDraftWording =>
        CStateWording.LStateWordingRead(CImageDraftLocation, null, "Card.LocationHint");
}
