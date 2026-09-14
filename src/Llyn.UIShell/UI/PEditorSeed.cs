using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal string PEditorLanguageRead()
    {
        return _pSpeakerChoice;
    }

    internal void PEditorTagAdd(long tagId)
    {
        PEditorRequestSend(new LRequestTagPick(_pEditorDraft, 0, tagId, 0));
    }

    internal void PEditorRegisterAdd(long registerId)
    {
        PEditorRequestSend(new LRequestRegisterPick(_pEditorDraft, 0, registerId, 0));
    }

    internal void PEditorSituationAdd(long situationId)
    {
        PEditorRequestSend(new LRequestSituationPick(_pEditorDraft, 0, situationId, 0));
    }

    internal void PEditorExampleAdd(long exampleId)
    {
        PEditorRequestSend(new LRequestSentenceExample(_pEditorDraft, 0, 0, exampleId));
    }

    internal void PEditorReferenceAdd(long referenceId)
    {
        PEditorRequestSend(new LRequestSentenceReference(_pEditorDraft, 0, 0, referenceId));
    }
}
