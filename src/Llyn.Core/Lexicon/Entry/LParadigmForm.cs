using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigmForm(
    string LParadigmFormText,
    IReadOnlyList<LInflectionMark> LParadigmFormMarks,
    string? LParadigmFormTip,
    int LParadigmFormSplit = 0)
{
    public IReadOnlyList<LParadigmPiece> LParadigmFormPiece { get; init; } = [];

    public IReadOnlyList<LParadigmPiece> LParadigmFormScan()
    {
        string text = LParadigmFormText;
        bool[] marked = new bool[text.Length];
        foreach (LInflectionMark mark in LParadigmFormMarks)
        {
            int offset = Math.Clamp(mark.LInflectionMarkOffset, 0, text.Length);
            int end = Math.Clamp(mark.LInflectionMarkOffset + mark.LInflectionMarkLength, offset, text.Length);
            Array.Fill(marked, true, offset, end - offset);
        }

        int split = LParadigmFormSplit;
        List<LParadigmPiece> pieces = [];
        int start = 0;
        for (int index = 1; index <= text.Length; index++)
        {
            if (index < text.Length && index != split && marked[index] == marked[start])
            {
                continue;
            }

            pieces.Add(new LParadigmPiece(text[start..index], marked[start], index == split && split < text.Length));
            start = index;
        }

        return pieces;
    }
}
