using System;
using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PSentenceFrame : INotifyPropertyChanged
{
    private CStateWording _pSentenceFrameParticle;
    private CStateWording _pSentenceFrameDependence;
    private bool _pSentenceFrameVisible;
    private CSentenceOrder _pSentenceFrameOrder = CSentenceOrder.CSentenceOrderPlain;

    internal PSentenceFrame(CStateWording particle, CStateWording dependence)
    {
        _pSentenceFrameParticle = particle;
        _pSentenceFrameDependence = dependence;
    }

    public CStateWording PSentenceFrameParticle
    {
        get => _pSentenceFrameParticle;
        private set
        {
            if (_pSentenceFrameParticle == value)
            {
                return;
            }

            _pSentenceFrameParticle = value;
            PSentenceFrameRaise(nameof(PSentenceFrameParticle));
        }
    }

    public CStateWording PSentenceFrameDependence
    {
        get => _pSentenceFrameDependence;
        private set
        {
            if (_pSentenceFrameDependence == value)
            {
                return;
            }

            _pSentenceFrameDependence = value;
            PSentenceFrameRaise(nameof(PSentenceFrameDependence));
        }
    }

    public bool PSentenceFrameVisible
    {
        get => _pSentenceFrameVisible || PSentenceFrameWritten;
        set
        {
            _pSentenceFrameVisible = value;
            PSentenceFrameRaise(nameof(PSentenceFrameVisible));
        }
    }

    public bool PSentenceFrameWritten =>
        !_pSentenceFrameParticle.CStateWordingMuted || !_pSentenceFrameDependence.CStateWordingMuted;

    public string PSentenceFrameGap =>
        !_pSentenceFrameParticle.CStateWordingMuted && !_pSentenceFrameDependence.CStateWordingMuted
            ? " "
            : string.Empty;

    public CSentenceOrder PSentenceFrameOrder
    {
        get => _pSentenceFrameOrder;
        private set
        {
            if (_pSentenceFrameOrder == value)
            {
                return;
            }

            _pSentenceFrameOrder = value;
            PSentenceFrameRaise(nameof(PSentenceFrameOrder));
        }
    }

    internal void PSentenceFrameApply(CSentenceOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        PSentenceFrameOrder = order;
    }

    internal void PSentenceFrameShow(CStateWording particle, CStateWording dependence)
    {
        bool changed = _pSentenceFrameParticle != particle || _pSentenceFrameDependence != dependence;
        PSentenceFrameParticle = particle;
        PSentenceFrameDependence = dependence;
        if (changed)
        {
            PSentenceFrameRaise(nameof(PSentenceFrameVisible));
            PSentenceFrameRaise(nameof(PSentenceFrameWritten));
            PSentenceFrameRaise(nameof(PSentenceFrameGap));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PSentenceFrameRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
