using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigmForm(
    string LParadigmFormText,
    IReadOnlyList<LInflectionMark> LParadigmFormMarks,
    LParadigmStatus LParadigmFormStatus,
    int LParadigmFormSplit = 0);
