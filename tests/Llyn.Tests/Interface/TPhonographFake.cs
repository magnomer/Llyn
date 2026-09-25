using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPhonographFake : LPhonograph
{
    public List<string> TPhonographFakePlayed { get; } = [];

    public void LPhonographPlay(string file)
    {
        TPhonographFakePlayed.Add(file);
    }

    public void LPhonographStop()
    {
    }

    public void LPhonographVolumeSet(double volume)
    {
    }
}
