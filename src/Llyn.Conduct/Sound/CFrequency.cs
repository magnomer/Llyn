namespace Llyn.Conduct;

public sealed record CFrequency(
    int CFrequencyBand,
    string CFrequencySource,
    CFrequencyTier CFrequencyRank,
    int CFrequencySpare,
    bool CFrequencyRanked)
{
    public string CFrequencyKey => CFrequencyRank switch
    {
        CFrequencyTier.CFrequencyTierCore => "Frequency.Core",
        CFrequencyTier.CFrequencyTierEveryday => "Frequency.Everyday",
        CFrequencyTier.CFrequencyTierAdvanced => "Frequency.Advanced",
        CFrequencyTier.CFrequencyTierRare => "Frequency.Rare",
        _ => "Frequency.Unknown",
    };
}
