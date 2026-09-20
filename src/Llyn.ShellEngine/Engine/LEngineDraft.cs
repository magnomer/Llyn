using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly HashSet<long> _lEngineDraftStale = [];

    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)
    {
        return LEntryClerkField.LMarkdownParse(text);
    }

    internal LDraft LEngineDraftStart(string origin, long? entryId)
    {
        lock (_lEngineGate)
        {
            IReadOnlyList<string> languages = LEngineLanguageRead();
            string language = languages.Count > 0 ? languages[0] : string.Empty;
            return _lEngineCitationClerk.LEntryStart(origin, entryId, language);
        }
    }

    internal IReadOnlyList<LRequest> LEngineDraftPrepare(long id)
    {
        lock (_lEngineGate)
        {
            LEntryDraft draft = LEngineDraftLoad(id).LDraftContent;
            List<LRequest> requests = [];
            if (!draft.LEntryDraftDefined)
            {
                requests.Add(new LRequestCardAddition(id, LCardKind.LCardKindMeaning, 0, 0));
            }

            if (!draft.LEntryDraftCollocated)
            {
                requests.Add(new LRequestCardAddition(id, LCardKind.LCardKindCollocation, 0, 0));
            }

            foreach (LCardDraft card in draft.LEntryDraftMeanings)
            {
                LEngineSentencePrepare(id, card, requests);
            }

            foreach (LCardDraft card in draft.LEntryDraftCollocations)
            {
                LEngineSentencePrepare(id, card, requests);
            }

            LGlyph? glyph = LEngineGlyphRead(draft.LEntryDraftLanguage);
            if (LEngineSchemeRead(draft.LEntryDraftLanguage) is [string scheme, ..]
                && !LGlyph.LGlyphOtherCheck(glyph, draft.LEntryDraftTranscriptions))
            {
                requests.Add(new LRequestTranscriptionAddition(id, scheme, 0, true));
            }

            if (glyph is not null && !LGlyph.LGlyphRowCheck(glyph, draft.LEntryDraftTranscriptions))
            {
                requests.Add(new LRequestTranscriptionAddition(
                    id, glyph.LGlyphName, draft.LEntryDraftTranscriptions.Count, true));
            }

            return requests;
        }
    }

    private static void LEngineSentencePrepare(long id, LCardDraft card, List<LRequest> requests)
    {
        if (!card.LCardDraftExemplified)
        {
            requests.Add(new LRequestSentenceAddition(id, card.LCardDraftId, 0));
        }
    }

    internal LDraft? LEngineDraftRead(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            return _lEngineClaimClerk.LDraftRead(id);
        }
    }

    internal IReadOnlyList<LDraft> LEngineDraftScan()
    {
        lock (_lEngineGate)
        {
            return _lEngineClaimClerk.LDraftScan();
        }
    }

    public void LEngineDraftDelete(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            _lEngineClaimClerk.LClaimClerkDelete(id);
            _lEngineTrove.LTroveClear(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    internal bool LEngineDraftCheck(long id)
    {
        return LEngineDraftCheck(id, out _);
    }

    internal bool LEngineDraftCheck(long id, out string? refusal)
    {
        lock (_lEngineGate)
        {
            refusal = null;
            if (id == 0)
            {
                return false;
            }

            LEngineDraftValidate(id);

            LDraft? draft = _lEngineClaimClerk.LDraftRead(id);
            if (draft is null)
            {
                return false;
            }

            refusal = LClaimClerk.LDraftRefusalRead(draft);
            return LEngineDraftCheck(draft);
        }
    }

    private bool LEngineDraftCheck(LDraft draft)
    {
        return _lEngineCitationClerk.LCitationDraftCheck(draft);
    }

    internal void LEngineDraftSweep(long id)
    {
        LDraft saved;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            saved = LEngineDraftLoad(id).LDraftNormalize();
            _lEngineClaimClerk.LDraftSave(saved);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
    }

    internal LOutcome LEngineDraftCommit(long id)
    {
        LOutcome outcome;
        List<long> raised = [];
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            outcome = _lEngineOutcomeClerk.LOutcomeClerkCommit(id, raised);
            _lEngineTrove.LTroveClear(id);
        }

        foreach (long entryId in raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectEntry, entryId);
            LEngineInflectionStart(entryId);
        }

        return outcome;
    }

    internal void LEngineDraftCancel(long id)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            _lEngineClaimClerk.LClaimClerkCancel(id);
            _lEngineTrove.LTroveClear(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    public void LEngineLeftoverSweep()
    {
        lock (_lEngineGate)
        {
            _lEngineClaimClerk.LClaimClerkSweep();

            foreach (LDraft draft in _lEngineClaimClerk.LDraftScan())
            {
                if (_lEngineClaimClerk.LClaimClerkHeld.Contains(draft.LDraftId)
                    || _lEngineClaimClerk.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                if (draft.LDraftEntryId <= 0)
                {
                    if (!LEngineDraftCheck(draft))
                    {
                        _lEngineClaimClerk.LClaimClerkCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (_lEngineCitationClerk.LCitationLeftoverCheck(draft))
                {
                    _lEngineClaimClerk.LClaimClerkCancel(draft.LDraftId);
                }
            }
        }
    }

    public IReadOnlyList<LDraft> LEngineLeftoverRead()
    {
        lock (_lEngineGate)
        {
            List<LDraft> leftovers = [];
            foreach (LDraft draft in _lEngineClaimClerk.LDraftScan())
            {
                if (_lEngineClaimClerk.LClaimClerkHeld.Contains(draft.LDraftId)
                    || !LEngineDraftCheck(draft.LDraftId)
                    || _lEngineClaimClerk.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                leftovers.Add(draft);
            }

            return leftovers;
        }
    }

    private void LEngineDraftValidate(long id)
    {
        LClaimClerk.LClaimStaleValidate(_lEngineDraftStale, id);
    }

    private LDraft LEngineDraftLoad(long id)
    {
        return _lEngineClaimClerk.LDraftLoad(id);
    }
}
