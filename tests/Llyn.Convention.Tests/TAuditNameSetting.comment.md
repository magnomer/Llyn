# TAuditNameSetting.cs

## `internal static class TAuditNameSetting`

Hand-written and tracked: every naming-audit setting lives here and nowhere else.
No script writes this file, and no registered name may appear in it.
Bases, verbs, and exemptions reach the tests only through the generated TAuditNameRegistry.cs.
The tests read nothing untracked, so a gitignored json or ps1 can never change what they check.
