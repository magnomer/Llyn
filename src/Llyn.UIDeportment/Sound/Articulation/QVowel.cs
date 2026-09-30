namespace Llyn.UIDeportment;

internal sealed partial class QArticulation
{
    private static readonly string[] QVowelBackness =
    [
        "Articulation.Front", "Articulation.Central", "Articulation.Back"
    ];

    private static readonly string[] QVowelHeight =
    [
        "Articulation.Close",
        "Articulation.NearClose",
        "Articulation.CloseMid",
        "Articulation.Mid",
        "Articulation.OpenMid",
        "Articulation.NearOpen",
        "Articulation.Open"
    ];

    private static readonly string[,] QVowelCharacter =
    {
        { "i y", "ɨ ʉ", "ɯ u" },
        { "ɪ ʏ", "", "ʊ" },
        { "e ø", "ɘ ɵ", "ɤ o" },
        { "", "ə", "" },
        { "ɛ œ", "ɜ ɞ", "ʌ ɔ" },
        { "æ", "ɐ", "" },
        { "a ɶ", "ä", "ɑ ɒ" }
    };

    private void QVowelBuild()
    {
        QArticulationTableBuild(QVowel, QVowelBackness.Length, QVowelHeight.Length);

        for (int column = 0; column < QVowelBackness.Length; column++)
        {
            QArticulationHeaderPlace(QVowel, QVowelBackness[column], column);
        }

        for (int row = 0; row < QVowelHeight.Length; row++)
        {
            QArticulationSidePlace(QVowel, QVowelHeight[row], row);

            for (int column = 0; column < QVowelBackness.Length; column++)
            {
                QArticulationCellPlace(
                    QVowel,
                    QVowelCharacter[row, column].Split(' ', System.StringSplitOptions.RemoveEmptyEntries),
                    column,
                    row);
            }
        }
    }
}
