using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CLeafChip(
    long CLeafChipId,
    CStateWording CLeafChipWording,
    CSubject CLeafChipSubject,
    bool CLeafChipStored)
{
    internal static CLeafChip LLeafChipRead(LSituationDraft situation)
    {
        return new CLeafChip(
            situation.LSituationDraftId,
            CStateWording.LStateWordingRead(CFolio.CFolioStateRead(situation.LSituationDraftTitle), null),
            CSubject.CSubjectSituation,
            situation.LSituationDraftStored);
    }

    internal static CLeafChip LLeafChipRead(LRegisterDraft register)
    {
        return new CLeafChip(
            register.LRegisterDraftId,
            CStateWording.LStateWordingRead(CFolio.CFolioStateRead(register.LRegisterDraftName), null),
            CSubject.CSubjectRegister,
            register.LRegisterDraftStored);
    }

    internal static CLeafChip LLeafChipRead(LTagDraft tag)
    {
        return new CLeafChip(
            tag.LTagDraftId,
            new CStateWording(tag.LTagDraftText, null, false, null),
            CSubject.CSubjectTag,
            tag.LTagDraftStored);
    }
}
