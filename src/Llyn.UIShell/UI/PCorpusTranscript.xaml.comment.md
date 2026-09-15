# PCorpusTranscript.xaml.cs

## `public partial class PCorpusTranscript : ResourceDictionary`

The class the transcript dictionary needs so its templates can name handlers.
It holds the Corpus panel it was built for and forwards every handler to it unchanged.
The panel's handlers are internal for this one caller, and stay the panel's.
