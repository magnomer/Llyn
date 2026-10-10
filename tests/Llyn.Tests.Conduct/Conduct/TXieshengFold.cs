using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TXieshengFold
{
    [Fact]
    public async Task XieshengFoldToggle_ShownMember_StoresItsOwnStateAndRedrawsThePage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        LEntry entry = await TXiesheng.TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        xiesheng.TXieshengStemOpen(language, "龍");
        int changed = 0;
        xiesheng.CXieshengChanged += () => changed++;

        bool taken = xiesheng.CXieshengFoldToggle("龍", true);

        Assert.True(taken);
        Assert.True(changed > 0);
        Assert.True(Assert.Single(xiesheng.CXieshengStemRead().CStemPageMembers).CStemMemberOpened);
        Assert.False(engine.TEngineSpreadCheck(entry.LEntryId));
    }

    [Fact]
    public async Task XieshengChanged_EntryFoldNotice_StaysSilentWhileSeriesFoldRedraws()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        LEntry entry = await TXiesheng.TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);
        xiesheng.TXieshengStemOpen(language, "龍");
        int changed = 0;
        xiesheng.CXieshengChanged += () => changed++;

        engine.TEngineReflexSpread(entry.LEntryId, true);
        int unrelated = changed;
        xiesheng.CXieshengFoldToggle("龍", true);

        Assert.Equal(0, unrelated);
        Assert.True(changed > 0);
    }

    [Fact]
    public async Task XieshengFoldToggle_NoSeriesShown_AnswersFalseAndStoresNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXiesheng.TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier);

        bool taken = xiesheng.CXieshengFoldToggle("龍", true);

        Assert.False(taken);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM stem_fold;"));
    }

    [Fact]
    public async Task XieshengFoldToggle_FailingPort_ShowsTheNoticeAndAnswersFalse()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXiesheng.TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierFaultCreate(engine, "LStemPort.LEngineStemSpread", true);
        string language = pack.TLanguageFixtureName;
        await TXiesheng.TXieshengStemSave(engine, language);
        List<string> asked = [];
        CXiesheng xiesheng = TXiesheng.TXieshengPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        xiesheng.TXieshengStemOpen(language, "龍");

        bool taken = xiesheng.CXieshengFoldToggle("龍", true);

        Assert.False(taken);
        Assert.Equal(["Reflex.SpreadFailed"], asked);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM stem_fold;"));
    }

    [Fact]
    public void StemMemberRead_UnfoldedLanguageRows_FoldableWhileABareMemberIsNot()
    {
        LStemMember written = TInterface.TStemMemberCreate(
            "龍",
            [],
            [TInterface.TReflexDraftCreate("Korean", "", "룡")],
            [TInterface.TReflexGuiseCreate(false, false, false)]);

        IReadOnlyList<CStemMember> members = TInterfaceConductSound.TStemMemberRead(
            [written, TInterface.TStemMemberCreate("瀧", [], [])]);

        Assert.False(Assert.Single(members[0].CStemMemberReflexes).CReflexFolded);
        Assert.True(members[0].CStemMemberFoldable);
        Assert.False(members[1].CStemMemberFoldable);
    }
}
