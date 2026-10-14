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
    LPressClerk LWorkspaceStaffGalley,
    LCourierClerk LWorkspaceStaffCourier,
    LDeckClerk LWorkspaceStaffDeck,
    LCueClerk LWorkspaceStaffCue,
    LStencilClerk LWorkspaceStaffStencil,
    LDepotLatch LWorkspaceStaffLatch,
    LDepotClerk LWorkspaceStaffDepot)
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
        LMarkupClerkLink link = new(
            rig, catalog.LCatalogStaffReference, catalog.LCatalogStaffAuthor, language.LLanguageStaffTrail);
        LMarkupClerkAncestor markupAncestor = new(rig, entry.LEntryStaffAncestor, link);
        LMarkupClerk markup = new(
            rig,
            new LMarkupClerkEntry(
                rig, language.LLanguageStaffReflex, new LMarkupClerkCard(rig, markupExample), markupAncestor));
        LMarkupClerkIntake intake = new(
            rig, claim.LClaimStaffClaim, entry.LEntryStaffEntry, revision, entry.LEntryStaffQuery,
            language.LLanguageStaffLacuna, language.LLanguageStaffFrequency, link,
            new LMarkupClerkDraft(rig, link, markupAncestor));
        LPortraitClerk portrait = new(
            language.LLanguageStaffLanguage, entry.LEntryStaffEntry, language.LLanguageStaffVocabulary,
            catalog.LCatalogStaffTranslation, catalog.LCatalogStaffReference, catalog.LCatalogStaffFavorite,
            entry.LEntryStaffFold, language.LLanguageStaffFanqie, language.LLanguageStaffFrequency,
            language.LLanguageStaffParadigm, language.LLanguageStaffScript, entry.LEntryStaffLineage,
            entry.LEntryStaffAncestor, settings);
        LPortraitClerkPress press = new(rig);
        LPressClerk galley = new(rig);
        LCourierClerk courier = new(
            rig, entry.LEntryStaffQuery, gate, settings, exception => workspace.LWorkspaceAuditRecord(exception));

        LDeckClerk deck = new(rig, language.LLanguageStaffTrail);
        LCueClerk cue = new(rig, entry.LEntryStaffEntry);
        LStencilClerk stencil = new(
            rig, entry.LEntryStaffEntry, catalog.LCatalogStaffTranslation, catalog.LCatalogStaffReference,
            language.LLanguageStaffTrail);
        LDepotLatch latch = new();
        LDepotClerk depot = new(
            rig, stencil, language.LLanguageStaffTrail, latch, gate, settings,
            exception => workspace.LWorkspaceAuditRecord(exception));

        return new LWorkspaceStaff(
            workspace, markup, intake, portrait, press, galley, courier, deck, cue, stencil, latch, depot);
    }
}
