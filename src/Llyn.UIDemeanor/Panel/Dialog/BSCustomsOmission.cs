using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSCustomsOmission : BSParleyBody
{
    private readonly IReadOnlyList<CMarkupOmission> _bsCustomsOmissionList;
    private readonly string _bsCustomsOmissionHeading;
    private readonly BTable _bsCustomsOmissionTable;

    internal BSCustomsOmission(IReadOnlyList<CMarkupOmission> omissions, string heading)
    {
        ArgumentNullException.ThrowIfNull(omissions);
        ArgumentNullException.ThrowIfNull(heading);

        _bsCustomsOmissionList = omissions;
        _bsCustomsOmissionHeading = heading;
        _bsCustomsOmissionTable = new BTable(omissions.Count, BSCustoms.BSCustomsHeight - 2, []);
    }

    public int BSParleyBodyWidth => BSCustoms.BSCustomsWidth;

    public int BSParleyBodyHeight => BSCustoms.BSCustomsHeight;

    public string BSParleyBodyPosition => string.Empty;

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        return _bsCustomsOmissionTable.BTableObserve(key);
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        yield return new BCanvasText(row, column, width, _bsCustomsOmissionHeading, role);
        foreach (int index in _bsCustomsOmissionTable.BTableRowShown)
        {
            CMarkupOmission omission = _bsCustomsOmissionList[index];
            yield return new BCanvasText(
                row + 2 + index - _bsCustomsOmissionTable.BTableTop,
                column,
                width,
                $"{omission.CMarkupOmissionLine,6}  {omission.CMarkupOmissionText}",
                role);
        }
    }
}
