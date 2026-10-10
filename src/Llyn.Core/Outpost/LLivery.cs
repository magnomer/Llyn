using System;
using System.Collections.Generic;

namespace Llyn.Core;

public interface LLivery
{
    string LLiveryRead();

    LLiveryNote LLiveryFormat(
        LLiveryPage page,
        string style,
        Func<long, string> note,
        Func<string, string, string, string> link,
        Func<string, string> lookup);

    LLiveryNote LLiveryFormat(
        LLiveryStem stem, string style, Func<long, string> note, Func<string, string> lookup);

    LLiveryNote LLiveryFormat(
        LLiveryDiwei diwei, string style, Func<long, string> note, Func<string, string> lookup);

    string LLiveryMarkFormat(string style);

    string LLiveryIdFormat(string seed);

    string LLiveryDigestFormat(LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels);
}
