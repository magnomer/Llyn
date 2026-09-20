namespace Llyn.Core;

public sealed record LSpeechValue(
    long LSpeechValueId,
    string LSpeechValueLanguage,
    long LSpeechValueCode,
    string LSpeechValueName,
    int LSpeechValuePosition,
    long LSpeechValueParent = 0);
