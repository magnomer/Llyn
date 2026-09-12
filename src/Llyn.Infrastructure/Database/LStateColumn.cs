using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LStateColumn
{
    public static string LStateColumnFormat(LState state)
    {
        return state switch
        {
            LState.LStateUnknown => "unknown",
            LState.LStateSpecified => "specified",
            _ => "unspecified",
        };
    }

    public static LState LStateColumnParse(string state)
    {
        return state switch
        {
            "unknown" => LState.LStateUnknown,
            "specified" => LState.LStateSpecified,
            _ => LState.LStateUnspecified,
        };
    }

    public static bool LStateColumnCheck(string state)
    {
        return state is "unspecified" or "unknown" or "specified";
    }

    public static void LStateColumnApply(SqliteCommand command, string field, LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(value);

        if (value.LStateValueUnreadable)
        {
            throw new LRefusal(LRefusal.LRefusalUnreadable);
        }

        command.Parameters.AddWithValue($"${field}State", LStateColumnFormat(value.LStateValueState));
        command.Parameters.AddWithValue(
            $"${field}",
            value.LStateValueState == LState.LStateSpecified && value.LStateValueText is not null
                ? value.LStateValueText
                : DBNull.Value);
    }

    public static void LStateColumnApply(SqliteCommand command, string field, LStateAnchor anchor)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(anchor);

        if (anchor.LStateAnchorUnreadable)
        {
            throw new LRefusal(LRefusal.LRefusalUnreadable);
        }

        command.Parameters.AddWithValue($"${field}State", LStateColumnFormat(anchor.LStateAnchorState));
        command.Parameters.AddWithValue(
            $"${field}",
            anchor.LStateAnchorState == LState.LStateSpecified && anchor.LStateAnchorId is not null
                ? anchor.LStateAnchorId.Value
                : DBNull.Value);
    }

    public static void LStateColumnApply(SqliteCommand command, string field, LStateMark mark)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(mark);

        if (mark.LStateMarkUnreadable)
        {
            throw new LRefusal(LRefusal.LRefusalUnreadable);
        }

        command.Parameters.AddWithValue($"${field}State", LStateColumnFormat(mark.LStateMarkState));
    }

    public static LStateMark LStateColumnLoad(SqliteDataReader reader, int state)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string stored = reader.GetString(state);
        if (!LStateColumnCheck(stored))
        {
            return new LStateMark(LState.LStateUnspecified, LStateMarkUnreadable: true);
        }

        return LStateMark.LStateMarkRead(LStateColumnParse(stored));
    }

    public static LStateAnchor LStateColumnResolve(SqliteDataReader reader, int state)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string stored = reader.GetString(state);
        if (!LStateColumnCheck(stored))
        {
            return new LStateAnchor(LState.LStateUnspecified, null, LStateAnchorUnreadable: true);
        }

        return new LStateAnchor(
            LStateColumnParse(stored),
            reader.IsDBNull(state + 1) ? null : reader.GetInt64(state + 1));
    }

    public static LStateValue LStateColumnRead(SqliteDataReader reader, int state)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string stored = reader.GetString(state);
        string? text = reader.IsDBNull(state + 1) ? null : reader.GetString(state + 1);
        if (!LStateColumnCheck(stored))
        {
            return new LStateValue(LState.LStateUnspecified, text ?? stored, LStateValueUnreadable: true);
        }

        return new LStateValue(LStateColumnParse(stored), text);
    }
}
