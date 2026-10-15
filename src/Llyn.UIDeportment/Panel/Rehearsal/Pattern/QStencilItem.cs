using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStencilItem : INotifyPropertyChanged
{
    private CStencilSlot _qStencilItemSlot;

    internal QStencilItem(CStencilSlot slot)
    {
        _qStencilItemSlot = slot;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CStencilSlot QStencilItemSlot
    {
        get => _qStencilItemSlot;

        private set
        {
            if (_qStencilItemSlot == value)
            {
                return;
            }

            _qStencilItemSlot = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QStencilItemSlot)));
        }
    }

    internal static bool QStencilItemMatch(QStencilItem held, QStencilItem fresh)
    {
        return held.QStencilItemSlot.CStencilSlotColumn == fresh.QStencilItemSlot.CStencilSlotColumn
            && held.QStencilItemSlot.CStencilSlotIndex == fresh.QStencilItemSlot.CStencilSlotIndex;
    }

    internal static void QStencilItemSync(QStencilItem held, QStencilItem fresh)
    {
        held.QStencilItemSlot = fresh.QStencilItemSlot;
    }
}
