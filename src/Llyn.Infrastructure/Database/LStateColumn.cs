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

    public static void LStateColumnApply(SqliteCommand command, string field, LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(value);

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

        command.Parameters.AddWithValue($"${field}State", LStateColumnFormat(anchor.LStateAnchorState));
        command.Parameters.AddWithValue(
            $"${field}",
            anchor.LStateAnchorState == LState.LStateSpecified && anchor.LStateAnchorId is not null
                ? anchor.LStateAnchorId.Value
                : DBNull.Value);
    }

    public static LStateAnchor LStateColumnResolve(SqliteDataReader reader, int state)
    {
        ArgumentNullException.ThrowIfNull(reader);

        return new LStateAnchor(
            LStateColumnParse(reader.GetString(state)),
            reader.IsDBNull(state + 1) ? null : reader.GetInt64(state + 1));
    }

    public static LStateValue LStateColumnRead(SqliteDataReader reader, int state)
    {
        ArgumentNullException.ThrowIfNull(reader);

        return new LStateValue(
            LStateColumnParse(reader.GetString(state)),
            reader.IsDBNull(state + 1) ? null : reader.GetString(state + 1));
    }
}
