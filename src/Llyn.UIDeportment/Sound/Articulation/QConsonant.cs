namespace Llyn.UIDeportment;

internal sealed partial class QArticulation
{
    private static readonly string[] QConsonantLocation =
    [
        "Articulation.Bilabial",
        "Articulation.Labiodental",
        "Articulation.Dental",
        "Articulation.Alveolar",
        "Articulation.Postalveolar",
        "Articulation.Retroflex",
        "Articulation.Palatal",
        "Articulation.Velar",
        "Articulation.Uvular",
        "Articulation.Pharyngeal",
        "Articulation.Glottal"
    ];

    private static readonly string[] QConsonantManner =
    [
        "Articulation.Plosive",
        "Articulation.Nasal",
        "Articulation.Trill",
        "Articulation.Tap",
        "Articulation.Fricative",
        "Articulation.LateralFricative",
        "Articulation.Approximant",
        "Articulation.LateralApproximant"
    ];

    private static readonly string[,] QConsonantCharacter =
    {
        { "p b", "", "", "t d", "", "ʈ ɖ", "c ɟ", "k ɡ", "q ɢ", "", "ʔ" },
        { "m", "ɱ", "", "n", "", "ɳ", "ɲ", "ŋ", "ɴ", "", "" },
        { "ʙ", "", "", "r", "", "", "", "", "ʀ", "", "" },
        { "", "ⱱ", "", "ɾ", "", "ɽ", "", "", "", "", "" },
        { "ɸ β", "f v", "θ ð", "s z", "ʃ ʒ", "ʂ ʐ", "ç ʝ", "x ɣ", "χ ʁ", "ħ ʕ", "h ɦ" },
        { "", "", "", "ɬ ɮ", "", "", "", "", "", "", "" },
        { "", "ʋ", "", "ɹ", "", "ɻ", "j", "ɰ", "", "", "" },
        { "", "", "", "l", "", "ɭ", "ʎ", "ʟ", "", "", "" }
    };

    private void QConsonantBuild()
    {
        QArticulationTableBuild(QConsonant, QConsonantLocation.Length, QConsonantManner.Length);

        for (int column = 0; column < QConsonantLocation.Length; column++)
        {
            QArticulationHeaderPlace(QConsonant, QConsonantLocation[column], column);
        }

        for (int row = 0; row < QConsonantManner.Length; row++)
        {
            QArticulationSidePlace(QConsonant, QConsonantManner[row], row);

            for (int column = 0; column < QConsonantLocation.Length; column++)
            {
                QArticulationCellPlace(QConsonant, QConsonantCharacter[row, column], column, row);
            }
        }
    }
}
