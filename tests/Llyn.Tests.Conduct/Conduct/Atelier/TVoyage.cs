using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TVoyage
{
    [Fact]
    public void VoyageRead_NothingRecorded_LightsNeitherWay()
    {
        CVoyage voyage = new();

        Assert.Equal(new CVoyageState(false, false), voyage.TVoyageRead());
    }

    [Fact]
    public void VoyageStationAdd_EmptyOrStandingStation_RecordsNothingTwice()
    {
        CVoyage voyage = new();

        voyage.TVoyageStationAdd("Library", 0);
        voyage.TVoyageStationAdd("Library", 4);
        voyage.TVoyageStationAdd("Library", 4);
        List<(string, long)> shown = [];
        voyage.TVoyageUndo("Corpus", 9, TVoyageShowCreate(shown, true));

        Assert.Equal([("Library", 4L)], shown);
        Assert.Equal(new CVoyageState(false, true), voyage.TVoyageRead());
    }

    [Fact]
    public void VoyageUndo_ShownStation_ParksTheStandingStationForRedo()
    {
        CVoyage voyage = new();
        voyage.TVoyageStationAdd("Library", 4);
        List<(string, long)> shown = [];

        bool back = voyage.TVoyageUndo("Corpus", 9, TVoyageShowCreate(shown, true));
        bool forward = voyage.TVoyageRedo("Library", 4, TVoyageShowCreate(shown, true));

        Assert.True(back);
        Assert.True(forward);
        Assert.Equal([("Library", 4L), ("Corpus", 9L)], shown);
        Assert.Equal(new CVoyageState(true, false), voyage.TVoyageRead());
    }

    [Fact]
    public void VoyageUndo_RefusedJump_LeavesBothTrails()
    {
        CVoyage voyage = new();
        voyage.TVoyageStationAdd("Library", 4);

        bool back = voyage.TVoyageUndo("Corpus", 9, TVoyageShowCreate([], false));

        Assert.False(back);
        Assert.Equal(new CVoyageState(true, false), voyage.TVoyageRead());
    }

    [Fact]
    public void VoyageStationAdd_AfterUndo_ForgetsTheFuture()
    {
        CVoyage voyage = new();
        voyage.TVoyageStationAdd("Library", 4);
        voyage.TVoyageUndo("Corpus", 9, TVoyageShowCreate([], true));

        voyage.TVoyageStationAdd("Library", 5);

        Assert.Equal(new CVoyageState(true, false), voyage.TVoyageRead());
    }

    [Fact]
    public void VoyageStationAdd_PastTheCap_DropsTheOldestStation()
    {
        CVoyage voyage = new();
        for (long id = 1; id <= 51; id++)
        {
            voyage.TVoyageStationAdd("Library", id);
        }

        List<(string, long)> shown = [];
        while (voyage.TVoyageUndo("Corpus", 99, TVoyageShowCreate(shown, true)))
        {
        }

        Assert.Equal(50, shown.Count);
        Assert.Equal(("Library", 2L), shown[^1]);
    }

    private static Func<string, long, bool> TVoyageShowCreate(List<(string, long)> shown, bool accepted)
    {
        return (tab, id) =>
        {
            shown.Add((tab, id));
            return accepted;
        };
    }
}
