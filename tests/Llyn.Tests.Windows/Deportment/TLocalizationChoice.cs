using System.Collections.Generic;
using System.Windows.Controls;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLocalizationChoice
{
    [Fact]
    public void LocalizationRefine_RepeatedRefine_SavesNoLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        string stored = engine.TEngineLocalizationRead();
        string wanted = stored == "ko" ? "en" : "ko";
        KeyValuePair<string, string>[] languages = [new("en", "English"), new("ko", "Korean")];
        object? shown = null;

        TWindow.TWindowRun(() =>
        {
            ComboBox choice = new() { Name = "PLocalization" };
            StackPanel settings = new();
            settings.Children.Add(choice);
            object localization = TInterfaceDeportment.TLocalizationCreate(settings, atelier.CAtelierLedger, envoy);

            TInterfaceDeportment.TLocalizationRefine(localization, languages, wanted);
            TInterfaceDeportment.TLocalizationRefine(localization, languages, wanted);
            shown = choice.SelectedValue;
        });

        Assert.Equal(wanted, shown);
        Assert.Empty(asked);
        Assert.Equal(stored, engine.TEngineLocalizationRead());
    }
}
