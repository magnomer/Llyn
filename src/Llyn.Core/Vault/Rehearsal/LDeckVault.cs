using System.Collections.Generic;

namespace Llyn.Core;

public interface LDeckVault
{
    LDeck LDeckCreate(LDeck deck);

    IReadOnlyList<LDeck> LDeckRead();

    LDeck? LDeckRead(long deckId);

    void LDeckUpdate(LDeck deck);

    void LDeckOrderSet(long? folderId, IReadOnlyList<long> order);

    void LDeckDelete(long deckId);

    IReadOnlyList<LBearing> LBearingRead(long deckId);

    void LBearingSave(long deckId, IReadOnlyList<LBearing> bearings);
}
