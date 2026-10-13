using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LMarkdownPort
{
    IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text);
}
