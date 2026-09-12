using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PMentionLine
{
    private readonly Dictionary<long, string> _pMentionHeadword = [];

    private readonly Dictionary<long, string> _pMentionSense = [];

    public ObservableCollection<PMentionChip> PMentionLineChip { get; } = [];

    internal void PMentionLineShow(LEngine engine, string text, IReadOnlyList<LMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

        PMentionLineRead(engine, mentions);

        List<PMentionChip> wanted = new(mentions.Count);
        foreach (LMentionDraft mention in mentions)
        {
            wanted.Add(PMentionLineCreate(text, mention, silent));
        }

        for (int index = 0; index < wanted.Count; index++)
        {
            if (index >= PMentionLineChip.Count)
            {
                PMentionLineChip.Add(wanted[index]);
            }
            else if (!PMentionLineChip[index].Equals(wanted[index]))
            {
                PMentionLineChip[index] = wanted[index];
            }
        }

        while (PMentionLineChip.Count > wanted.Count)
        {
            PMentionLineChip.RemoveAt(PMentionLineChip.Count - 1);
        }
    }

    internal void PMentionLineClear()
    {
        PMentionLineChip.Clear();
        _pMentionHeadword.Clear();
        _pMentionSense.Clear();
    }

    private PMentionChip PMentionLineCreate(string text, LMentionDraft mention, string silent)
    {
        int start = LMentionSpan.LMentionUnitRead(text, mention.LMentionDraftOffset);
        int end = LMentionSpan.LMentionUnitRead(text, mention.LMentionDraftOffset + mention.LMentionDraftLength);
        string word = end > start ? text[start..end] : string.Empty;

        string name = mention.LMentionDraftEntry == 0
            ? silent
            : _pMentionHeadword.GetValueOrDefault(mention.LMentionDraftEntry, string.Empty);
        string sense = mention.LMentionDraftSense == 0
            ? string.Empty
            : _pMentionSense.GetValueOrDefault(mention.LMentionDraftSense, string.Empty);

        return new PMentionChip(mention.LMentionDraftId, word, name, sense);
    }

    private void PMentionLineRead(LEngine engine, IReadOnlyList<LMentionDraft> mentions)
    {
        List<long> entries = [];
        List<long> senses = [];
        foreach (LMentionDraft mention in mentions)
        {
            if (mention.LMentionDraftEntry != 0 && !_pMentionHeadword.ContainsKey(mention.LMentionDraftEntry))
            {
                entries.Add(mention.LMentionDraftEntry);
            }

            if (mention.LMentionDraftSense != 0 && !_pMentionSense.ContainsKey(mention.LMentionDraftSense))
            {
                senses.Add(mention.LMentionDraftSense);
            }
        }

        if (entries.Count > 0)
        {
            foreach (LTranslationTarget target in engine.LEngineTargetRead(entries))
            {
                _pMentionHeadword[target.LTranslationTargetId] = target.LTranslationTargetHeadword;
            }
        }

        foreach (long sense in senses)
        {
            LMeaning? meaning = engine.LEngineMeaningRead(sense);
            string title = meaning?.LMeaningTitle.LStateValueShow() ?? string.Empty;
            if (title.Length == 0 && meaning is not null)
            {
                title = meaning.LMeaningDefinition.LStateValueShow();
            }

            _pMentionSense[sense] = title;
        }
    }
}
