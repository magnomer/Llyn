using System;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayCitationShow()
    {
        PCitationConverter converter = PDisplayCitationRead();
        try
        {
            converter.PCitationConverterShow(_lEngine.LEngineCitationRead());
        }
        catch (Exception)
        {
            converter.PCitationConverterClear();
        }
    }

    private PCitationConverter PDisplayCitationRead()
    {
        return (PCitationConverter)Resources["Display.Card.Citation"];
    }
}
