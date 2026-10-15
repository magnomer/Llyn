using System;
using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSentenceFrame : INotifyPropertyChanged
{
    private CStateWording _qSentenceFrameParticle;
    private CStateWording _qSentenceFrameDependence;
    private bool _qSentenceFrameVisible;
    private CSentenceOrder _qSentenceFrameOrder = CSentenceOrder.CSentenceOrderPlain;

    internal QSentenceFrame(CStateWording particle, CStateWording dependence)
    {
        _qSentenceFrameParticle = particle;
        _qSentenceFrameDependence = dependence;
    }

    public CStateWording QSentenceFrameParticle
    {
        get => _qSentenceFrameParticle;
        private set
        {
            if (_qSentenceFrameParticle == value)
            {
                return;
            }

            _qSentenceFrameParticle = value;
            QSentenceFrameRaise(nameof(QSentenceFrameParticle));
        }
    }

    public CStateWording QSentenceFrameDependence
    {
        get => _qSentenceFrameDependence;
        private set
        {
            if (_qSentenceFrameDependence == value)
            {
                return;
            }

            _qSentenceFrameDependence = value;
            QSentenceFrameRaise(nameof(QSentenceFrameDependence));
        }
    }

    public bool QSentenceFrameVisible
    {
        get => _qSentenceFrameVisible || QSentenceFrameWritten;
        set
        {
            _qSentenceFrameVisible = value;
            QSentenceFrameRaise(nameof(QSentenceFrameVisible));
        }
    }

    public bool QSentenceFrameWritten =>
        !_qSentenceFrameParticle.CStateWordingMuted || !_qSentenceFrameDependence.CStateWordingMuted;

    public string QSentenceFrameGap =>
        !_qSentenceFrameParticle.CStateWordingMuted && !_qSentenceFrameDependence.CStateWordingMuted
            ? " "
            : string.Empty;

    public CSentenceOrder QSentenceFrameOrder
    {
        get => _qSentenceFrameOrder;
        private set
        {
            if (_qSentenceFrameOrder == value)
            {
                return;
            }

            _qSentenceFrameOrder = value;
            QSentenceFrameRaise(nameof(QSentenceFrameOrder));
        }
    }

    internal void QSentenceFrameApply(CSentenceOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        QSentenceFrameOrder = order;
    }

    internal void QSentenceFrameShow(CStateWording particle, CStateWording dependence)
    {
        bool changed = _qSentenceFrameParticle != particle || _qSentenceFrameDependence != dependence;
        QSentenceFrameParticle = particle;
        QSentenceFrameDependence = dependence;
        if (changed)
        {
            QSentenceFrameRaise(nameof(QSentenceFrameVisible));
            QSentenceFrameRaise(nameof(QSentenceFrameWritten));
            QSentenceFrameRaise(nameof(QSentenceFrameGap));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void QSentenceFrameRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
