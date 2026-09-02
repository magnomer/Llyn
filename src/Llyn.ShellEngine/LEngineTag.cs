using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LTag> LEngineTagRead()
    {
        return new LTagArchive(_lEngineDatabase).LTagCatalogRead();
    }

    public IReadOnlyList<LTag> LEngineTagRead(string ownerId, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        return LEngineOwnerCheck(owner)
            ? tags.LTagCollocationRead(ownerId)
            : tags.LTagSenseRead(ownerId);
    }

    public void LEngineTagSave(string ownerId, IReadOnlyList<LTag> written, LOwner owner)
    {
        ArgumentNullException.ThrowIfNull(written);

        LTagArchive tags = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            tags.LTagCollocationSave(ownerId, written);
            return;
        }

        tags.LTagSenseSave(ownerId, written);
    }

    public void LEngineTagChange(string text, string renamed)
    {
        new LTagArchive(_lEngineDatabase).LTagChange(text, renamed);
    }

    public void LEngineTagDelete(string text)
    {
        new LTagArchive(_lEngineDatabase).LTagDelete(text);
    }
}
