using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed record LCatalogStaff(
    LTagClerk LCatalogStaffTag,
    LRegisterClerk LCatalogStaffRegister,
    LTranslationClerk LCatalogStaffTranslation,
    LReferenceClerk LCatalogStaffReference,
    LExampleClerk LCatalogStaffExample,
    LSituationClerk LCatalogStaffSituation,
    LMentionClerk LCatalogStaffMention,
    LUsageClerk LCatalogStaffUsage,
    LAuthorClerk LCatalogStaffAuthor,
    LFavoriteClerk LCatalogStaffFavorite)
{
    internal static LCatalogStaff LCatalogStaffBuild(LRig rig, LLanguageCache cache, LRevisionClerk revision)
    {
        LTagClerk tag = new(rig);
        LRegisterClerk register = new(rig);
        LTranslationClerk translation = new(rig, revision);
        LReferenceClerk reference = new(rig);
        LExampleClerk example = new(rig, reference);
        LSituationClerk situation = new(rig);
        LMentionClerk mention = new(rig, cache);
        LUsageClerk usage = new(rig);
        LAuthorClerk author = new(rig);
        LFavoriteClerk favorite = new(rig);

        return new LCatalogStaff(
            tag, register, translation, reference, example, situation, mention, usage, author, favorite);
    }
}
