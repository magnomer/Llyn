using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSituation(
    CStateWording CSituationTitle,
    CStateWording CSituationKind,
    CStateWording CSituationDescription,
    IReadOnlyList<CMarkdownBlock> CSituationMarkdown,
    IReadOnlyList<CImageDraft> CSituationImage,
    IReadOnlyList<CVideoDraft> CSituationVideo);
