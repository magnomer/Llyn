namespace Llyn.Core;

public interface LNoteVault
{
    void LNoteSave(LNote note);

    LNote? LNoteRead(long entryId);

    void LNoteDelete(long entryId);
}
