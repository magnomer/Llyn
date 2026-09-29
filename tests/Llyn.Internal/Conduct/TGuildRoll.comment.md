# TGuildRoll.cs

## `public sealed class TGuildRoll`

Covers the guild's one roll answer on a real workspace.
The rows, the empty verdict and the vita arrive together, so the view reads once.
A query matching nothing answers empty, since a queried roll drops the uncredited row.
A chosen Author gone from the roll closes the panel before the vita is read, so the vita is nobody's.
Each row carries its icon key and its worded citation count, and each fellow its worded shared count.
