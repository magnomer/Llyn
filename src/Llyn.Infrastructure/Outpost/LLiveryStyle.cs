using System;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LLiveryStyle
{
    public static string LLiveryStyleFormat(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        string ink = theme.LThemeRead("ink");
        string muted = theme.LThemeRead("muted");
        string line = theme.LThemeRead("line");
        string canvas = theme.LThemeRead("canvas");
        string surface = theme.LThemeRead("surface");
        string accent = theme.LThemeRead("accent");
        string soft = theme.LThemeRead("accentSoft");
        string situation = theme.LThemeRead("situation");
        string situationSoft = theme.LThemeRead("situationSoft");
        string situationEdge = theme.LThemeRead("situationEdge");
        string raised = theme.LThemeRead("surfaceRaised");
        string favorite = theme.LThemeRead("favorite");
        string helper = theme.LThemeRead("helper");
        string helperSoft = theme.LThemeRead("helperSoft");
        string helperEdge = theme.LThemeRead("helperEdge");
        string core = theme.LThemeRead("frequencyCore");
        string everyday = theme.LThemeRead("frequencyEveryday");
        string advanced = theme.LThemeRead("frequencyAdvanced");
        string rare = theme.LThemeRead("frequencyRare");
        string warning = theme.LThemeRead("warning");
        string table = ".llyn table:has(> thead > tr > th:empty)";
        string reading = ".llyn table:has(> thead > tr > th:nth-child(5):last-child:empty)";
        string sound = ".llyn table:not(.llyn-paradigm table)"
            + ":has(> thead > tr > th:nth-child(2):last-child:empty)";

        return $$"""
            .llyn { font-family: {{theme.LThemeFamily}}; color: {{ink}}; font-size: 15px; line-height: 1.5; }
            .llyn p { margin: 0 0 14px; }
            .llyn a { color: {{accent}}; text-decoration: none; }
            .llyn h1 { margin: 0 0 24px; padding: 0; border: 0; font-size: 40px; font-weight: 700; line-height: 1.2; }
            .llyn h2 { margin: 32px 0 12px; padding: 0; border: 0; font-size: 20px; font-weight: 600; }
            .llyn hr { margin: 28px 0 16px; border: 0; border-top: 1px solid {{line}}; }
            .llyn ul { margin: 0 0 12px; padding-left: 29px; }
            .llyn li { margin: 0 0 8px; }
            .llyn li::marker { color: {{muted}}; }
            .llyn img { max-width: 100%; }
            .llyn video { max-width: 100%; border-radius: 8px; }
            .llyn a[data-resource-id]:has(+ .media-player) { display: none; }
            .llyn .resource-icon { display: none; }
            .llyn .media-audio { height: 32px; width: 220px; margin: 0 0 0 6px; vertical-align: middle; }
            .llyn thead:not(:has(th:not(:empty))) { display: none; }
            {{table}} { width: auto; margin: 0 0 8px; border: 0; border-collapse: collapse; background: none; }
            {{table}} tr { border: 0; background: none; }
            {{table}} td { padding: 0 5px; border: 0; background: none; vertical-align: middle; }
            {{reading}} td { height: 31px; }
            {{reading}} td:nth-child(1) { color: {{muted}}; font-size: 11px; text-align: right; white-space: nowrap; }
            {{reading}} td:nth-child(2) { color: {{muted}}; font-size: 11px; text-align: right; white-space: nowrap; }
            {{reading}} td:nth-child(3) { font-size: 20px; padding-right: 24px; }
            {{reading}} td:nth-child(4) { color: {{muted}}; font-size: 13px; padding-right: 24px; }
            {{reading}} td:nth-child(5) { color: {{muted}}; font-size: 13px; }
            {{sound}} td { height: 37px; }
            {{sound}} td:first-child { color: {{muted}}; font-size: 11px; text-align: right; white-space: nowrap; }
            .llyn .llyn-more { margin: 0 0 14px; }
            .llyn .llyn-more > summary { list-style: none; cursor: pointer; padding-left: 143px; font-size: 12px; }
            .llyn .llyn-more > summary::-webkit-details-marker { display: none; }
            .llyn .llyn-main { color: {{accent}}; }
            .llyn .llyn-note { color: {{muted}}; font-size: 13px; }
            .llyn .llyn-tone { font-size: 0.6em; vertical-align: super; line-height: 0; }
            .llyn .llyn-accent { font-size: 18px; font-weight: 600; }
            .llyn .llyn-transcription { font-size: 18px; font-weight: 600; }
            .llyn span.llyn-glyph { font-size: 22px; }
            .llyn img.llyn-flag { height: 14px; margin-right: 4px; vertical-align: -1px; border-radius: 2px; }
            .llyn .llyn-contour {
                display: inline-block; padding: 8px; border: 1px solid {{line}}; border-radius: 8px;
                background: {{surface}};
            }
            .llyn .llyn-contour-chart { width: 108px; height: 114px; vertical-align: top; }
            .llyn h1 .llyn-language {
                display: inline-block; height: 25px; margin-left: 12px; padding: 0 10px; border-radius: 13px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; font-weight: 400; line-height: 25px;
                vertical-align: middle;
            }
            .llyn .llyn-heart {
                margin-left: 16px; color: {{line}}; font-size: 20px; font-weight: 400; vertical-align: middle;
            }
            .llyn .llyn-heart.llyn-on { color: {{favorite}}; }
            .llyn .llyn-star {
                position: relative; font-size: 17px; font-weight: 400; vertical-align: middle;
                color: color-mix(in srgb, {{accent}} 55%, {{surface}});
            }
            .llyn .llyn-star.llyn-unrated { color: color-mix(in srgb, {{muted}} 35%, {{surface}}); }
            .llyn .llyn-star.llyn-on { color: {{accent}}; }
            .llyn .llyn-star.llyn-half::before {
                content: "★"; position: absolute; left: 0; top: 0; width: 50%; overflow: hidden; color: {{accent}};
            }
            .llyn .llyn-rating {
                margin-left: 4px; color: {{muted}}; font-size: 12px; font-weight: 400; vertical-align: middle;
            }
            .llyn .llyn-speech {
                display: inline-block; height: 21px; margin-right: 6px; padding: 0 12px; border-radius: 11px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; line-height: 21px;
            }
            .llyn .llyn-unit {
                display: inline-block; height: 21px; margin-right: 6px; padding: 0 12px; border-radius: 11px;
                border: 1px solid {{soft}}; color: {{muted}}; font-size: 12px; line-height: 19px;
            }
            .llyn .llyn-frequency {
                display: inline-block; height: 28px; padding: 0 12px; border-radius: 14px;
                background: {{raised}}; color: {{muted}}; font-size: 12px; line-height: 28px;
            }
            .llyn .llyn-pip { font-size: 9px; color: {{line}}; }
            .llyn .llyn-pip.llyn-on { color: {{muted}}; }
            .llyn .llyn-frequency-core, .llyn .llyn-frequency-core .llyn-pip.llyn-on { color: {{core}}; }
            .llyn .llyn-frequency-everyday, .llyn .llyn-frequency-everyday .llyn-pip.llyn-on { color: {{everyday}}; }
            .llyn .llyn-frequency-advanced, .llyn .llyn-frequency-advanced .llyn-pip.llyn-on { color: {{advanced}}; }
            .llyn .llyn-frequency-rare, .llyn .llyn-frequency-rare .llyn-pip.llyn-on { color: {{rare}}; }
            .llyn .llyn-paradigm {
                display: inline-block; margin: 0 0 14px; padding: 8px 15px; border: 1px solid {{line}};
                border-radius: 6px; background: {{raised}};
            }
            .llyn .llyn-paradigm table { margin: 0; }
            .llyn .llyn-paradigm td { height: 23px; padding: 0 14px 0 0; }
            .llyn .llyn-label { color: {{muted}}; font-size: 12px; }
            .llyn table.llyn-inflection {
                width: auto; margin: 0 0 14px; border: 0; border-collapse: collapse; background: none;
            }
            .llyn .llyn-inflection tr { border: 0; background: none; }
            .llyn .llyn-inflection th, .llyn .llyn-inflection td {
                height: 27px; padding: 0 18px 0 0; border: 0; background: none; text-align: left;
                white-space: nowrap; vertical-align: middle;
            }
            .llyn .llyn-inflection th {
                border-bottom: 1px solid {{line}}; color: {{muted}}; font-size: 12px; font-weight: 400;
            }
            .llyn .llyn-inflection td { font-size: 15px; }
            .llyn .llyn-inflection tr.llyn-rule td:not(:first-child) { border-top: 1px dotted {{line}}; }
            .llyn .llyn-inflection tr.llyn-close td { border-top: 1px solid {{line}}; }
            .llyn .llyn-inflection td.llyn-group { color: {{accent}}; font-size: 14px; font-weight: 600; }
            .llyn .llyn-inflection td.llyn-label { font-size: 12px; }
            .llyn .llyn-inflection .llyn-marked { color: {{warning}}; }
            .llyn .llyn-inflection .llyn-cut { color: {{muted}}; }
            .llyn .llyn-inflection .llyn-muted { color: {{muted}}; }
            .llyn .llyn-inflection-box {
                display: inline-block; margin: 0 0 14px; padding: 14px 16px; border: 1px solid {{line}};
                border-radius: 12px; background: {{surface}};
            }
            .llyn .llyn-inflection-box table.llyn-inflection { margin: 0; }
            .llyn .llyn-inflection-full > summary {
                display: block; width: fit-content; margin: 0 0 10px auto; padding: 2px; border-radius: 9px;
                background: {{raised}}; text-align: right; list-style: none; cursor: pointer;
                font-size: 12px; font-weight: 600; line-height: 20px;
            }
            .llyn .llyn-inflection-full > summary::-webkit-details-marker { display: none; }
            .llyn .llyn-inflection-full > summary > span {
                display: inline-block; padding: 2px 11px; border: 1px solid {{raised}}; border-radius: 7px;
                color: {{muted}};
            }
            .llyn .llyn-inflection-full > summary > span:hover { color: {{ink}}; }
            .llyn .llyn-inflection-full:not([open]) > summary > .llyn-inflection-switch-short,
            .llyn .llyn-inflection-full[open] > summary > .llyn-inflection-switch-long {
                border-color: {{line}}; background: {{surface}}; color: {{accent}};
            }
            .llyn .llyn-inflection-full[open] + .llyn-inflection-short { display: none; }
            .llyn .llyn-card {
                margin: 0 0 28px; padding: 14px; border: 1px solid {{line}}; border-radius: 8px;
                background: {{surface}};
            }
            .llyn .llyn-card > :last-child { margin-bottom: 0; }
            .llyn .llyn-card table { width: 100%; margin: 0; }
            .llyn .llyn-card td { height: 24px; padding: 0 6px; font-size: 14px; white-space: nowrap; }
            .llyn .llyn-card td:first-child { width: 92px; padding-left: 0; }
            .llyn .llyn-card td:nth-child(2) { font-size: 16px; }
            .llyn .llyn-card td:last-child { width: 100%; padding-right: 0; text-align: right; }
            .llyn .llyn-card .llyn-tone {
                color: {{accent}}; font-size: 13px; line-height: inherit; vertical-align: baseline;
            }
            .llyn .llyn-heading {
                display: inline-block; height: 24px; padding: 0 13px; border-radius: 12px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; line-height: 24px;
            }
            .llyn .llyn-book {
                display: inline-block; height: 24px; padding: 0 13px; border-radius: 12px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; line-height: 24px;
            }
            .llyn .llyn-series {
                display: inline-block; height: 24px; padding: 0 10px; border-radius: 12px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; line-height: 24px;
            }
            .llyn .llyn-stem {
                display: inline-block; height: 23px; padding: 0 6px; border-radius: 6px;
                background: {{soft}}; color: {{accent}}; font-size: 16px; line-height: 23px;
            }
            .llyn .llyn-initial {
                display: inline-block; height: 17px; padding: 0 4px; border-radius: 4px;
                background: {{soft}}; color: {{accent}}; font-size: 13px; line-height: 17px;
            }
            .llyn .llyn-rime {
                display: inline-block; height: 17px; padding: 0 4px; border-radius: 4px;
                background: {{soft}}; color: {{accent}}; font-size: 13px; line-height: 17px;
            }
            .llyn .llyn-medial { color: {{muted}}; font-size: 13px; }
            .llyn .llyn-medial.llyn-on { color: {{ink}}; }
            .llyn .llyn-spelling { color: {{muted}}; font-size: 12px; }
            .llyn .llyn-source {
                display: inline-block; height: 20px; padding: 0 8px; border: 1px solid {{line}}; border-radius: 10px;
                background: {{surface}}; color: {{muted}}; font-size: 11px; line-height: 18px;
            }
            .llyn .llyn-style {
                display: inline-block; height: 24px; margin-right: 20px; padding: 0 10px; border-radius: 12px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; line-height: 24px; vertical-align: top;
            }
            .llyn .llyn-figure { display: inline-block; margin: 0 8px 8px 0; text-align: center; vertical-align: top; }
            .llyn img.llyn-glyph { display: block; height: 64px; margin: 0 auto; }
            .llyn .llyn-caption { display: block; color: {{muted}}; font-size: 10px; line-height: 14px; }
            .llyn .llyn-epoch { display: block; }
            .llyn .llyn-quote { display: block; margin-left: 64px; font-size: 12px; }
            .llyn .llyn-card:has(> p > .llyn-number), .llyn .llyn-card:has(> summary > .llyn-number) {
                padding: 14px 30px 20px;
            }
            .llyn .llyn-card > p:has(> .llyn-number) {
                margin: -14px -30px 20px; padding: 14px 23px; border-bottom: 1px solid {{line}};
            }
            .llyn .llyn-card > summary {
                display: block; position: relative; list-style: none; margin: -14px -30px 20px; padding: 14px 23px;
                border-bottom: 1px solid {{line}};
                cursor: pointer;
            }
            .llyn .llyn-card > summary::-webkit-details-marker { display: none; }
            .llyn .llyn-card > summary::after {
                content: ""; position: absolute; top: 50%; right: 23px; width: 8px; height: 8px;
                border-right: 2px solid {{muted}}; border-bottom: 2px solid {{muted}};
            }
            .llyn .llyn-card[open] > summary::after { transform: translateY(-25%) rotate(-135deg); }
            .llyn .llyn-card:not([open]) > summary::after { transform: translateY(-75%) rotate(45deg); }
            .llyn .llyn-card:not([open]) > summary { margin-bottom: -20px; border-bottom: 0; }
            .llyn .llyn-number {
                display: inline-block; width: 26px; height: 26px; margin-right: 12px; border-radius: 13px;
                background: {{soft}}; color: {{accent}}; font-size: 12px; line-height: 26px; text-align: center;
            }
            .llyn .llyn-title { font-size: 16px; font-weight: 500; }
            .llyn .llyn-title.llyn-blank { color: {{muted}}; }
            .llyn span.llyn-blank:empty {
                display: inline-block; width: 64px; height: 64px; border: 1px solid {{line}}; border-radius: 6px;
                background: {{canvas}}; vertical-align: top;
            }
            .llyn .llyn-expression { color: {{muted}}; font-size: 14px; }
            .llyn .llyn-situation {
                display: inline-block; height: 23px; margin-right: 6px; padding: 0 10px;
                border: 1px solid {{situationEdge}}; border-radius: 12px; background: {{situationSoft}};
                color: {{situation}}; font-size: 13px; line-height: 21px;
            }
            .llyn .llyn-register {
                display: inline-block; height: 23px; margin-right: 6px; padding: 0 10px;
                border: 1px solid {{helperEdge}}; border-radius: 12px; background: {{helperSoft}};
                color: {{helper}}; font-size: 13px; line-height: 21px;
            }
            .llyn .llyn-tag {
                display: inline-block; height: 25px; margin-right: 6px; padding: 0 10px; border: 1px solid {{line}};
                border-radius: 13px; background: {{surface}}; color: {{muted}}; font-size: 12px; line-height: 23px;
            }
            .llyn .llyn-card li { font-family: {{theme.LThemeSerif}}; }
            .llyn .llyn-particle { margin-right: 8px; color: {{accent}}; font-weight: 600; }
            .llyn .llyn-dependence { margin-right: 8px; color: {{accent}}; font-weight: 600; }
            .llyn .llyn-byline { float: right; color: {{muted}}; font-family: {{theme.LThemeSerif}}; }
            .llyn .llyn-gloss { color: {{muted}}; font-family: {{theme.LThemeSerif}}; font-style: italic; }
            .llyn .llyn-target {
                display: inline-block; height: 25px; margin-right: 6px; padding: 0 10px; border-radius: 13px;
                background: {{soft}}; color: {{accent}}; line-height: 25px;
            }
            .llyn .llyn-language { color: {{muted}}; font-size: 12px; }
            .llyn .llyn-target .llyn-language { margin-left: 4px; font-size: 11px; }
            .llyn .llyn-epithet { margin-left: 6px; color: {{muted}}; font-size: 13px; }
            .llyn .llyn-owner {
                display: inline-block; height: 17px; margin: 0 8px; padding: 0 8px; border-radius: 9px;
                background: {{soft}}; color: {{accent}}; font-size: 11px; line-height: 17px;
            }
            .llyn img.llyn-image { display: block; border-radius: 8px; }
            .llyn .media-video { display: block; max-width: 100%; margin: 0; }
            .llyn .llyn-span { color: {{muted}}; font-size: 12px; }
            .llyn p:has(> .llyn-stamp) { color: {{muted}}; font-size: 12px; line-height: 19px; }
            .llyn .llyn-stamp { display: inline-block; width: 63px; }
            .llyn .llyn-vacant { color: {{muted}}; font-size: 13px; }
            .llyn .llyn-diwei {
                margin: 0 0 20px; padding: 14px; border: 1px solid {{line}}; border-radius: 8px;
                background: {{surface}};
            }
            .llyn .llyn-diwei > :last-child { margin-bottom: 0; }
            .llyn .llyn-diwei table { margin: 10px 0 0; }
            .llyn .llyn-diwei td { height: 28px; padding: 0 14px 0 0; }
            .llyn .llyn-diwei td:first-child { font-weight: 600; white-space: nowrap; }
            .llyn .llyn-diwei td:last-child { width: 100%; padding-right: 0; }
            .llyn .llyn-rounded { color: {{situation}}; font-weight: 600; }
            .llyn .llyn-mark {
                display: inline-block; height: 23px; margin-right: 6px; padding: 0 8px; border: 1px solid {{line}};
                border-radius: 12px; background: {{surface}}; font-size: 13px; line-height: 21px;
            }
            .llyn .llyn-count { margin-left: 2px; color: {{muted}}; font-size: 11px; }
            """;
    }
}
