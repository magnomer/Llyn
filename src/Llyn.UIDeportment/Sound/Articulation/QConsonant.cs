using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QArticulation
{
    private void QConsonantIntroduce()
    {
        CArticulation chart = CCatalog.CCatalogConsonantRead();
        QArticulationTableBuild(QConsonant, chart.CArticulationHeaders.Count, chart.CArticulationSides.Count);

        for (int column = 0; column < chart.CArticulationHeaders.Count; column++)
        {
            QArticulationHeaderPlace(QConsonant, chart.CArticulationHeaders[column], column);
        }

        for (int row = 0; row < chart.CArticulationSides.Count; row++)
        {
            QArticulationSidePlace(QConsonant, chart.CArticulationSides[row], row);

            for (int column = 0; column < chart.CArticulationCells[row].Count; column++)
            {
                QArticulationCellPlace(QConsonant, chart.CArticulationCells[row][column], column, row);
            }
        }
    }
}
