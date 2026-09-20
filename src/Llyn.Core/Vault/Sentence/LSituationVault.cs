using System.Collections.Generic;

namespace Llyn.Core;

public interface LSituationVault
{
    LSituation LSituationCreate(LSituation situation);

    LSituation? LSituationRead(long id);

    IReadOnlyList<LSituation> LSituationRead();

    IReadOnlyList<LSituation> LSituationMeaningRead(long meaningId);

    IReadOnlyList<LSituation> LSituationCollocationRead(long collocationId);

    void LSituationUpdate(LSituation situation);

    int LSituationReferenceRead(long id);

    IReadOnlyDictionary<long, int> LSituationReferenceRead();

    IReadOnlyList<LUsage> LSituationUsageRead(long id);

    void LSituationDelete(long id);

    void LSituationDelete(long id, bool detach);

    void LSituationMeaningAttach(long meaningId, long situationId, int position);

    void LSituationCollocationAttach(long collocationId, long situationId, int position);

    void LSituationMeaningDetach(long meaningId, long situationId);

    void LSituationCollocationDetach(long collocationId, long situationId);
}
