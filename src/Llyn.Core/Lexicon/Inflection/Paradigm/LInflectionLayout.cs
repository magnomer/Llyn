namespace Llyn.Core;

public sealed record LInflectionLayout(
    long LInflectionLayoutPart,
    LInflectionSheet LInflectionLayoutCollapsed,
    LInflectionSheet LInflectionLayoutExpanded,
    LInflectionLayout? LInflectionLayoutCustom = null);
