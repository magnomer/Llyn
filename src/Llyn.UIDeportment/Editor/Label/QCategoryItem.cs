namespace Llyn.UIDeportment;

internal sealed class QCategoryItem
{
    internal QCategoryItem(string name, bool taken)
    {
        QCategoryItemName = name;
        QCategoryItemTaken = taken;
    }

    public string QCategoryItemName { get; }

    public bool QCategoryItemTaken { get; }
}
