using System;
using Llyn.Application;

namespace Llyn.ShellEngine;

public sealed class LQuillAuthor
{
    private readonly LTenure _lQuillAuthorTenure;

    public LQuillAuthor(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillAuthorTenure = tenure;
    }

    public void LQuillAuthorSet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        _lQuillAuthorTenure.LTenureRequestDefer(new LRequestAuthorName(_lQuillAuthorTenure.LTenureId, name));
    }
}
