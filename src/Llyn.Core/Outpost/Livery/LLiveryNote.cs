using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLiveryNote(
    string LLiveryNoteBody,
    IReadOnlyList<LParcel> LLiveryNoteParcel);
