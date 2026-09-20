using System;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayCitationShow()
    {
        PCitationConverter converter = PDisplayCitationRead();
        try
        {
            converter.PCitationConverterShow(_lDisplay.LDisplayCitationRead());
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
