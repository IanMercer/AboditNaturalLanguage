using System.Linq;
using AboditNLP;
using AboditNLP.Attributes;
using AboditNLP.Verb;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// AboditNLP provides a couple of different techniques to handle words or sentences
    /// that are not immediately recognized.
    /// 
    /// First off, you can define rules that take approximately spelled tokens by using
    /// the Insensitive attribute from the AboditNLP.Attributes namespace.
    /// 
    /// Secondly, you can define rules with effectively a catch-all behavior using TokenPhrase.
    /// 
    /// Thirdly, you can check to see if no rule executed when you call Execute.
    /// 
    /// </summary>
    public partial class SampleRules : INLPRule
    {
        // Using a priority lower than the exact match rule and an edit distance of 1
        [Priority(100)]
        public NLPActionResult GrammarDumpWithSpellingMistake(define2 define, [AboditNLP.Attributes.Insensitive(caseInsensitive:true, editDistance:1)]IAmbiguous<ITokenText> token)
        {
            if (token.Count() == 1)
            {
                st.Say($"I think you perhaps meant 'define {token.First().Text}'");
                // But go ahead and show some more aggressive approximate matches anyway
                intent.DefineSuggestions(st, token.First().Text);
            }
            else
            {
                st.Say($"I think you perhaps meant 'define' and then one of {string.Join(", ", token.Select(x => x.Text))}");
            }
            return NLPActionResult.None;
        }

        // Using a priority lower than any other 'define' rule with an arbitrary phrase
        // this will handle edits that are even further out
        public NLPActionResult GrammarDump(define2 define, TokenPhrase phrase)
        {
            st.Say("Sorry " + phrase.Text + " isn't in my dictionary");
            intent.DefineSuggestions(st, phrase.Text);
            return NLPActionResult.None;
        }
    }
}
