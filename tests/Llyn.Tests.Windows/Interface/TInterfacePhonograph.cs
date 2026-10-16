using Llyn.Core;
using Llyn.Core.Windows;

namespace Llyn.Tests;

internal static class TInterfacePhonograph
{
    internal static LPhonograph TPhonographCreate() => new LPhonographMedia();

    internal static void TPhonographPlay(LPhonograph phonograph, string file)
    {
        phonograph.LPhonographPlay(file);
    }
}
