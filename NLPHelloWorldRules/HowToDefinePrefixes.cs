using AboditNLP;
using AboditNLP.Attributes;
using AboditNLP.Verb;
using AboditNLP.Pronoun;
using AboditNLP.Preposition;
using AboditNLP.Question;
using AboditNLP.Punctuation;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// The prefix in this case is any collection of words a user might add in front of a request like
    /// 'please' or 'please can you' or 'can you please', ... Using a Production Rule all of these
    /// phrases can be mapped to a single new token which can then be used in other rules (production or complete sentence rules)
    /// </summary>
    public class Prefix
    {
        /// <summary>
        /// The original text
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// You could also store the planned response prefix if you wanted it to match the input, say
        /// </summary>
        public string OptionalResponsePrefix { get; set; }
    }


    public partial class SampleRules : INLPRule
    {
        [ProductionRule]
        public Prefix PleaseTellMeAbout([Optional]please1 please, tell3 tell, me me, aboutOrAround2 about)
        {
            return new Prefix { Original = "please tell me about", OptionalResponsePrefix = "Certainly," };
        }

        [ProductionRule]
        public Prefix WhatIs(what what, amIsAre isAre)
        {
            return new Prefix { Original = "what is", OptionalResponsePrefix = "" };
        }

        [ProductionRule]
        public Prefix Hello(Noun.hello hello, [Optional]comma comma)
        {
            // In this case we want to use the same greeting back, i.e. Hello->Hello, Hi->Hi, ...
            return new Prefix { Original = hello.Text + ",", OptionalResponsePrefix = hello.Text + "," };
        }
    }
}
