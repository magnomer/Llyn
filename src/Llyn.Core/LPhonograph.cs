namespace Llyn.Core;

public interface LPhonograph
{
    void LPhonographPlay(string file);

    void LPhonographStop();

    void LPhonographVolumeSet(double volume);
}
