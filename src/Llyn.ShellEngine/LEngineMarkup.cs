using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public const long LEngineMarkupCeiling = 64L * 1024 * 1024;

    public LMarkupCargo LEngineMarkupRead(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        IReadOnlyList<LMarkupEntry> parsed = LMarkup.LMarkupParse(
            LEngineMarkupLoad(path), out IReadOnlyList<LMarkupOmission> skipped);

        List<LMarkupOmission> omissions = [.. skipped];
        List<LMarkupEntry> entries = new(parsed.Count);
        foreach (LMarkupEntry entry in parsed)
        {
            string language = entry.LMarkupEntryLanguage;
            if (language.Length == 0 || LLanguageLoader.LLanguageNameValidate(language))
            {
                entries.Add(entry);
                continue;
            }

            omissions.Add(new LMarkupOmission(entry.LMarkupEntryLine, $"language \"{language}\""));
            entries.Add(entry with { LMarkupEntryLanguage = string.Empty });
        }

        return new LMarkupCargo(entries, omissions);
    }

    private static string LEngineMarkupLoad(string path)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length > LEngineMarkupCeiling)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        using StreamReader reader = new(stream, Encoding.UTF8, true);
        return reader.ReadToEnd();
    }

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_lEngineGate)
        {
            return LEngineMarkupFind(entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage);
        }
    }

    public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        ArgumentNullException.ThrowIfNull(cargo);
        ArgumentNullException.ThrowIfNull(intakes);

        IReadOnlyList<LMarkupEntry> entries = cargo.LMarkupCargoEntry;
        if (intakes.Count != entries.Count)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LMarkupIntake?[] ordered = new LMarkupIntake?[entries.Count];
        foreach (LMarkupIntake intake in intakes)
        {
            if (intake.LMarkupIntakeIndex < 0
                || intake.LMarkupIntakeIndex >= ordered.Length
                || ordered[intake.LMarkupIntakeIndex] is not null)
            {
                throw new LRefusal(LRefusal.LRefusalItem);
            }

            ordered[intake.LMarkupIntakeIndex] = intake;
        }

        List<LMarkupOmission> omissions = [.. cargo.LMarkupCargoOmission];
        List<LEntry> stored = new(entries.Count);
        lock (_lEngineGate)
        {
            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEngineMarkupValidate(ordered!);
            IReadOnlyDictionary<int, long> prepared = LEngineMarkupPrepare(entries, ordered!);

            Dictionary<(string, string), long> named = [];
            for (int index = 0; index < entries.Count; index++)
            {
                (string, string) key = (
                    LCatalog.LCatalogTextNormalize(entries[index].LMarkupEntryHeadword),
                    LCatalog.LCatalogTextNormalize(entries[index].LMarkupEntryLanguage));
                named[key] = named.ContainsKey(key) ? 0 : prepared[index];
            }

            LEntryArchive archive = new(_lEngineDatabase);
            List<LRevisionChange> changes = [];
            List<(LMarkupMention, int)> held = [];
            Dictionary<long, long> identity = [];
            for (int index = 0; index < entries.Count; index++)
            {
                long id = prepared[index];
                LMarkupMode mode = ordered[index]!.LMarkupIntakeMode;
                if (mode == LMarkupMode.LMarkupModeReplace)
                {
                    LEngineMarkupDetach(id, omissions);
                }

                LMarkupEntry entry = entries[index];
                if (mode != LMarkupMode.LMarkupModeNew && string.IsNullOrWhiteSpace(entry.LMarkupEntryLanguage))
                {
                    entry = entry with
                    {
                        LMarkupEntryLanguage = archive.LEntryRead(id)?.LEntryLanguage ?? string.Empty,
                    };
                }

                int noted = omissions.Count;
                LEntryDraft draft = LEngineMarkupResolve(entry, named, omissions, held);
                LEngineLineSet(omissions, noted, entry.LMarkupEntryLine);

                if (mode == LMarkupMode.LMarkupModeMerge)
                {
                    draft = LEngineMarkupAppend(
                        LEngineEntryLoad(id) ?? throw new LRefusal(LRefusal.LRefusalEntry), draft);
                }

                changes.Add(new LRevisionChange(
                    0,
                    id,
                    "entry",
                    mode == LMarkupMode.LMarkupModeNew ? "create" : "update",
                    draft.LEntryDraftHeadword));
                stored.Add(LEngineEntryUpdate(id, draft, identity, changes));
            }

            LEngineMarkupSettle(held, identity, named, omissions);

            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord(changes);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

            session.LDatabaseSessionCommit();
        }

        foreach (LEntry entry in stored)
        {
            LEngineFrequencyStart(entry.LEntryId);
        }

        return new LMarkupOutcome(stored, omissions);
    }

    private static void LEngineLineSet(List<LMarkupOmission> omissions, int noted, int line)
    {
        for (int index = noted; index < omissions.Count; index++)
        {
            if (omissions[index].LMarkupOmissionLine == 0)
            {
                omissions[index] = omissions[index] with { LMarkupOmissionLine = line };
            }
        }
    }

    private void LEngineMarkupValidate(IReadOnlyList<LMarkupIntake> intakes)
    {
        LEntryArchive archive = new(_lEngineDatabase);
        IReadOnlyList<LDraft>? drafts = null;
        HashSet<long> targets = [];
        foreach (LMarkupIntake intake in intakes)
        {
            if (intake.LMarkupIntakeMode == LMarkupMode.LMarkupModeNew)
            {
                continue;
            }

            long target = intake.LMarkupIntakeTarget;
            if (target <= 0 || archive.LEntryRead(target) is null)
            {
                throw new LRefusal(LRefusal.LRefusalEntry);
            }

            if (!targets.Add(target))
            {
                throw new LRefusal(LRefusal.LRefusalItem);
            }

            drafts ??= LDraftArchive.LDraftArchiveScan(_lEngineWorkspace);
            foreach (LDraft draft in drafts)
            {
                if (draft.LDraftEntryId == target
                    && draft.LDraftExample is null
                    && draft.LDraftSituation is null
                    && draft.LDraftReference is null
                    && _lEngineDraftHeld.Contains(draft.LDraftId))
                {
                    throw new LRefusal(LRefusal.LRefusalStale);
                }
            }
        }
    }

    private IReadOnlyDictionary<int, long> LEngineMarkupPrepare(
        IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<LMarkupIntake> intakes)
    {
        LEntryArchive archive = new(_lEngineDatabase);
        Dictionary<int, long> prepared = new(entries.Count);
        foreach (LMarkupIntake intake in intakes)
        {
            LMarkupEntry entry = entries[intake.LMarkupIntakeIndex];
            if (intake.LMarkupIntakeMode != LMarkupMode.LMarkupModeNew)
            {
                prepared[intake.LMarkupIntakeIndex] = intake.LMarkupIntakeTarget;
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.LMarkupEntryHeadword))
            {
                throw new LRefusal(LRefusal.LRefusalHeadword);
            }

            LEntry created = archive.LEntryCreate(
                new LEntry(0, entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage, 0, null, null, null),
                [],
                []);
            prepared[intake.LMarkupIntakeIndex] = created.LEntryId;
        }

        return prepared;
    }

    private LEntryDraft LEngineMarkupResolve(
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int)> held)
    {
        string language = entry.LMarkupEntryLanguage;
        LSpeechArchive values = new(_lEngineDatabase);

        List<LSpeechDraft> speeches = new(entry.LMarkupEntrySpeech.Count);
        foreach (string name in entry.LMarkupEntrySpeech)
        {
            if (LEngineMarkupResolve(values, language, name, omissions) is LSpeechValue value)
            {
                speeches.Add(LSpeechDraft.LSpeechDraftCreate(value.LSpeechValueId, value.LSpeechValueName));
            }
        }

        List<LInflection> inflections = new(entry.LMarkupEntryInflection.Count);
        foreach (LMarkupInflection inflection in entry.LMarkupEntryInflection)
        {
            inflections.Add(LEngineMarkupResolve(values, language, inflection, inflections.Count, omissions));
        }

        List<LPronunciationDraft> pronunciations = new(entry.LMarkupEntryPronunciation.Count);
        foreach (LPronunciationDraft pronunciation in entry.LMarkupEntryPronunciation)
        {
            pronunciations.Add(LEngineMarkupResolve(pronunciation.LPronunciationDraftAudio, omissions)
                ? pronunciation
                : pronunciation with { LPronunciationDraftAudio = string.Empty });
        }

        return new LEntryDraft(
            entry.LMarkupEntryHeadword,
            language,
            pronunciations,
            entry.LMarkupEntryNote,
            LEngineMarkupResolve(entry.LMarkupEntryMeaning, entry, prepared, omissions, held),
            LEngineMarkupResolve(entry.LMarkupEntryCollocation, entry, prepared, omissions, held),
            speeches,
            entry.LMarkupEntryForm,
            inflections,
            entry.LMarkupEntryTranscription);
    }

    private static LSpeechValue? LEngineMarkupResolve(
        LSpeechArchive values, string language, string name, List<LMarkupOmission> omissions)
    {
        LSpeechValue? value = string.IsNullOrWhiteSpace(language) ? null : values.LSpeechValueFind(language, name);
        if (value is null)
        {
            omissions.Add(new LMarkupOmission(0, $"speech \"{name}\""));
        }

        return value;
    }

    private LInflection LEngineMarkupResolve(
        LSpeechArchive values,
        string language,
        LMarkupInflection inflection,
        int position,
        List<LMarkupOmission> omissions)
    {
        LSpeechValue? speech = inflection.LMarkupInflectionSpeech.Length == 0
            ? null
            : LEngineMarkupResolve(values, language, inflection.LMarkupInflectionSpeech, omissions);

        LMorphologyArchive morphologies = new(_lEngineDatabase);
        IReadOnlyList<LFeature> features = speech is null ? [] : morphologies.LFeatureRead(speech.LSpeechValueId);

        List<long> resolved = new(inflection.LMarkupInflectionMorphology.Count);
        foreach (string name in inflection.LMarkupInflectionMorphology)
        {
            LMorphology? found = null;
            foreach (LFeature feature in features)
            {
                found = morphologies.LMorphologyFind(feature.LFeatureId, name);
                if (found is not null)
                {
                    break;
                }
            }

            if (found is null)
            {
                omissions.Add(new LMarkupOmission(0, $"morphology \"{name}\""));
                continue;
            }

            resolved.Add(found.LMorphologyId);
        }

        return new LInflection(
            0,
            0,
            position,
            inflection.LMarkupInflectionText,
            inflection.LMarkupInflectionLocal,
            speech?.LSpeechValueId,
            resolved);
    }

    private IReadOnlyList<LCardDraft> LEngineMarkupResolve(
        IReadOnlyList<LMarkupCard> cards,
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int)> held)
    {
        List<LCardDraft> drafts = new(cards.Count);
        foreach (LMarkupCard card in cards)
        {
            List<LSentenceDraft> sentences = new(card.LMarkupCardSentence.Count);
            foreach (LMarkupSentence sentence in card.LMarkupCardSentence)
            {
                sentences.Add(new LSentenceDraft(
                    sentence.LMarkupSentenceExample is LMarkupExample example
                        ? LEngineMarkupResolve(example, entry.LMarkupEntryLine, prepared, omissions, held)
                        : null,
                    sentence.LMarkupSentenceParticle,
                    sentence.LMarkupSentenceDependence));
            }

            List<long> translations = new(card.LMarkupCardTranslation.Count);
            foreach (LMarkupTranslation translation in card.LMarkupCardTranslation)
            {
                long target = LEngineMarkupResolve(
                    translation.LMarkupTranslationHeadword, translation.LMarkupTranslationLanguage, prepared);
                if (target == 0)
                {
                    omissions.Add(new LMarkupOmission(0, $"translation \"{translation.LMarkupTranslationHeadword}\""));
                    continue;
                }

                translations.Add(target);
            }

            List<LImageDraft> images = new(card.LMarkupCardImage.Count);
            foreach (LImageDraft image in card.LMarkupCardImage)
            {
                if (LEngineMarkupResolve(image.LImageDraftLocation.LStateValueShow(), omissions))
                {
                    images.Add(image);
                }
            }

            List<LVideoDraft> videos = new(card.LMarkupCardVideo.Count);
            foreach (LVideoDraft video in card.LMarkupCardVideo)
            {
                if (LEngineMarkupResolve(video.LVideoDraftLocation.LStateValueShow(), omissions))
                {
                    videos.Add(video);
                }
            }

            drafts.Add(new LCardDraft(
                card.LMarkupCardTitle,
                card.LMarkupCardExpression,
                card.LMarkupCardMeaning,
                sentences,
                card.LMarkupCardSituation,
                card.LMarkupCardRegister,
                translations,
                card.LMarkupCardTag,
                images,
                videos,
                drafts.Count + 1,
                0,
                LEngineMarkupResolve(card.LMarkupCardChild, entry, prepared, omissions, held)));
        }

        return drafts;
    }
}
