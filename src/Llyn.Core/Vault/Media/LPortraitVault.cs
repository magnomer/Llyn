namespace Llyn.Core;

public interface LPortraitVault
{
    void LPortraitSave(LPortraitPage page, LPortraitFormat format, string path);

    string LPortraitSheetFormat(LPortraitPage page);
}
