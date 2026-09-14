using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LDraftArchive
{
    private const string LDraftArchiveExtension = ".json";
    private const string LDraftArchivePending = ".json.tmp";

    public const int LDraftArchiveVersion = 4;

    private static readonly TimeSpan LDraftArchiveStale = TimeSpan.FromHours(1);

    private static readonly JsonSerializerOptions LDraftArchiveIndent = new() { WriteIndented = true };

    public static void LDraftArchiveSave(string root, LDraft draft)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentOutOfRangeException.ThrowIfZero(draft.LDraftId);
        LDraftArchiveValidate(draft);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(root);
        string pending = Path.Combine(folder, draft.LDraftId.ToString(CultureInfo.InvariantCulture) + LDraftArchivePending);
        string path = Path.Combine(folder, draft.LDraftId.ToString(CultureInfo.InvariantCulture) + LDraftArchiveExtension);

        File.WriteAllText(
            pending,
            JsonSerializer.Serialize(draft with { LDraftVersion = LDraftArchiveVersion }, LDraftArchiveIndent));
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }

    public static LDraft? LDraftArchiveRead(string root, long id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceDraftRead(root), id.ToString(CultureInfo.InvariantCulture) + LDraftArchiveExtension);
        return LDraftArchiveLoad(path);
    }

    public static IReadOnlyList<LDraft> LDraftArchiveScan(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(root);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, "*" + LDraftArchiveExtension);
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }

        List<LDraft> drafts = [];
        foreach (string file in files)
        {
            if (!file.EndsWith(LDraftArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (LDraftArchiveLoad(file) is LDraft draft)
            {
                drafts.Add(draft);
            }
        }

        return drafts;
    }

    public static void LDraftArchiveDelete(string root, long id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceDraftRead(root), id.ToString(CultureInfo.InvariantCulture) + LDraftArchiveExtension);
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public static IReadOnlyList<long> LDraftArchiveSweep(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(root);

        string[] pending;
        string[] files;
        try
        {
            pending = Directory.GetFiles(folder, "*" + LDraftArchivePending);
            files = Directory.GetFiles(folder, "*" + LDraftArchiveExtension);
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }

        DateTime edge = DateTime.UtcNow - LDraftArchiveStale;
        foreach (string file in pending)
        {
            if (!file.EndsWith(LDraftArchivePending, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                if (File.GetLastWriteTimeUtc(file) > edge)
                {
                    continue;
                }

                File.Delete(file);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        List<long> dropped = [];
        foreach (string file in files)
        {
            if (!file.EndsWith(LDraftArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string text;
            try
            {
                text = File.ReadAllText(file);
            }
            catch (IOException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

            LDraft? draft = LDraftArchiveParse(text, false);
            if (draft is not null && draft.LDraftVersion == LDraftArchiveVersion)
            {
                continue;
            }

            if (!LDraftBrokenSave(root, file))
            {
                continue;
            }

            if (draft is not null && draft.LDraftId != 0)
            {
                dropped.Add(draft.LDraftId);
            }
        }

        return dropped;
    }

    private static bool LDraftBrokenSave(string root, string file)
    {
        string folder = LWorkspaceRoot.LWorkspaceBrokenRead(root);
        string name = Path.GetFileName(file);
        string target = Path.Combine(folder, name);
        if (File.Exists(target))
        {
            string stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string stem = Path.GetFileNameWithoutExtension(name);
            target = Path.Combine(folder, stem + "." + stamp + LDraftArchiveExtension);
        }

        try
        {
            File.Move(file, target, true);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static void LDraftArchiveValidate(LDraft draft)
    {
        if (draft.LDraftExample is LExample example)
        {
            LDraftArchiveCheck(example.LExampleId, "example");
        }

        if (draft.LDraftSituation is LSituation situation)
        {
            LDraftArchiveCheck(situation.LSituationId, "situation");
            LDraftArchiveCheck(situation.LSituationImage, situation.LSituationVideo);
        }

        if (draft.LDraftReference is LReference reference)
        {
            LDraftArchiveCheck(reference.LReferenceId, "reference");
        }

        foreach (LAuthor author in draft.LDraftAuthor)
        {
            LDraftArchiveCheck(author.LAuthorId, "author");
        }

        foreach (LPronunciationDraft spoken in draft.LDraftContent.LEntryDraftPronunciations)
        {
            LDraftArchiveCheck(spoken.LPronunciationDraftId, "pronunciation");
        }

        foreach (LTranscriptionDraft spelled in draft.LDraftContent.LEntryDraftTranscriptions)
        {
            LDraftArchiveCheck(spelled.LTranscriptionDraftId, "transcription");
        }

        LDraftArchiveCheck(draft.LDraftContent.LEntryDraftMeanings);
        LDraftArchiveCheck(draft.LDraftContent.LEntryDraftCollocations);
    }

    private static void LDraftArchiveCheck(IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            LDraftArchiveCheck(card.LCardDraftId, "card");

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                LDraftArchiveCheck(sentence.LSentenceDraftId, "sentence");
                if (sentence.LSentenceDraftExample is LExampleDraft example)
                {
                    LDraftArchiveCheck(example.LExampleDraftId, "example");
                }
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                LDraftArchiveCheck(situation.LSituationDraftId, "situation");
            }

            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                LDraftArchiveCheck(register.LRegisterDraftId, "register");
            }

            foreach (long translation in card.LCardDraftTranslation)
            {
                LDraftArchiveCheck(translation, "translation");
            }

            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                LDraftArchiveCheck(tag.LTagDraftId, "tag");
            }

            LDraftArchiveCheck(card.LCardDraftImage, card.LCardDraftVideo);
            LDraftArchiveCheck(card.LCardDraftChild);
        }
    }

    private static void LDraftArchiveCheck(IReadOnlyList<LImageDraft> images, IReadOnlyList<LVideoDraft> videos)
    {
        foreach (LImageDraft image in images)
        {
            LDraftArchiveCheck(image.LImageDraftId, "image");
        }

        foreach (LVideoDraft video in videos)
        {
            LDraftArchiveCheck(video.LVideoDraftId, "video");
        }
    }

    private static void LDraftArchiveCheck(long id, string kind)
    {
        if (id == 0)
        {
            throw new InvalidOperationException($"A draft {kind} carries no id.");
        }
    }

    private static LDraft? LDraftArchiveLoad(string path, bool checking = true)
    {
        try
        {
            return LDraftArchiveParse(File.ReadAllText(path), checking);
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static LDraft? LDraftArchiveParse(string text, bool checking)
    {
        try
        {
            LDraft? draft = JsonSerializer.Deserialize<LDraft>(text, LDraftArchiveIndent);
            return checking && draft is not null && draft.LDraftVersion != LDraftArchiveVersion
                ? null
                : draft;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }
}
