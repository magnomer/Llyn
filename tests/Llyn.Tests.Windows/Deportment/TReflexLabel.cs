using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflexLabel
{
    [Fact]
    public void ReflexTypeRefine_TypedNames_LabelsTheNewNames()
    {
        List<string> shown = [];

        TWindow.TWindowRun(() =>
        {
            System.Windows.Application application =
                System.Windows.Application.Current ?? new System.Windows.Application();
            application.Resources["Reflex.Cantonese"] = "Yue";
            application.Resources["Reflex.Kan-on"] = "Han";
            try
            {
                QReflexItem row = new(new CReflex(
                    1, "Cantonese", "Kan-on", "ipa", string.Empty, string.Empty, string.Empty, false, string.Empty, [],
                    new CRespellingMark(false, string.Empty, string.Empty), false, true));
                shown.Add(row.QReflexItemLabel);
                shown.Add(row.QReflexItemTag);

                TInterfaceDeportment.TReflexTypeRefine(row, CReflexField.CReflexFieldLanguage, "Wu");
                TInterfaceDeportment.TReflexTypeRefine(row, CReflexField.CReflexFieldKind, "Go-on");
                shown.Add(row.QReflexItemLabel);
                shown.Add(row.QReflexItemTag);
            }
            finally
            {
                application.Resources.Remove("Reflex.Cantonese");
                application.Resources.Remove("Reflex.Kan-on");
            }
        });

        Assert.Equal(["Yue", "Han", "Wu", "Go-on"], shown);
    }
}
