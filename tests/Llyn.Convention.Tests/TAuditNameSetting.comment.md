# TAuditNameSetting.cs

## `internal static class TAuditNameSetting`

Hand-written and tracked: every naming-audit setting lives here and nowhere else.
No script writes this file, and no registered name may appear in it.
Bases, verbs, and exemptions reach the tests only through the generated TAuditNameRegistry.cs.
The tests read no script configuration, so no json or ps1 can change what they check.
They do read the untracked sources Git lists, since an unstaged file is still source.
