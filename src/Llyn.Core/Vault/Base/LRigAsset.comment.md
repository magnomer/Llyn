# LRigAsset.cs
Hash: `353ad140ea730368`

## `public sealed record LRigAsset(LManifestVault LRigAssetManifest, LLocalizationVault LRigAssetLocalization, LMarkupVault LRigAssetMarkup, LPortraitVault LRigAssetPortrait, LLivery LRigAssetLivery)`

The asset group of the rig holds the ports that read and write workspace and theme files.
It covers the manifest, localization, markup, portrait and livery.
`LRig` holds it as `LRigAsset`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigAsset{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactoryAsset`, and a test builds it from fakes.
