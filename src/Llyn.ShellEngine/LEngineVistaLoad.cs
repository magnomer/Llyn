using System;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LDraft? LEngineVistaLoad(LVista vista)
    {
        lock (_lEngineGate)
        {
            if (vista.LVistaChosen is not long id || id <= 0)
            {
                return null;
            }

            LDraft draft = new(0, vista.LVistaTab, id, LEngineDraftBlank, DateTimeOffset.UtcNow);
            return vista.LVistaSubject switch
            {
                LSubject.LSubjectEntry => LEngineEntryLoad(id) is LEntryDraft entry
                    ? draft with { LDraftContent = entry } : null,
                LSubject.LSubjectExample => LEngineExampleRead(id) is LExample example
                    ? draft with { LDraftExample = example } : null,
                LSubject.LSubjectSituation => LEngineSituationRead(id) is LSituation situation
                    ? draft with { LDraftSituation = situation } : null,
                LSubject.LSubjectReference => LEngineReferenceRead(id) is LReference reference
                    ? draft with { LDraftReference = reference, LDraftAuthor = LEngineCreditRead(id) } : null,
                LSubject.LSubjectAuthor => LEngineAuthorRead(id) is LAuthor author
                    ? draft with { LDraftAuthorHeld = author } : null,
                LSubject.LSubjectTag => LEngineTagRead().FirstOrDefault(row => row.LTagId == id) is LTag tag
                    ? draft with { LDraftTag = tag } : null,
                LSubject.LSubjectRegister => LEngineRegisterFind(string.Empty, LCatalogOrder.LCatalogOrderName)
                    .FirstOrDefault(row => row.LCatalogRegisterStored.LRegisterId == id) is LCatalogRegister register
                    ? draft with { LDraftRegister = register.LCatalogRegisterStored } : null,
                _ => null,
            };
        }
    }
}
