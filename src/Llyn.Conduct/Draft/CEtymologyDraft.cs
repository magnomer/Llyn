using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CEtymologyDraft(string CEtymologyDraftText, IReadOnlyList<CMentionDraft> CEtymologyDraftMentions);
