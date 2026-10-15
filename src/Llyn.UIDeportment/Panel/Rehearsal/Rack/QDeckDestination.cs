using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDeckDestination
{
    internal QDeckDestination(CDeckDestination destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        QDeckDestinationDeck = destination;
    }

    public CDeckDestination QDeckDestinationDeck { get; }
}
