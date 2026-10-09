namespace Llyn.Core;

public sealed record LRigLexicon(
    LEtymologyVault LRigLexiconEtymologies,
    LCollocationVault LRigLexiconCollocations,
    LGlossVault LRigLexiconGlosses,
    LInflectionVault LRigLexiconInflections,
    LLacunaVault LRigLexiconLacunae,
    LMeaningVault LRigLexiconMeanings,
    LMorphologyVault LRigLexiconMorphologies,
    LSpeechVault LRigLexiconSpeeches);
