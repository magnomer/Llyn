using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LSentenceArchive TSentenceArchiveCreate(LDatabase database) =>
        new(database);

    internal static LSentenceOrder TSentenceOrderLoad(string language) =>
        LSentenceLoader.LSentenceLoaderLoad(language);

    internal static IReadOnlyList<long> TSentenceMeaningSave(
        this LSentenceArchive sentenceArchive,
        long meaningId,
        IReadOnlyList<LSentence> sentences)
    {
        return sentenceArchive.LSentenceMeaningSave(meaningId, sentences);
    }

    internal static IReadOnlyList<LSentence> TSentenceMeaningRead(
        this LSentenceArchive sentenceArchive,
        long meaningId) =>
        sentenceArchive.LSentenceMeaningRead(meaningId);

    internal static IReadOnlyList<string> TSentenceParticleRead(
        this LSentenceArchive sentenceArchive,
        string language) =>
        sentenceArchive.LSentenceParticleRead(language);

    internal static IReadOnlyList<string> TSentenceDependenceRead(
        this LSentenceArchive sentenceArchive,
        string language) =>
        sentenceArchive.LSentenceDependenceRead(language);

    internal static void TSentenceCollocationSave(
        this LSentenceArchive sentenceArchive,
        long collocationId,
        IReadOnlyList<LSentence> sentences)
    {
        sentenceArchive.LSentenceCollocationSave(collocationId, sentences);
    }

    internal static IReadOnlyList<LSentence> TSentenceCollocationRead(
        this LSentenceArchive sentenceArchive,
        long collocationId) =>
        sentenceArchive.LSentenceCollocationRead(collocationId);

    internal static LMentionArchive TMentionArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LMention> TMentionRead(
        this LMentionArchive mentionArchive,
        long exampleId) =>
        mentionArchive.LMentionExampleRead(exampleId);

    internal static IReadOnlyList<long> TMentionSave(
        this LMentionArchive mentionArchive,
        long exampleId,
        IReadOnlyList<LMention> mentions) =>
        mentionArchive.LMentionExampleSave(exampleId, mentions);
}
