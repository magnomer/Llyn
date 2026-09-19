using System.Collections.Generic;

namespace Llyn.Core;

public interface LImageVault
{
    LImage LImageCreate(LImage image);

    LImage? LImageRead(long id);

    IReadOnlyList<LImage> LImageMeaningRead(long meaningId);

    IReadOnlyList<LImage> LImageCollocationRead(long collocationId);

    IReadOnlyList<LImage> LImageSituationRead(long situationId);

    void LImageUpdate(LImage image);

    void LImageMeaningAttach(long meaningId, long imageId, int position);

    void LImageCollocationAttach(long collocationId, long imageId, int position);

    void LImageSituationAttach(long situationId, long imageId, int position);

    void LImageMeaningDetach(long meaningId, long imageId);

    void LImageCollocationDetach(long collocationId, long imageId);

    void LImageSituationDetach(long situationId, long imageId);
}
