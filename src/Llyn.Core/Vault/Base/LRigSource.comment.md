# LRigSource.cs
Hash: `443817afa7375534`

## `public sealed record LRigSource(LSourceFactory LRigSourceFactory, LFanqieSource LRigSourceFanqie, LShengfuSource LRigSourceShengfu, LReflexSource LRigSourceReflex, LScriptSource LRigSourceScript, LRecordingVault LRigSourceRecordings)`

The source group of the rig, holding the ports that fetch pronunciation data from outside.
It covers the source factory, the four fetchers and the recordings.
`LRig` holds it as `LRigSource`, so the rig stays small while each port keeps its own slot.
Every property is named `LRigSource{Base}`, the base being the port's own, as on the rig.
The composition root builds it through `LRigFactorySource`, and a test builds it from fakes.
