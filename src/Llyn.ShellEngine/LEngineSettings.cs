using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSettings LEngineSettingsRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineSettings;
        }
    }

    public void LEngineLocalizationSave(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        LEngineSettingsChange(settings => settings with { LSettingsLocalization = language });
    }

    public void LEngineWindowSave(LWindowState window)
    {
        ArgumentNullException.ThrowIfNull(window);
        LEngineSettingsChange(settings => settings with { LSettingsWindow = window });
    }

    public void LEngineVolumeSave(double volume)
    {
        double level = Math.Clamp(volume, 0, 1);
        LEngineSettingsChange(settings => settings with { LSettingsVolume = level });
    }

    public void LEngineRespellingSave(bool respelled)
    {
        LEngineSettingsChange(settings => settings with { LSettingsRespelled = respelled });
    }

    public void LEngineFrequencySave(bool frequency)
    {
        LEngineSettingsChange(settings => settings with { LSettingsFrequency = frequency });
    }

    public void LEngineMorphologySave(bool morphology)
    {
        lock (_lEngineGate)
        {
            LEngineSettingsChange(settings => settings with { LSettingsMorphology = morphology });
            if (!morphology)
            {
                LEngineInflectionClear();
            }
        }
    }

    private void LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (_lEngineGate)
        {
            _lEngineSettings = change(_lEngineSettings);
            LSettingsLoader.LSettingsLoaderSave(_lEngineWorkspace, _lEngineSettings);
        }
    }
}
