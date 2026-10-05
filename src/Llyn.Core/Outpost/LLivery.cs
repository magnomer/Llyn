using System.Collections.Generic;

namespace Llyn.Core;

public interface LLivery
{
    string LLiveryRead();

    LLiveryNote LLiveryFormat(LPortraitPage page, string style);

    string LLiveryMarkFormat(string style);

    string LLiveryIdFormat(string seed);

    string LLiveryDigestFormat(LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels);
}
