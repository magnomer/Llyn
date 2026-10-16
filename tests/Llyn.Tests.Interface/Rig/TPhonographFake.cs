using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPhonographFake : LPhonograph
{
    public event Action? LPhonographFinished;

    public List<string> TPhonographFakePlayed { get; } = [];

    public int TPhonographFakeStopped { get; private set; }

    public double TPhonographFakeVolume { get; private set; } = -1;

    public void LPhonographPlay(string file)
    {
        TPhonographFakePlayed.Add(file);
    }

    public void LPhonographStop()
    {
        TPhonographFakeStopped++;
    }

    public void LPhonographVolumeSet(double volume)
    {
        TPhonographFakeVolume = volume;
    }

    public void TPhonographFakeFinish()
    {
        LPhonographFinished?.Invoke();
    }
}
