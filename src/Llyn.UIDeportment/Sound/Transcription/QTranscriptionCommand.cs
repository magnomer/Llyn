using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QTranscriptionCommand
{
    public static RoutedCommand QTranscriptionCommandAddition { get; } =
        new(nameof(QTranscriptionCommandAddition), typeof(QTranscriptionCommand));

    public static RoutedCommand QTranscriptionCommandRemoval { get; } =
        new(nameof(QTranscriptionCommandRemoval), typeof(QTranscriptionCommand));

    public static RoutedCommand QTranscriptionCommandNotation { get; } =
        new(nameof(QTranscriptionCommandNotation), typeof(QTranscriptionCommand));
}
