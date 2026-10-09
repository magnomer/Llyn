# LCourierWarrant.cs
Hash: `f0cbd592b106bcaa`

## `public sealed class LCourierWarrant`

Pairs Llyn with Joplin by asking for a token and polling until the user decides.
It stands apart from `LCourierClerk` because pairing shares no push state but the busy flag.
The clerk keeps that flag, so a pairing never runs beside a push.

## `private const int LCourierWarrantPatience = 120;`

How many times a connect polls for the user's decision, one interval apart.
Two minutes leaves time to find Joplin's prompt without leaving a forgotten request polling forever.

## `private static readonly TimeSpan LCourierWarrantInterval = TimeSpan.FromSeconds(1);`

The wait before each poll, so a connect asks Joplin about once a second.

## `public LCourierWarrant(LOutpost outpost, LWarrant keeper, Action<Exception> fault)`

Takes the Joplin port, the warrant that hides a token, and the clerk's fault sink.
So a pairing failure leaves the same trace as a push failure.

## `public async Task<string> LCourierWarrantAttach(int stored, CancellationToken cancellation)`

Asks Joplin for a token and answers it hidden, the form the engine stores in settings.
`stored` is the port from the settings, tried before Joplin's own range.
A Joplin that answers on no port refuses with `LRefusalOutpost`, since no request can reach it.
Any other failure to start the request goes to `fault`, then refuses with `LRefusalOutpost`.
Each poll waits first, because the user needs time to see Joplin's prompt.
A poll that throws `TimeoutException` goes to `fault` and counts as still waiting.
`LCourierClerk.LCourierStall` such polls in a row refuse with `LRefusalOutpost`, since Joplin stopped answering.
Any poll that answers breaks the row.
A rejection refuses with `LRefusalWarrant`, and the user may simply try again.
Running out of polls refuses with `LRefusalPending`, so the request does not hang the shell.

## `public static bool LCourierWarrantCheck(Exception exception)`

Whether `exception` is the refusal of a token Joplin rejected or that cannot be restored.
The engine drops the stored token on it, so the shell offers Connect again.
Only the exception itself is read, since every courier refusal is thrown bare.
