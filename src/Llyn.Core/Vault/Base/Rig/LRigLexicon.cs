namespace Llyn.Core;

public sealed record LRigLexicon(
    LAncestorVault LRigLexiconAncestors,
    LCollocationVault LRigLexiconCollocations,
    LGlossVault LRigLexiconGlosses,
    LInflectionVault LRigLexiconInflections,
    LLacunaVault LRigLexiconLacunae,
    LMeaningVault LRigLexiconMeanings,
    LMorphologyVault LRigLexiconMorphologies,
    LSpeechVault LRigLexiconSpeeches);
