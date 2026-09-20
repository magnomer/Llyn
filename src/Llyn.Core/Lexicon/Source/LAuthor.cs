using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LAuthor(
    long LAuthorId,
    string LAuthorName)
{
    public bool LAuthorNamed => LAuthorName.Trim().Length > 0;

    public bool LAuthorStored => LAuthorId > 0;

    public bool LAuthorMatch(long id)
    {
        return LAuthorId == id;
    }

    public bool LAuthorNameMatch(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return string.Equals(LAuthorName, name, StringComparison.Ordinal);
    }
}
