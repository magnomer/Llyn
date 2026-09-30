namespace Llyn.Conduct;

public sealed record CSentenceDraft(
    long CSentenceDraftId,
    CExampleDraft? CSentenceDraftExample,
    CStateWording CSentenceDraftText,
    CStateWording CSentenceDraftParticle,
    CStateWording CSentenceDraftDependence);
