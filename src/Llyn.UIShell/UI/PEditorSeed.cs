using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal string PEditorLanguageRead()
    {
        return PSpeakerLanguageRead();
    }

    internal void PEditorTagAdd(long tagId)
    {
        PEditorRequestSend(new LRequestTagPick(PEditorDraft, 0, tagId, 0));
    }

    internal void PEditorRegisterAdd(long registerId)
    {
        PEditorRequestSend(new LRequestRegisterPick(PEditorDraft, 0, registerId, 0));
    }

    internal void PEditorSituationAdd(long situationId)
    {
        PEditorRequestSend(new LRequestSituationPick(PEditorDraft, 0, situationId, 0));
    }

    internal void PEditorExampleAdd(long exampleId)
    {
        PEditorRequestSend(new LRequestSentenceExample(PEditorDraft, 0, 0, exampleId));
    }

    internal void PEditorReferenceAdd(long referenceId)
    {
        PEditorRequestSend(new LRequestSentenceReference(PEditorDraft, 0, 0, referenceId));
    }
}
