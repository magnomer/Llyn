using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LRigFactoryAsset
{
    public static LRigAsset LRigAssetBuild(LTheme theme)
    {
        return new(
            new LLocalizationLoader(),
            new LMarkupFile(),
            new LPortraitFile(theme));
    }
}
