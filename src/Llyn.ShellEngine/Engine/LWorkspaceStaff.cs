using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LWorkspaceStaff(
    LWorkspaceClerk LWorkspaceStaffWorkspace,
    LMarkupClerk LWorkspaceStaffMarkup,
    LMarkupClerkIntake LWorkspaceStaffIntake,
    LPortraitClerk LWorkspaceStaffPortrait,
    LPortraitClerkPress LWorkspaceStaffPress,
    LCourierClerk LWorkspaceStaffCourier)
{
    internal static LWorkspaceStaff LWorkspaceStaffBuild(
        LRig rig,
        LLanguageCache cache,
        LRevisionClerk revision,
        object gate,
        Func<LSettings> settings,
        LCatalogStaff catalog,
        LClaimStaff claim,
        LLanguageStaff language,
        LEntryStaff entry)
    {
        LWorkspaceClerk workspace = new(
            rig, cache, language.LLanguageStaffReflex, language.LLanguageStaffParadigm);
        LMarkupClerkExample markupExample = new(rig);
        LMarkupClerk markup = new(
            rig,
            new LMarkupClerkEntry(
                rig, language.LLanguageStaffReflex, new LMarkupClerkCard(rig, markupExample), markupExample));
        LMarkupClerkLink link = new(
            rig, catalog.LCatalogStaffReference, catalog.LCatalogStaffAuthor, language.LLanguageStaffTrail);
        LMarkupClerkIntake intake = new(
            rig, claim.LClaimStaffClaim, entry.LEntryStaffEntry, revision, entry.LEntryStaffQuery,
            language.LLanguageStaffLacuna, language.LLanguageStaffFrequency, link, new LMarkupClerkDraft(rig, link));
        LPortraitClerk portrait = new(
            language.LLanguageStaffLanguage, entry.LEntryStaffEntry, language.LLanguageStaffVocabulary,
            catalog.LCatalogStaffTranslation, catalog.LCatalogStaffReference, catalog.LCatalogStaffFavorite,
            language.LLanguageStaffFanqie, language.LLanguageStaffFrequency, language.LLanguageStaffParadigm,
            language.LLanguageStaffScript, settings);
        LPortraitClerkPress press = new(rig);
        LCourierClerk courier = new(
            rig, entry.LEntryStaffQuery, gate, settings, exception => workspace.LWorkspaceAuditRecord(exception));

        return new LWorkspaceStaff(workspace, markup, intake, portrait, press, courier);
    }
}
