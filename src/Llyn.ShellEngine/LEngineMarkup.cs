using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LEntry> LEngineMarkupImport(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        throw new NotSupportedException("No markup reader stands in this workspace.");
    }
}
