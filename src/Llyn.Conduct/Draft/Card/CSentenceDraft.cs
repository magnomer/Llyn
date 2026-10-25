namespace Llyn.Conduct;

public sealed record CSentenceDraft(
    long CSentenceDraftId,
    long CSentenceDraftLine,
    CExampleDraft? CSentenceDraftExample,
    bool CSentenceDraftCited,
    CStateWording CSentenceDraftText,
    CStateWording CSentenceDraftParticle,
    CStateWording CSentenceDraftDependence);
