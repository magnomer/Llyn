using System.Windows.Input;

namespace Llyn.UIShell;

public static class PTranscriptionCommand
{
    public static RoutedCommand PTranscriptionCommandAddition { get; } =
        new(nameof(PTranscriptionCommandAddition), typeof(PTranscriptionCommand));

    public static RoutedCommand PTranscriptionCommandRemoval { get; } =
        new(nameof(PTranscriptionCommandRemoval), typeof(PTranscriptionCommand));

    public static RoutedCommand PTranscriptionCommandNotation { get; } =
        new(nameof(PTranscriptionCommandNotation), typeof(PTranscriptionCommand));
}
