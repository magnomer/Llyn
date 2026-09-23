# TAuditCommentSetting.cs

## `internal static class TAuditCommentSetting`

Hand-written and tracked: the comment-audit rules and scope live here, not in a generated sidecar.
auditcomments.ps1 reads its own gitignored auditcomments.json and never writes this file.
Keep the two in agreement by hand, because the tests must depend on nothing untracked.

## `public static readonly string[] TAuditCommentFiles`

Repository-root files audited beside the roots, since a root of `.` would sweep docs and scratch folders.
Each needs its comment file at the root, and a listed `.props` file must carry no in-code comment.

## `public static readonly string[] TAuditCommentExempt`

TAuditNameRegistry.cs is generated and carries a generated-file header, so it alone is exempt.

## `public static readonly string[] TAuditCommentSources`

Lists the source extensions that need paired comment files.
Its patterns match the source pairs in scripts/auditcomments.json.
