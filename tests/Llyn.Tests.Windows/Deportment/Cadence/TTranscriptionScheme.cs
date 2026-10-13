using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TTranscriptionScheme
{
    [Fact]
    public void TranscriptionSchemeRefine_Reordered_PairsByName()
    {
        QTranscriptionItem row = new(1, "Alpha", string.Empty);
        row.TTranscriptionSchemeRefine([new CScheme("Alpha", true), new CScheme("Beta", false)]);

        row.TTranscriptionSchemeRefine([new CScheme("Beta", true), new CScheme("Alpha", false)]);
        List<QTranscriptionChoice> reordered = [.. row.QTranscriptionItemChoice];
        bool[] taken = [reordered[0].QTranscriptionChoiceTaken, reordered[1].QTranscriptionChoiceTaken];
        row.TTranscriptionSchemeRefine([new CScheme("Beta", false), new CScheme("Alpha", true)]);

        Assert.Equal(2, reordered.Count);
        Assert.Equal("Beta", reordered[0].QTranscriptionChoiceScheme);
        Assert.Equal("Beta", reordered[0].QTranscriptionChoiceLabel);
        Assert.Equal("Alpha", reordered[1].QTranscriptionChoiceScheme);
        Assert.Equal("Alpha", reordered[1].QTranscriptionChoiceLabel);
        Assert.Equal([true, false], taken);
        Assert.Same(reordered[0], row.QTranscriptionItemChoice[0]);
        Assert.Same(reordered[1], row.QTranscriptionItemChoice[1]);
        Assert.False(row.QTranscriptionItemChoice[0].QTranscriptionChoiceTaken);
        Assert.True(row.QTranscriptionItemChoice[1].QTranscriptionChoiceTaken);
    }
}
