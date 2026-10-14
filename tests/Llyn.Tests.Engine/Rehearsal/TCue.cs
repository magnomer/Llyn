using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TCue
{
    [Fact]
    public void CueOrphaned_MeaningGrainNullSense_ReturnsTrue()
    {
        LCue cue = TInterface.TCueCreate(LGrain.LGrainMeaning, null, null, null, null, null);

        Assert.True(cue.LCueOrphaned);
        Assert.True(cue.LCueResting);
        Assert.False(TInterface.TCueCreate(LGrain.LGrainMeaning, null, 7, null, null, null).LCueOrphaned);
    }

    [Fact]
    public void CueOrphaned_FreeGrain_ReturnsFalse()
    {
        LCue cue = TInterface.TCueCreate(LGrain.LGrainFree, null, null, null, null, null);

        Assert.False(cue.LCueOrphaned);
        Assert.False(cue.LCueResting);
        Assert.True((cue with { LCueExcluded = true }).LCueResting);
    }

    [Fact]
    public void CueOrphaned_PairNullTarget_ReturnsTrue()
    {
        Assert.True(TInterface.TCueCreate(LGrain.LGrainPair, null, 7, null, null, null).LCueOrphaned);
        Assert.False(TInterface.TCueCreate(LGrain.LGrainPair, null, 7, null, null, 9).LCueOrphaned);
        Assert.False(TInterface.TCueCreate(LGrain.LGrainPair, null, null, 8, null, 9).LCueOrphaned);
    }

    [Fact]
    public void CueOrphaned_PairBothSourcesNull_ReturnsTrue()
    {
        Assert.True(TInterface.TCueCreate(LGrain.LGrainPair, null, null, null, null, 9).LCueOrphaned);
    }
}
