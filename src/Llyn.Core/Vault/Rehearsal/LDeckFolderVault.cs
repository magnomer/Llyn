using System.Collections.Generic;

namespace Llyn.Core;

public interface LDeckFolderVault
{
    LDeckFolder LDeckFolderCreate(string name);

    IReadOnlyList<LDeckFolder> LDeckFolderRead();

    void LDeckFolderUpdate(LDeckFolder folder);

    void LDeckFolderDelete(long folderId);
}
