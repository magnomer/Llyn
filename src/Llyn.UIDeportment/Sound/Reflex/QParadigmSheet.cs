using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmSheet
{
    private QParadigmSheet(QParadigmTable collapsed, QParadigmTable expanded)
    {
        QParadigmSheetCollapsed = collapsed;
        QParadigmSheetExpanded = expanded;
    }

    public QParadigmTable QParadigmSheetCollapsed { get; }

    public QParadigmTable QParadigmSheetExpanded { get; }

    internal static QParadigmSheet? QParadigmSheetCreate(CParadigmView? view)
    {
        return view is null
            ? null
            : new QParadigmSheet(
                QParadigmTable.QParadigmTableCreate(view.CParadigmViewCollapsed),
                QParadigmTable.QParadigmTableCreate(view.CParadigmViewExpanded));
    }
}
