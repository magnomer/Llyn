namespace Llyn.Core;

public sealed record LRigSentence(
    LExampleVault LRigSentenceExamples,
    LMentionVault LRigSentenceMentions,
    LSentenceVault LRigSentenceSentences,
    LTranslationVault LRigSentenceTranslations);
