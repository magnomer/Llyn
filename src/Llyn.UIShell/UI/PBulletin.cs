using Llyn.Core;

namespace Llyn.UIShell;

internal static class PBulletin
{
    internal static bool PBulletinEntryCheck(LSubject subject)
    {
        return subject switch
        {
            LSubject.LSubjectDraft => false,
            LSubject.LSubjectFrequency => false,
            LSubject.LSubjectGrasp => false,
            LSubject.LSubjectInflection => false,
            LSubject.LSubjectScript => false,
            LSubject.LSubjectFanqie => false,
            _ => true,
        };
    }
}
