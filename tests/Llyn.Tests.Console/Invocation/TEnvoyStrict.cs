using Llyn.Conduct;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TEnvoyStrict
{
    [Fact]
    public void EnvoyConfirm_Yes_AnswersYesUnasked()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-version", "-yes"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        bool answer = envoy.CEnvoyConfirm("Scribe.DeleteConfirm");

        Assert.True(answer);
        Assert.Empty(console.TConsoleFakeError);
        Assert.False(envoy.BEnvoyStrictRefusal);
    }

    [Fact]
    public void EnvoyConfirm_No_RefusesTheRun()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeAnswer = "y" };
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-no", "-version"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        bool answer = envoy.CEnvoyConfirm("Scribe.DeleteConfirm");

        Assert.False(answer);
        Assert.Empty(console.TConsoleFakeError);
        Assert.True(envoy.BEnvoyStrictRefusal);
    }

    [Fact]
    public void EnvoyConfirm_TerminalLine_AsksOnStandardError()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeAnswer = " Y " };
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-version"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        bool answer = envoy.CEnvoyConfirm("Scribe.DeleteConfirm");

        Assert.True(answer);
        Assert.Equal(["llyn: Delete this entry? It cannot be undone. [y/N]"], console.TConsoleFakeError);
        Assert.Empty(console.TConsoleFakeOutput);
        Assert.False(envoy.BEnvoyStrictRefusal);
    }

    [Fact]
    public void EnvoyConfirm_NoTerminal_AnswersNoUnasked()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = false, TConsoleFakeAnswer = "y" };
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-version"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        bool answer = envoy.CEnvoyConfirm("Scribe.DeleteConfirm");

        Assert.False(answer);
        Assert.Empty(console.TConsoleFakeError);
        Assert.True(envoy.BEnvoyStrictRefusal);
    }

    [Fact]
    public void EnvoyLeaveConfirm_NoOption_StaysAndRefuses()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeAnswer = "y" };
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-version"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        bool? answer = envoy.CEnvoyLeaveConfirm();

        Assert.Null(answer);
        Assert.True(envoy.BEnvoyStrictRefusal);
    }

    [Fact]
    public void EnvoyFailureShow_Key_PrintsTheSentence()
    {
        TConsoleFake console = new();
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-version", "-quiet"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        envoy.CEnvoyFailureShow("Workspace.OpenFailed");

        Assert.Equal(["llyn: The workspace could not be opened."], console.TConsoleFakeError);
        Assert.True(envoy.BEnvoyStrictRefusal);
    }

    [Fact]
    public void EnvoyOmissionShow_Quiet_PrintsNothing()
    {
        TConsoleFake console = new();
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BInvocation invocation = BInvocation.BInvocationParse(["-version", "-quiet"], catalog, out _)!;
        BEnvoyStrict envoy = new(console, catalog, invocation);

        envoy.CEnvoyOmissionShow([new CMarkupOmission("12", "The element holds no text.")]);

        Assert.Empty(console.TConsoleFakeError);
        Assert.False(envoy.BEnvoyStrictRefusal);
    }
}
