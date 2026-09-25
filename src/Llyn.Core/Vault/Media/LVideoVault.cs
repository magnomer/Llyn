using System.Collections.Generic;

namespace Llyn.Core;

public interface LVideoVault
{
    LVideo LVideoCreate(LVideo video);

    LVideo? LVideoRead(long id);

    IReadOnlyList<LVideo> LVideoMeaningRead(long meaningId);

    IReadOnlyList<LVideo> LVideoCollocationRead(long collocationId);

    IReadOnlyList<LVideo> LVideoSituationRead(long situationId);

    void LVideoUpdate(LVideo video);

    void LVideoMeaningAttach(long meaningId, long videoId, int position);

    void LVideoCollocationAttach(long collocationId, long videoId, int position);

    void LVideoSituationAttach(long situationId, long videoId, int position);

    void LVideoMeaningDetach(long meaningId, long videoId);

    void LVideoCollocationDetach(long collocationId, long videoId);

    void LVideoSituationDetach(long situationId, long videoId);
}
