using System;

namespace Llyn.Core;

public interface LPhonograph
{
    event Action? LPhonographFinished;

    void LPhonographPlay(string file);

    void LPhonographStop();

    void LPhonographVolumeSet(double volume);
}
