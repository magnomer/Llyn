using System;

namespace Llyn.Core;

public interface LPortraitVault
{
    void LPortraitSave(LPortraitPage page, LPortraitMedium format, string path);

    string LPortraitSheetFormat(LPortraitPage page);

    void LPortraitSave(
        LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup, string path);

    string LPortraitSheetFormat(
        LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, Func<string, string> lookup);
}
