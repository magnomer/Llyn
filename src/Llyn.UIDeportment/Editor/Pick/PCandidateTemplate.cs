using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PCandidateTemplate : ResourceDictionary
{
    private readonly PEditor _pCandidateHost;

    internal PCandidateTemplate(PEditor host)
    {
        _pCandidateHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Pick/PCandidateTemplate.xaml", UriKind.Relative)));
    }

    internal void PCandidateHandle(object sender, MouseButtonEventArgs e)
    {
        _pCandidateHost.PCandidateHandle(sender, e);
    }
}
