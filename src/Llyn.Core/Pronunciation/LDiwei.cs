namespace Llyn.Core;

public sealed record LDiwei(
    long LDiweiId,
    string LDiweiLanguage,
    string LDiweiKind,
    string LDiweiKey,
    int LDiweiCount = 0)
{
    public const string LDiweiInitial = "initial";

    public const string LDiweiRime = "rime";

    public const string LDiweiTone = "tone";
}
