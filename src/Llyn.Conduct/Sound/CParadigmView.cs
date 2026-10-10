using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmView(CParadigmTable CParadigmViewCollapsed, CParadigmTable CParadigmViewExpanded)
{
    internal static CParadigmView? CParadigmViewCreate(LParadigmView? view)
    {
        return view is null
            ? null
            : new CParadigmView(
                CParadigmTable.CParadigmTableCreate(view.LParadigmViewCollapsed),
                CParadigmTable.CParadigmTableCreate(view.LParadigmViewExpanded));
    }
}
