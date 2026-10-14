using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TAncestorClerk
{
    [Fact]
    public void AncestorRelationRead_CustomRelation_ListsBuiltinsFirst()
    {
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(TAncestorRigCreate());
        clerk.TAncestorRelationAdd("cognate");

        IReadOnlyList<LAncestorRelation> relations = clerk.TAncestorRelationRead();

        Assert.Equal(
            ["Relation.Borrowed", "Relation.Compound", "Relation.Derived", "Relation.Calque", "Relation.Blend", ""],
            relations.Select(relation => relation.LAncestorRelationKey));
    }

    [Fact]
    public void AncestorRelationCreate_TypedName_KeepsNameAsTyped()
    {
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(TAncestorRigCreate());

        LAncestorRelation created = clerk.TAncestorRelationAdd("  Folk etymology ");

        Assert.Equal("Folk etymology", created.LAncestorRelationName);
        Assert.Equal(string.Empty, created.LAncestorRelationKey);
    }

    [Fact]
    public void AncestorRelationCreate_BuiltinName_ReturnsBuiltin()
    {
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(TAncestorRigCreate());

        LAncestorRelation found = clerk.TAncestorRelationAdd("Borrowed");

        Assert.True(found.LAncestorRelationBuiltin);
        Assert.Equal(5, clerk.TAncestorRelationRead().Count);
    }

    [Fact]
    public void AncestorRelationRead_UsedRelation_CountsBoxes()
    {
        LRig rig = TAncestorRigCreate();
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(rig);
        long relation = clerk.TAncestorRelationAdd("cognate").LAncestorRelationId;
        rig.TAncestorMapSave(1, TInterface.TAncestorBoxCreate(1, null, "a") with { LAncestorRelationId = relation });
        rig.TAncestorMapSave(2, TInterface.TAncestorBoxCreate(1, null, "b") with { LAncestorRelationId = relation });

        LAncestorRelation read = clerk.TAncestorRelationRead().Single(held => held.LAncestorRelationId == relation);

        Assert.Equal(2, read.LAncestorRelationUsage);
    }

    [Fact]
    public void AncestorRelationDelete_UsedRelation_ReturnsUsageCount()
    {
        LRig rig = TAncestorRigCreate();
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(rig);
        long relation = clerk.TAncestorRelationAdd("cognate").LAncestorRelationId;
        rig.TAncestorMapSave(1, TInterface.TAncestorBoxCreate(1, null, "a") with { LAncestorRelationId = relation });

        int? cleared = clerk.TAncestorRelationDelete(relation);

        Assert.Equal(1, cleared);
        Assert.DoesNotContain(clerk.TAncestorRelationRead(), held => held.LAncestorRelationId == relation);
        Assert.Null(Assert.Single(rig.TAncestorMapRead(1)).LAncestorRelationId);
    }

    [Fact]
    public void AncestorRelationDelete_BuiltinRelation_Refuses()
    {
        LRig rig = TAncestorRigCreate();
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(rig);
        long relation = clerk.TAncestorRelationRead()[0].LAncestorRelationId;
        rig.TAncestorMapSave(1, TInterface.TAncestorBoxCreate(1, null, "a") with { LAncestorRelationId = relation });

        int? cleared = clerk.TAncestorRelationDelete(relation);

        Assert.Null(cleared);
        Assert.Contains(clerk.TAncestorRelationRead(), held => held.LAncestorRelationId == relation);
        Assert.Equal(relation, Assert.Single(rig.TAncestorMapRead(1)).LAncestorRelationId);
    }

    [Fact]
    public void AncestorLanguageRead_AddedTongue_ListsAppFirst()
    {
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(TAncestorRigCreate());
        clerk.TAncestorTongueAdd("Old Norse");

        IReadOnlyList<LAncestorLanguage> languages = clerk.TAncestorLanguageRead();

        Assert.Equal(["English", "Korean", "Old Norse"], languages.Select(language => language.LAncestorLanguageName));
        Assert.Equal([false, false, true], languages.Select(language => language.LAncestorLanguageTongue is not null));
    }

    [Fact]
    public void AncestorTongueCreate_AppLanguageName_ReturnsAppLanguage()
    {
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(TAncestorRigCreate());

        LAncestorLanguage found = clerk.TAncestorTongueAdd(" english ");

        Assert.Equal("English", found.LAncestorLanguageName);
        Assert.Null(found.LAncestorLanguageTongue);
        Assert.Equal(2, clerk.TAncestorLanguageRead().Count);
    }

    [Fact]
    public void AncestorLanguageRead_UsedTongue_CountsBoxes()
    {
        LRig rig = TAncestorRigCreate();
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(rig);
        long tongue = clerk.TAncestorTongueAdd("Old Norse").LAncestorLanguageTongue ?? 0;
        rig.TAncestorMapSave(
            1,
            TInterface.TAncestorBoxCreate(1, null, "a") with { LAncestorTongueId = tongue },
            TInterface.TAncestorBoxCreate(2, null, "b") with { LAncestorTongueId = tongue });

        LAncestorLanguage read = clerk.TAncestorLanguageRead().Single(held => held.LAncestorLanguageTongue == tongue);

        Assert.Equal(2, read.LAncestorLanguageUsage);
    }

    [Fact]
    public void AncestorTongueDelete_UsedTongue_ReturnsUsageCount()
    {
        LRig rig = TAncestorRigCreate();
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(rig);
        long tongue = clerk.TAncestorTongueAdd("Old Norse").LAncestorLanguageTongue ?? 0;
        rig.TAncestorMapSave(1, TInterface.TAncestorBoxCreate(1, null, "a") with { LAncestorTongueId = tongue });

        int? cleared = clerk.TAncestorTongueDelete(tongue);

        Assert.Equal(1, cleared);
        Assert.Equal(2, clerk.TAncestorLanguageRead().Count);
        Assert.Null(Assert.Single(rig.TAncestorMapRead(1)).LAncestorTongueId);
    }

    [Fact]
    public void AncestorTongueDelete_MissingTongue_Refuses()
    {
        LAncestorClerk clerk = TInterface.TAncestorClerkCreate(TAncestorRigCreate());

        Assert.Null(clerk.TAncestorTongueDelete(404));
    }

    private static LRig TAncestorRigCreate() =>
        TInterface.TRigClerkCreate(new TVaultFake()) with { LRigLanguages = new TLanguageListed() };

    private sealed class TLanguageListed : LLanguageVault
    {
        public IReadOnlyList<string> LLanguageScan() => ["English", "Korean"];

        public LLanguage LLanguageRead(string language) => throw new NotSupportedException();

        public bool LLanguageNameValidate(string? language) => true;

        public Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation) =>
            Task.FromResult<string?>(null);

        public string? LLanguageFlagFind(string code) => null;
    }
}
