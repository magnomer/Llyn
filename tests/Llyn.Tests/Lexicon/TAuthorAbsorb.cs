using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAuthorAbsorb
{
    [Fact]
    public void AuthorAbsorb_CreditedElsewhere_MovesCreditsAndDeletesDropped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TAuthorWorkCreate(engine, "A Dictionary");
        LReference grammar = TAuthorWorkCreate(engine, "A Grammar");

        LAuthor kept = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        LAuthor dropped = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim M."));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));

        engine.TEngineAuthorAttach(dictionary.LReferenceId, lee.LAuthorId, 0);
        engine.TEngineAuthorAttach(dictionary.LReferenceId, dropped.LAuthorId, 1);
        engine.TEngineAuthorAttach(grammar.LReferenceId, kept.LAuthorId, 0);

        engine.TEngineAuthorAbsorb(kept.LAuthorId, dropped.LAuthorId);

        Assert.Null(engine.TEngineAuthorRead(dropped.LAuthorId));
        Assert.Equal(
            ["Lee", "Kim"],
            engine.TEngineAuthorRead(dictionary.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));
        Assert.Equal(
            ["Kim"],
            engine.TEngineAuthorRead(grammar.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));
    }

    [Fact]
    public void AuthorAbsorb_BothCreditedOnOneSource_KeepsOneCreditInPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference dictionary = TAuthorWorkCreate(engine, "A Dictionary");

        LAuthor dropped = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim M."));
        LAuthor lee = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Lee"));
        LAuthor kept = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));

        engine.TEngineAuthorAttach(dictionary.LReferenceId, dropped.LAuthorId, 0);
        engine.TEngineAuthorAttach(dictionary.LReferenceId, lee.LAuthorId, 1);
        engine.TEngineAuthorAttach(dictionary.LReferenceId, kept.LAuthorId, 2);

        engine.TEngineAuthorAbsorb(kept.LAuthorId, dropped.LAuthorId);

        Assert.Equal(
            ["Kim", "Lee"],
            engine.TEngineAuthorRead(dictionary.LReferenceId, LOwner.LOwnerReference)
                .Select(author => author.LAuthorName));
    }

    [Fact]
    public void AuthorAbsorb_SameOrMissingAuthor_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));

        Assert.Throws<InvalidOperationException>(() => engine.TEngineAuthorAbsorb(kim.LAuthorId, kim.LAuthorId));
        Assert.Throws<InvalidOperationException>(() => engine.TEngineAuthorAbsorb(kim.LAuthorId, kim.LAuthorId + 100));
        Assert.NotNull(engine.TEngineAuthorRead(kim.LAuthorId));
    }

    private static LReference TAuthorWorkCreate(LEngine engine, string title)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
    }
}
