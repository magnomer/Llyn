namespace Llyn.Core;

public interface LLivery
{
    string LLiveryRead();

    LLiveryNote LLiveryFormat(LPortraitPage page, string style);
}
