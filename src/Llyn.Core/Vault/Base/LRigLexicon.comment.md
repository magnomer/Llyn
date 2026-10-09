# LRigLexicon.cs
Hash: `eb91d47d19742e69`

## `public sealed record LRigLexicon(LEtymologyVault LRigLexiconEtymologies, LCollocationVault LRigLexiconCollocations, LGlossVault LRigLexiconGlosses, LInflectionVault LRigLexiconInflections, LLacunaVault LRigLexiconLacunae, LMeaningVault LRigLexiconMeanings, LMorphologyVault LRigLexiconMorphologies, LSpeechVault LRigLexiconSpeeches)`

The lexicon group of the rig, holding the ports that describe a word itself.
It covers etymologies, collocations, glosses, inflections, lacunae, meanings, morphologies and parts of speech.
`LRig` holds it as `LRigLexicon`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigLexicon{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactoryLexicon`, and a test builds it from fakes.
