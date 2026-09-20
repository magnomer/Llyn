using System.Collections.Generic;

namespace Llyn.Core;

public interface LRegisterVault
{
    LRegister LRegisterCreate(LRegister register);

    void LRegisterDefaultCreate(IReadOnlyList<LRegister> registers);

    LRegister? LRegisterRead(long id);

    IReadOnlyList<LRegister> LRegisterRead();

    IReadOnlyList<LRegister> LRegisterMeaningRead(long meaningId);

    IReadOnlyList<LRegister> LRegisterCollocationRead(long collocationId);

    void LRegisterNameUpdate(long registerId, LStateValue name);

    int LRegisterReferenceRead(long id);

    IReadOnlyDictionary<long, int> LRegisterReferenceRead();

    void LRegisterDelete(long id);

    void LRegisterDelete(long id, bool detach);

    void LRegisterMeaningAttach(long meaningId, long registerId, int position);

    void LRegisterCollocationAttach(long collocationId, long registerId, int position);

    void LRegisterMeaningDetach(long meaningId, long registerId);

    void LRegisterCollocationDetach(long collocationId, long registerId);

    IReadOnlyList<LRegister> LRegisterLoad(string language);
}
