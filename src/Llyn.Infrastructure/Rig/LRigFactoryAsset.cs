using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryAsset
{
    public static LRigAsset LRigAssetBuild(string root, LTheme theme) =>
        new(
            new LManifestFile(new LKeepFile(root)),
            new LLocalizationLoader(),
            new LMarkupFile(),
            new LPortraitFile(theme),
            new LLiverySheet(theme));
}
