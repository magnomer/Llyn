namespace Llyn.Core;

public interface LPortraitVault
{
    void LPortraitSave(LPortraitPage page, LPortraitMedium format, string path);

    string LPortraitSheetFormat(LPortraitPage page);
}
