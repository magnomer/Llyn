# LFreedom.cs
Hash: `a9233b2e06da7c36`

## `public enum LFreedom`

Whether the entry's unit can be uttered alone as a response, kept apart from the unit itself.
The user chooses it, and any freedom goes with any unit.
The numbers are stored in `entry.freedom`, so a member never changes its number.

- `LFreedomEmpty` — Not chosen yet.
- `LFreedomDependent` — Cannot be uttered alone as a response.
- `LFreedomStandalone` — Can be uttered alone as a response and understood by the listener.
