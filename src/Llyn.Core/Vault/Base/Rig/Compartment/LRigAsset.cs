namespace Llyn.Core;

public sealed record LRigAsset(
    LManifestVault LRigAssetManifest,
    LLocalizationVault LRigAssetLocalization,
    LMarkupVault LRigAssetMarkup,
    LPortraitVault LRigAssetPortrait,
    LLivery LRigAssetLivery);
