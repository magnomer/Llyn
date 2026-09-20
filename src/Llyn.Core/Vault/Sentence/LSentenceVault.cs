using System.Collections.Generic;

namespace Llyn.Core;

public interface LSentenceVault
{
    IReadOnlyList<LSentence> LSentenceMeaningRead(long meaningId);

    IReadOnlyList<LSentence> LSentenceCollocationRead(long collocationId);

    IReadOnlyList<long> LSentenceMeaningSave(long meaningId, IReadOnlyList<LSentence> sentences);

    IReadOnlyList<long> LSentenceCollocationSave(long collocationId, IReadOnlyList<LSentence> sentences);

    void LSentenceMeaningAttach(long meaningId, long exampleId, int position);

    void LSentenceCollocationAttach(long collocationId, long exampleId, int position);

    void LSentenceMeaningDetach(long meaningId, long exampleId);

    void LSentenceCollocationDetach(long collocationId, long exampleId);

    IReadOnlyList<string> LSentenceParticleRead(string language);

    IReadOnlyList<string> LSentenceDependenceRead(string language);

    LSentenceOrder LSentenceLoad(string language);
}
