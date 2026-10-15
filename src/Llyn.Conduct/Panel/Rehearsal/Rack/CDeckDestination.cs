namespace Llyn.Conduct;

public sealed record CDeckDestination(
    long CDeckDestinationDeck,
    long? CDeckDestinationFolder,
    string? CDeckDestinationKey,
    string CDeckDestinationName);
