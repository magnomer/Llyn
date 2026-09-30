using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QArticulation
{
    private void QVowelIntroduce()
    {
        CArticulation chart = CCatalog.CCatalogVowelRead();
        QArticulationTableBuild(QVowel, chart.CArticulationHeaders.Count, chart.CArticulationSides.Count);

        for (int column = 0; column < chart.CArticulationHeaders.Count; column++)
        {
            QArticulationHeaderPlace(QVowel, chart.CArticulationHeaders[column], column);
        }

        for (int row = 0; row < chart.CArticulationSides.Count; row++)
        {
            QArticulationSidePlace(QVowel, chart.CArticulationSides[row], row);

            for (int column = 0; column < chart.CArticulationCells[row].Count; column++)
            {
                QArticulationCellPlace(QVowel, chart.CArticulationCells[row][column], column, row);
            }
        }
    }
}
