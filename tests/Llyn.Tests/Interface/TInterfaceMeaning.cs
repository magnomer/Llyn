using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LCollocationArchive TCollocationArchiveCreate(LDatabase database) =>
        new(database);

    internal static LCollocation TCollocationCreate(
        this LCollocationArchive collocationArchive,
        LCollocation collocation) =>
        collocationArchive.LCollocationCreate(collocation);

    internal static IReadOnlyList<LCollocation> TCollocationRead(
        this LCollocationArchive collocationArchive,
        long entryId) =>
        collocationArchive.LCollocationRead(entryId);

    internal static void TCollocationDelete(this LCollocationArchive collocationArchive, long id)
    {
        collocationArchive.LCollocationDelete(id);
    }

    internal static LMeaningArchive TMeaningArchiveCreate(LDatabase database) =>
        new(database);

    internal static LMeaning TMeaningCreate(this LMeaningArchive meaningArchive, LMeaning meaning) =>
        meaningArchive.LMeaningCreate(meaning);

    internal static void TMeaningDelete(this LMeaningArchive meaningArchive, long id)
    {
        meaningArchive.LMeaningDelete(id);
    }

    internal static void TMeaningMove(this LMeaningArchive meaningArchive, long id, int position)
    {
        meaningArchive.LMeaningMove(id, position);
    }

    internal static IReadOnlyList<LMeaning> TMeaningRead(this LMeaningArchive meaningArchive, long entryId) =>
        meaningArchive.LMeaningRead(entryId);

    internal static LRegisterArchive TRegisterArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LRegister> TRegisterCollocationRead(
        this LRegisterArchive registerArchive,
        long collocationId) =>
        registerArchive.LRegisterCollocationRead(collocationId);

    internal static IReadOnlyList<LRegister> TRegisterMeaningRead(
        this LRegisterArchive registerArchive,
        long meaningId) =>
        registerArchive.LRegisterMeaningRead(meaningId);

    internal static IReadOnlyList<LRegister> TRegisterRead(this LRegisterArchive registerArchive) =>
        registerArchive.LRegisterRead();

    internal static LRegister? TRegisterRead(this LRegisterArchive registerArchive, long id) =>
        registerArchive.LRegisterRead(id);

    internal static void TRegisterDelete(this LRegisterArchive registerArchive, long id) =>
        registerArchive.LRegisterDelete(id);

    internal static void TRegisterDelete(
        this LRegisterArchive registerArchive, long id, bool detach) =>
        registerArchive.LRegisterDelete(id, detach);

    internal static LSituationArchive TSituationArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LSituation> TSituationCollocationRead(
        this LSituationArchive situationArchive,
        long collocationId) =>
        situationArchive.LSituationCollocationRead(collocationId);

    internal static IReadOnlyList<LSituation> TSituationMeaningRead(
        this LSituationArchive situationArchive,
        long meaningId) =>
        situationArchive.LSituationMeaningRead(meaningId);

    internal static LTagArchive TTagArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LTag> TTagCollocationRead(
        this LTagArchive tagArchive,
        long collocationId) =>
        tagArchive.LTagCollocationRead(collocationId);

    internal static IReadOnlyList<LTag> TTagMeaningRead(this LTagArchive tagArchive, long meaningId) =>
        tagArchive.LTagMeaningRead(meaningId);

    internal static void TTagMeaningSave(this LTagArchive tagArchive, long meaningId, IReadOnlyList<LTag> tags)
    {
        tagArchive.LTagMeaningSave(meaningId, tags);
    }

    internal static LTranslationArchive TTranslationArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LTranslation> TTranslationCollocationRead(
        this LTranslationArchive translationArchive,
        long collocationId) =>
        translationArchive.LTranslationCollocationRead(collocationId);

    internal static void TTranslationCollocationSave(
        this LTranslationArchive translationArchive,
        long collocationId,
        IReadOnlyList<LTranslation> translations)
    {
        translationArchive.LTranslationCollocationSave(collocationId, translations);
    }

    internal static IReadOnlyList<LUsage> TTranslationIncomingRead(
        this LTranslationArchive translationArchive,
        long entryId) =>
        translationArchive.LTranslationIncomingRead(entryId);

    internal static IReadOnlyList<LTranslation> TTranslationMeaningRead(
        this LTranslationArchive translationArchive,
        long meaningId) =>
        translationArchive.LTranslationMeaningRead(meaningId);

    internal static void TTranslationMeaningSave(
        this LTranslationArchive translationArchive,
        long meaningId,
        IReadOnlyList<LTranslation> translations)
    {
        translationArchive.LTranslationMeaningSave(meaningId, translations);
    }

    internal static IReadOnlyList<LTranslationTarget> TTranslationTargetRead(
        this LTranslationArchive translationArchive,
        IReadOnlyList<long> ids) =>
        translationArchive.LTranslationTargetRead(ids);
}
