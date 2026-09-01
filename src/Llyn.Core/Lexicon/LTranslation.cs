namespace Llyn.Core;

/// <summary>
/// One translation of an <see cref="LExample"/> into another language. A translation is owned text: it
/// belongs to exactly one Example, is ordered within it, and is removed with it. Its identity is
/// <see cref="LTranslationId"/> — an opaque, program-generated stable id — so reordering rewrites
/// <see cref="LTranslationPosition"/> only and never changes which translation is which.
/// </summary>
/// <param name="LTranslationId">Opaque, program-generated stable id.</param>
/// <param name="LTranslationLanguage">Language code the translation is written in.</param>
/// <param name="LTranslationText">The translated text.</param>
/// <param name="LTranslationPosition">Order among the owning Example's translations.</param>
public sealed record LTranslation(
    string LTranslationId,
    string LTranslationLanguage,
    string LTranslationText,
    int LTranslationPosition);
