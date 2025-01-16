using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Parses a string for special text tags that are commonly used in Dialogue boxes.
    /// </summary>
    public static class TextTagParser
    {
        public static string GetTagHelp()
        {
            return "" + "\n\n\t- - - - - - - -Tag Help- - - - - - - - \n\n" +
                            "\t{b} Bold Text {/b}\n" +
                            "\t{i} Italic Text {/i}\n" +
                            "\t{color=red} Color Text (color){/color}\n" +
                            "\t{size=30} Text size {/size}\n" +
                            "\n" +
                            "\t{s}, {s=60} Writing speed (chars per sec){/s}\n" +
                            "\t{w}, {w=0.5} Wait (seconds)\n" +
                            "\t{wi} Wait for input\n" +
                            "\t{wc} Wait for input and clear\n" +
                            "\t{wvo} Wait for voice over line to complete\n" +
                            "\t{wp}, {wp=0.5} Wait on punctuation (seconds){/wp}\n" +
                            "\t{c} Clear\n" +
                            "\t{x} Exit, advance to the next command without waiting for input\n" +
                            "\n" +
                            "\t{vpunch=10,0.5} Vertically punch screen (intensity,time)\n" +
                            "\t{hpunch=10,0.5} Horizontally punch screen (intensity,time)\n" +
                            "\t{punch=10,0.5} Punch screen (intensity,time)\n" +
                            "\t{flash=0.5} Flash screen (duration)\n" +
                            "\n" +
                            "\t{audio=AudioObjectName} Play Audio Once\n" +
                            "\t{audioloop=AudioObjectName} Play Audio Loop\n" +
                            "\t{audiopause=AudioObjectName} Pause Audio\n" +
                            "\t{audiostop=AudioObjectName} Stop Audio\n" +
                            "\n" +
                            "\t{m=MessageName} Broadcast message\n" +
                            "\t{$VarName} Substitute variable\n" +
                            "\n" +
                            "\t-------- Text Mesh Pro Tags --------\n" +
                            "\n" +
                            "\t<align=\"right\"> Right </align> <align=\"center\"> Center </align> <align=\"left\"> Left </align>\n" +
                            "\t<color=\"red\"> Red </color> <color=#005500> Dark Green </color>\n" +
                            "\t<alpha=#88> 88 </alpha>\n" +
                            "\t<i> Italic text </i>\n" +
                            "\t<b> Bold text </b>\n" +
                            "\t<cspace=1em> Character spacing </cspace>\n" +
                            "\t<font=\"FontName\"> Change font </font>\n" +
                            "\t<font=\"FontName\" material=\"MaterialName\"> Change font and material </font>\n" +
                            "\t<indent=15%> Indentation </indent>\n" +
                            "\t<line-height=100%> Line height </line-height>\n" +
                            "\t<line-indent=15%> Line indentation </line-indent>\n" +
                            "\t{link=id}link text{/link} <link=id>link text</link>\n" +
                            "\t<lowercase> Lowercase </lowercase>\n" +
                            "\t<uppercase> Uppercase </uppercase>\n" +
                            "\t<smallcaps> Smallcaps </smallcaps>\n" +
                            "\t<margin=5em> Margin </margin>\n" +
                            "\t<mark=#ffff00aa> Mark (Highlight) </mark>\n" +
                            "\t<mspace=2.75em> Monospace </mspace>\n" +
                            "\t<noparse> <b> </noparse>\n" +
                            "\t<nobr> Non-breaking spaces </nobr>\n" +
                            "\t<page> Page break\n" +
                            "\t<size=50%> Font size </size>\n" +
                            "\t<space=5em> Horizontal space\n" +
                            "\t<space=5em> Horizontal space\n" +
                            "\t<sprite=\"AssetName\" index=0> Sprite\n" +
                            "\t<s> Strikethrough </s>\n" +
                            "\t<u> Underline </u>\n" +
                            "\t<style=\"StyleName\"> Styles </style>\n" +
                            "\t<sub> Subscript </sub>\n" +
                            "\t<sup> Superscript </sup>\n" +
                            "\t<voffset=1em> Vertical offset </voffset>\n" +
                            "\t<width=60%> Text width </width>\n";
        }

        const string TextTokenRegexString = @"\{.*?\}";

        public static List<TextTagToken> Tokenise(string text)
        {
            List<TextTagToken> tokens = new List<TextTagToken>();

            // Although hard coded we are using a regex to parse the text
            Regex mr = new Regex(TextTokenRegexString);

            // Find first match in text
            Match m = mr.Match(text);

            int position = 0;
            while (m.Success)
            {
                // Get text leading up to the tag (pre text)
                string preText = text.Substring(position, m.Index - position);
                // Find the actual tag text
                string tagText = m.Value;

                if (preText != "")
                {
                    // Found an actual word that needs processing
                    AddWordsToken(tokens, preText);
                }
                // Now we have the tag text we can add it
                AddTagToken(tokens, tagText);

                position = m.Index + m.Length;
                m = m.NextMatch();
            }

            if (position < text.Length)
            {
                // Now we can find text after the last tag
                string postText = text.Substring(position, text.Length - position);
                if (postText != "")
                {
                    AddWordsToken(tokens, postText);
                }
            }

            // Remove any whitespace or newlines for readability
            bool trimLeading = false;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (trimLeading && token.type == TokenType.Words)
                {
                    // Remove whitespace or newlines from the start of the token
                    token.paramList[0] = token.paramList[0].TrimStart(' ', '\t', '\r', '\n');
                }
                if (token.type == TokenType.Clear || token.type == TokenType.WaitForInputAndClear)
                {
                    trimLeading = true;
                }
                else
                {
                    trimLeading = false;
                }
            }
            return tokens;
        }

        private static void AddWordsToken(List<TextTagToken> tokens, string words)
        {
            var token = new TextTagToken();
            token.type = TokenType.Words;
            token.paramList = new List<string> { words };
            tokens.Add(token);
        }

        private static void AddTagToken(List<TextTagToken> tokens, string tagText)
        {
            // Check to see if the tag is valid
            if (tagText.Length < 3 ||
                tagText.Substring(0, 1) != "{" ||
                tagText.Substring(tagText.Length - 1, 1) != "}")
            {
                return;
            }

            // Get the tag between the braces
            string tag = tagText.Substring(1, tagText.Length - 2);

            var type = TokenType.Invalid;
            List<string> parameters = ExtractParameters(tag);

            if (tag == "b")
            {
                type = TokenType.BoldStart;
            }
            else if (tag == "/b")
            {
                type = TokenType.BoldEnd;
            }
            else if (tag == "i")
            {
                type = TokenType.ItalicStart;
            }
            else if (tag == "/i")
            {
                type = TokenType.ItalicEnd;
            }
            else if (tag.StartsWith("color="))
            {
                type = TokenType.ColorStart;
            }
            else if (tag == "/color")
            {
                type = TokenType.ColorEnd;
            }
            else if (tag.StartsWith("size="))
            {
                type = TokenType.SizeStart;
            }
            else if (tag == "/size")
            {
                type = TokenType.SizeEnd;
            }
            else if (tag == "wi")
            {
                type = TokenType.WaitForInputNoClear;
            }
            else if (tag == "wc")
            {
                type = TokenType.WaitForInputAndClear;
            }
            else if (tag == "wvo")
            {
                type = TokenType.WaitForVoiceOver;
            }
            else if (tag.StartsWith("wp="))
            {
                type = TokenType.WaitOnPunctuationStart;
            }
            else if (tag == "wp")
            {
                type = TokenType.WaitOnPunctuationStart;
            }
            else if (tag == "/wp")
            {
                type = TokenType.WaitOnPunctuationEnd;
            }
            else if (tag.StartsWith("w="))
            {
                type = TokenType.Wait;
            }
            else if (tag == "w")
            {
                type = TokenType.Wait;
            }
            else if (tag == "c")
            {
                type = TokenType.Clear;
            }
            else if (tag.StartsWith("s="))
            {
                type = TokenType.SpeedStart;
            }
            else if (tag == "s")
            {
                type = TokenType.SpeedStart;
            }
            else if (tag == "/s")
            {
                type = TokenType.SpeedEnd;
            }
            else if (tag == "x")
            {
                type = TokenType.Exit;
            }
            else if (tag.StartsWith("m="))
            {
                type = TokenType.Message;
            }
            else if (tag.StartsWith("vpunch") ||
                     tag.StartsWith("vpunch="))
            {
                type = TokenType.VerticalPunch;
            }
            else if (tag.StartsWith("hpunch") ||
                     tag.StartsWith("hpunch="))
            {
                type = TokenType.HorizontalPunch;
            }
            else if (tag.StartsWith("punch") ||
                     tag.StartsWith("punch="))
            {
                type = TokenType.Punch;
            }
            else if (tag.StartsWith("flash") ||
                     tag.StartsWith("flash="))
            {
                type = TokenType.Flash;
            }
            else if (tag.StartsWith("audio="))
            {
                type = TokenType.Audio;
            }
            else if (tag.StartsWith("audioloop="))
            {
                type = TokenType.AudioLoop;
            }
            else if (tag.StartsWith("audiopause="))
            {
                type = TokenType.AudioPause;
            }
            else if (tag.StartsWith("audiostop="))
            {
                type = TokenType.AudioStop;
            }
            else if (tag.StartsWith("link="))
            {
                type = TokenType.LinkStart;
            }
            else if (tag.StartsWith("/link"))
            {
                type = TokenType.LinkEnd;
            }
            else if (tag.StartsWith("varc="))
            {
                type = TokenType.VariableCondition;
            }

            if (type != TokenType.Invalid)
            {
                TextTagToken token = new TextTagToken();
                token.type = type;
                token.paramList = parameters;
                tokens.Add(token);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Invalid text tag " + tag);
            }
        }

        private static List<string> ExtractParameters(string input)
        {
            List<string> paramList = new List<string>();

            // All parameters are comma separated and assigned value using '='
            int index = input.IndexOf('=');

            if (index == -1)
            {
                return paramList;
            }

            string paramString = input.Substring(index + 1);
            var splits = paramString.Split(',');
            foreach (var split in splits)
            {
                paramList.Add(split.Trim());
            }
            return paramList;
        }
    }
}
