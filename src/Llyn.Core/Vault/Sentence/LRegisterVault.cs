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

    IReadOnlyDictionary<long, int> LRegisterReferenceRead();

    void LRegisterMeaningAttach(long meaningId, long registerId, int position);

    void LRegisterCollocationAttach(long collocationId, long registerId, int position);

    void LRegisterMeaningDetach(long meaningId, long registerId);

    void LRegisterCollocationDetach(long collocationId, long registerId);

    IReadOnlyList<LRegister> LRegisterLoad(string language);
}
