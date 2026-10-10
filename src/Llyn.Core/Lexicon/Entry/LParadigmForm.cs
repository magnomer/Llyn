using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigmForm(
    string LParadigmFormText,
    IReadOnlyList<LInflectionMark> LParadigmFormMarks,
    string? LParadigmFormTip,
    int LParadigmFormSplit = 0);
