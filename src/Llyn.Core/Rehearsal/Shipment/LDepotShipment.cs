using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDepotShipment(
    string LDepotShipmentFolder,
    long LDepotShipmentDeck,
    string LDepotShipmentCode,
    bool LDepotShipmentBasic,
    IReadOnlyList<long> LDepotShipmentCues,
    IReadOnlyList<LCueFace> LDepotShipmentFaces,
    IReadOnlyList<IReadOnlyList<string?>> LDepotShipmentTyped);
