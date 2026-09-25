# TPhonographFake.cs

## `internal sealed class TPhonographFake : LPhonograph`

The phonograph every test engine is built over, which plays nothing.
A test never opens a platform player, so no run makes a sound.

## `public List<string> TPhonographFakePlayed { get; }`

Every file the phonograph was asked to play, in order.
A test reads it to prove a call did or did not reach the player.
