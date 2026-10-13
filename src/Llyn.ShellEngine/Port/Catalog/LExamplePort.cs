using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LExamplePort
{
    IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "");

    bool LEngineTextMatch(string field, string shown);

    (string, IReadOnlyList<LMentionPiece>, string) LEngineLineRead(
        LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations);
}
