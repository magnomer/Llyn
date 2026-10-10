using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CStemPage(
    string CStemPageLanguage,
    string CStemPageKey,
    IReadOnlyList<CStemMember> CStemPageMembers,
    bool CStemPageEmpty,
    CFont CStemPageFont,
    CFont CStemPageGlyph);
