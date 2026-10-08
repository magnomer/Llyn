using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LEntryStaff(
    LMeaningClerk LEntryStaffMeaning,
    LEntryClerk LEntryStaffEntry,
    LEntryQueryClerk LEntryStaffQuery,
    LGraspClerk LEntryStaffGrasp,
    LOutcomeClerk LEntryStaffOutcome,
    LCitationClerk LEntryStaffCitation)
{
    internal static LEntryStaff LEntryStaffBuild(
        LRig rig,
        LIdentity identity,
        LLanguageCache cache,
        LRevisionClerk revision,
        LCatalogStaff catalog,
        LClaimStaff claim,
        LLanguageStaff language)
    {
        LEntryQueryClerk query = new(rig);
        LGraspClerk grasp = new(rig);
        LCardClerk card = new(
            rig, catalog.LCatalogStaffTag, catalog.LCatalogStaffRegister, catalog.LCatalogStaffTranslation,
            catalog.LCatalogStaffExample);
        LMeaningClerk meaning = new(rig, card);
        LInflectionClerk inflection = new(rig);
        LEntryClerk entry = new(
            rig, card, meaning, language.LLanguageStaffVocabulary, inflection, language.LLanguageStaffParadigm,
            language.LLanguageStaffPronunciation, language.LLanguageStaffTranscription,
            language.LLanguageStaffReflex, language.LLanguageStaffRecording, language.LLanguageStaffFrequency,
            revision);
        LOutcomeClerk outcome = new(
            rig, cache, claim.LClaimStaffDraft, claim.LClaimStaffClaim, claim.LClaimStaffCourt, entry,
            language.LLanguageStaffLacuna, language.LLanguageStaffFrequency);
        LCitationClerk citation = new(
            rig, identity, claim.LClaimStaffClaim, catalog.LCatalogStaffAuthor, catalog.LCatalogStaffExample,
            catalog.LCatalogStaffReference, catalog.LCatalogStaffSituation, entry, revision);

        return new LEntryStaff(meaning, entry, query, grasp, outcome, citation);
    }
}
