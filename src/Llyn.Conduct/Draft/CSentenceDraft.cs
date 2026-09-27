namespace Llyn.Conduct;

public sealed record CSentenceDraft(
    long CSentenceDraftId,
    CExampleDraft? CSentenceDraftExample,
    CStateValue CSentenceDraftParticle,
    CStateValue CSentenceDraftDependence);
