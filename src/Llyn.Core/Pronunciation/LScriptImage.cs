namespace Llyn.Core;

public sealed record LScriptImage(
    string LScriptImageCharacter,
    string LScriptImageStyle,
    int LScriptImagePosition,
    string LScriptImageCaption,
    string LScriptImageGloss,
    byte[] LScriptImageData);
