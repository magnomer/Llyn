namespace Llyn.UIShell;

internal sealed class PSpeechItem
{
    internal PSpeechItem(string name)
    {
        PSpeechItemName = name;
    }

    public string PSpeechItemName { get; }
}
