# LShengfuSourceHttp.cs

## `public sealed class LShengfuSourceHttp : LShengfuSource`

The adapter that reads a character's phonetic series off the web under a pack rule.
It carries no language knowledge: the address and the regex both come from the rule.

## `public LShengfuSourceHttp(HttpClient client)`

Takes the one client the rig shares, so every fetch keeps the same agent and timeout.

## `public async Task<(LShengfu? LShengfuFound, bool LShengfuReached)> LShengfuSourceFind(`

Fetches the page of the character and reads the series out of it.
A missing page counts as answered with nothing, so the character is not fetched again.
A refused or unreachable page counts as unanswered, so a later display retries it.

## `public static LShengfu? LShengfuSourceScan(LShengfuRule rule, string character, string body)`

Runs the rule's pattern over the body and keeps each distinct `shengfu` group in order.
Several series of one character are joined by the rule's separator into one text.
Markup and entities are stripped first, so the text reads as the site printed it.
