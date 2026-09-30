using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLectern(
    string CLecternHeadword,
    string CLecternLanguage,
    IReadOnlyList<string> CLecternSpeeches,
    bool CLecternMarked,
    IReadOnlyList<CMarkdownBlock> CLecternNote,
    bool CLecternNoted,
    string CLecternAdded,
    string CLecternUpdated,
    bool CLecternStamped);
