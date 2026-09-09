using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LPress? _lEnginePress;

    public void LEnginePressApply(LPress press)
    {
        ArgumentNullException.ThrowIfNull(press);

        _lEnginePress = press;
    }

    public async Task LEnginePortraitExport(
        string entryId, string path, LPortraitFormat format, LPortraitLabel label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(label);

        if (format == LPortraitFormat.LPortraitFormatMarkup)
        {
            File.WriteAllText(path, LEngineMarkupFormat(entryId), new UTF8Encoding(false));
            return;
        }

        LPortrait portrait = LEnginePortraitRead(entryId, label);
        LTheme theme = LTheme.LThemeLoad();

        switch (format)
        {
            case LPortraitFormat.LPortraitFormatHtml:
                File.WriteAllText(
                    path, LSheet.LSheetFormat(portrait, theme), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatMarkdown:
                File.WriteAllText(
                    path, LOutline.LOutlineFormat(portrait), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatDocx:
                LFolio.LFolioSave(portrait, theme, path);
                return;
            case LPortraitFormat.LPortraitFormatPdf:
                if (_lEnginePress is not LPress press)
                {
                    throw new InvalidOperationException(
                        "No printing surface stands ready for this workspace.");
                }

                await press.LPressSave(LSheet.LSheetFormat(portrait, theme), path);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    private string LEngineMarkupFormat(string entryId)
    {
        LEntryDraft draft = LEngineEntryLoad(entryId)
            ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");

        List<string> ids = new List<string>();
        LEngineSourceRead(draft.LEntryDraftMeanings, ids);
        LEngineSourceRead(draft.LEntryDraftCollocations, ids);

        try
        {
            foreach (LReference attached in LEngineReferenceRead(entryId, LOwner.LOwnerEntry))
            {
                if (!ids.Contains(attached.LReferenceId))
                {
                    ids.Add(attached.LReferenceId);
                }
            }
        }
        catch (Exception)
        {
            ids.Clear();
        }

        List<LMarkup.LMarkupReference> sources = new List<LMarkup.LMarkupReference>();
        foreach (string id in ids)
        {
            LReference? held;
            List<string> writers = new List<string>();

            try
            {
                held = LEngineReferenceRead(id);

                if (held is not null)
                {
                    foreach (LAuthor author in LEngineAuthorRead(id, LOwner.LOwnerReference))
                    {
                        writers.Add(author.LAuthorName);
                    }
                }
            }
            catch (Exception)
            {
                continue;
            }

            if (held is not null)
            {
                sources.Add(new LMarkup.LMarkupReference(id, held, writers));
            }
        }

        return LMarkupDraft.LMarkupDraftFormat(draft, sources);
    }

    private static void LEngineSourceRead(
        IReadOnlyList<LCardDraft> cards, List<string> ids)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (LExampleDraft example in card.LCardDraftExample)
            {
                if (example.LExampleDraftReference.LStateValueState != LState.LStateSpecified)
                {
                    continue;
                }

                string id = example.LExampleDraftReference.LStateValueShow();
                if (id.Length > 0 && !ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
        }
    }
}
