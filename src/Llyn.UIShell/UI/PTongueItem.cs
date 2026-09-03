using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PTongueItem
{
    internal PTongueItem(string name, ImageSource? flag)
    {
        PTongueItemName = name;
        PTongueItemFlag = flag;
    }

    public string PTongueItemName { get; }

    public ImageSource? PTongueItemFlag { get; }
}
