namespace Llyn.Conduct;

public sealed record CFrequency(
    int CFrequencyBand,
    string CFrequencySource,
    string CFrequencyRank,
    int CFrequencySpare,
    bool CFrequencyRanked)
{
    public string CFrequencyKey => "Frequency." + CFrequencyRank;
}
