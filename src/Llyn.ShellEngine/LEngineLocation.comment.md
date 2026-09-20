# LEngineLocation.cs

## `public sealed partial class LEngine`

The one rule for where a stored location may point.
Image, video and audio locations come from the user or from an imported file, and both are read here.
A location that fails the rule is never touched, since touching a path can already send credentials.

## `public Uri? LEngineLocationResolve(string? location)`

The address a location names, or null when it names nothing the program may reach.
A web address passes as written.
A drive path passes as written, and a relative path is resolved against the workspace and must stay inside it.
The shell resolves the same way, so a relative path means the same thing on view and on import.
A UNC path, a `file` address with a host, a device path and every other scheme are refused.
Windows sends the user's credentials to any host named that way, so an imported file must not name one.

## `private Uri? LEngineLocationResolve(string path, string workspace)`

The file half of the rule, once web and scheme forms are settled.
The trail port resolves the text under the workspace and answers nothing for a path the system cannot form.
An address the runtime cannot form from the resolved path is refused rather than thrown.

## `public Uri? LEngineLocationRead(string? location)`

The address a location names when something is there to show: a web address, or a file on disk.
An image or video row previews through this, so a missing file shows no preview rather than a broken one.

## `public void LEngineLocationOpen(string target)`

Opens a folder or a web address through the usher, so no shell starts a process.

## `private string LEngineRecordingResolve(string file)`

The full path of a stored recording within the workspace in use now.
A path the rule refuses is returned as it stands, so a draft keeps what was written.

## `public bool LEngineRecordingExist(string? file)`

Whether a stored recording passes the rule and is on disk.
