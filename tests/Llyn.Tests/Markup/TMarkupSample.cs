namespace Llyn.Tests;

internal static class TMarkupSample
{
    internal const string TMarkupSampleText = """
        <llyn>
          <entry>
            <headword>kindle</headword>
            <language>en</language>
            <speech>verb</speech>
            <speech>noun</speech>
            <form>
              <text>kindles</text>
              <local>kin·dles</local>
              <role>third</role>
            </form>
            <inflection>
              <text>kindled</text>
              <speech>verb</speech>
              <morphology>past</morphology>
              <morphology>participle</morphology>
            </inflection>
            <pronunciation>
              <ipa>ˈkɪndl̩</ipa>
              <variety>British</variety>
              <syllable>
                <onset>k</onset>
                <nucleus>ɪ</nucleus>
                <coda>n</coda>
                <tone>1</tone>
              </syllable>
              <audio>kindle.mp3</audio>
              <source>Wiktionary</source>
            </pronunciation>
            <transcription>
              <scheme>Respelling</scheme>
              <text>KIN-dl</text>
            </transcription>
            <meaning>
              <title>light a fire</title>
              <definition>set something alight</definition>
              <sentence>
                <particle>with</particle>
                <dependence state="unknown" />
                <example>
                  <text>She kindled the fire with dry twigs.</text>
                  <language>en</language>
                  <gloss>
                    <language>ko</language>
                    <text>그녀는 마른 잔가지로 불을 붙였다.</text>
                  </gloss>
                  <mention>
                    <offset>4</offset>
                    <length>7</length>
                    <headword>kindle</headword>
                    <language>en</language>
                    <sense>1</sense>
                  </mention>
                  <reference>
                    <title>Hearth Tales</title>
                    <year>1998</year>
                    <kind>book</kind>
                    <url>https://example.org/hearth</url>
                    <note>first edition</note>
                    <author>Ada Ember</author>
                    <author>Rhys Coal</author>
                  </reference>
                </example>
              </sentence>
              <situation>
                <title>camping</title>
                <description>outdoors at night</description>
                <kind>leisure</kind>
              </situation>
              <register>literary</register>
              <translation>
                <headword>불붙이다</headword>
                <language>ko</language>
              </translation>
              <tag>fire</tag>
              <image>
                <location>hearth.png</location>
              </image>
              <video>
                <location>hearth.mp4</location>
                <span>00:10-00:20</span>
              </video>
              <meaning>
                <title>ignite figuratively</title>
                <definition>arouse a feeling</definition>
              </meaning>
            </meaning>
            <collocation>
              <title>kindle interest</title>
              <expression>kindle someone's interest</expression>
              <definition>make someone curious</definition>
            </collocation>
            <note>Chiefly *literary*.</note>
          </entry>
        </llyn>
        """;
}
