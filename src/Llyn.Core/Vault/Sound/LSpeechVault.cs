using System.Collections.Generic;

namespace Llyn.Core;

public interface LSpeechVault
{
    LSpeechValue LSpeechValueCreate(LSpeechValue value);

    LSpeechValue? LSpeechValueRead(long id);

    IReadOnlyList<LSpeechValue> LSpeechValueRead(string language);

    LSpeechValue? LSpeechValueFind(string language, string name);

    LSpeechPack LSpeechLoad(string language);
}
