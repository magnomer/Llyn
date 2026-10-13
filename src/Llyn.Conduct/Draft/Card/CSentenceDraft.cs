namespace Llyn.Conduct;

public sealed record CSentenceDraft(
    long CSentenceDraftId,
    CExampleDraft? CSentenceDraftExample,
    bool CSentenceDraftCited,
    CStateWording CSentenceDraftText,
    CStateWording CSentenceDraftParticle,
    CStateWording CSentenceDraftDependence);
