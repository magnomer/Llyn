using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PNotationTemplate : ResourceDictionary
{
    internal PNotationTemplate()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Transcription/PNotationTemplate.xaml", UriKind.Relative)));
    }
}
