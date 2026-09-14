# LEngineMarkupReplace.cs

## `public sealed partial class LEngine`

The Replace half of markup import that reaches outside the target.
The rewrite itself is the plain update path, and only the incoming sense mentions need settling first.

## `private void LEngineMarkupDetach(long entryId, List<LMarkupOmission> omissions)`

Before a Replace, every Mention citing one of the target's senses drops the sense through `LMentionSenseClear`.
The senses are about to be deleted, and a Mention left on a vanished sense would say nothing.
One omission names each affected Example once, however many senses it cited.
