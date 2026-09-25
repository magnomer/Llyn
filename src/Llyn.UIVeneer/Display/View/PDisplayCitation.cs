using System;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayCitationShow()
    {
        PCitationConverter converter = PDisplayCitationRead();
        try
        {
            converter.PCitationConverterShow(_lLectern.LLecternCitationRead());
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
