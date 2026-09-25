using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LPortraitText
{
    public static string LPortraitTextRead(LStateValue? value, string mark)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value.LStateValueState switch
        {
            _ when value.LStateValueUnreadable => value.LStateValueShow(),
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => mark,
            _ => string.Empty,
        };
    }

    public static string LPortraitTitleRead(LStateValue value, string vacant, string mark)
    {
        ArgumentNullException.ThrowIfNull(vacant);

        string text = LPortraitTextRead(value, mark);
        return text.Length > 0 ? text : vacant;
    }
}
