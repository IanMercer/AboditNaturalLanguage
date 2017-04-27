using AboditNLP;
using AboditNLP.Article;
using AboditNLP.Attributes;
using AboditNLP.Noun;
using AboditNLP.Verb;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// Wordnet integration makes it easy to recognize words that are synonyms of each other
    /// or to use a single interface to recognize entire classes of words.
    /// 
    /// Wordnet contains multiple definitions for the meanings of each word (or phrase) and
    /// each of these it returned by AboditNLP when the word is seen.
    /// 
    /// AboditNLP goes beyond what is in Wordnet to include verb tenses (and conjugation), and
    /// plurals, singulars and possessives for nouns. AboditNLP also adds many words that simply
    /// aren't in WordNet (articles (A, An, The, ...), question words (What, Where), and so on.
    /// 
    /// AboditNLP-Wordnet is built from the core Wordnet RDF files plus various extension files
    /// that add new words and new categorizations of words into groups with related meanings.
    /// 
    /// Currently AboditNLP-Wordnet includes all of Wordnet but I'm looking at carving out a
    /// reduced subset of the most common words as it's rather large.
    /// 
    /// AboditNLP-Wordnet also includes embedded resources containing the definitions from
    /// Wornet. These are useful to ensure you have the correct meaning (Synset) when
    /// recognizing a word. e.g. "tiger" could be "tiger-noun-1" or "tiger-noun-2" each
    /// of which has a different meaning and different synonyms. Did you want to regcognize
    /// a large feline or a person? The best way to see the different meanings for words is
    /// to type "define XYZ" and the rule below will dump out as much as it knows.
    /// 
    /// AboditNLP-Wordnet also contains a graph of words with relationships between
    /// synonyms, antonyms, part/whole relationships and type relationships. The graph
    /// is somewhat sparse and I'm looking at ways to expand it.
    /// </summary>
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// Dump an ambiguous set of words
        /// </summary>
        [Priority(2000)]
        public NLPActionResult GrammarDump(define2 define, IAmbiguous<ITokenText> tokenAmbiguous)
        {
            intent.GrammarDump(st, tokenAmbiguous);
            return NLPActionResult.None;
        }

        [Priority(1000)]
        public NLPActionResult GrammarDump(define2 define, ITokenText token)
        {
            intent.GrammarDump(st, token);
            return NLPActionResult.None;
        }

        // By using the interface Noun.Mammal1 (instead of Noun.mammal1) this rule recognizes instances of mammals
        // instead of the word mammal, so "Tiger", "Fox" will all cause this rule to execute.

        /// <summary>
        /// Recognize a class of words instead of just a single word
        /// Note the capital on "Mammal1". This will recognize lion, tiger, elephant, ...
        /// whereas "mammal1" would recognize just the word mammal or any synonyms of it.
        /// </summary>
        public NLPActionResult RecognizeMammals (Mammal1 mammal)
        {
            intent.RecognizeMammals(st, mammal);
            return NLPActionResult.None;
        }

        [Priority(10000)]   // over the non-ambiguous form
        public NLPActionResult QuestionIsaXaY(amIsAre isVerb,
            [Optional]AAn a, IAmbiguous<INoun> nounAmbiguous,
            [Optional]AAn a2, IAmbiguous<INoun> classAmbiguous)
        {
            intent.AnswerIsXaY(st, nounAmbiguous, classAmbiguous);
            return NLPActionResult.None;
        }

        [Priority(-20)]
        public NLPActionResult QuestionIsaXaY(amIsAre isVerb, 
            [Optional]AAn a, INoun noun, 
            [Optional]AAn a2, INoun @class)
        {
            intent.AnswerIsXaY(st, noun, @class);
            return NLPActionResult.None;
        }

        [Priority(-1000)]       // A rule of last resort!
        public NLPActionResult RecognizeAnyOtherNoun(INoun noun)
        {
            st.Say("Hmmm... I haven't been taught about " + noun.Plural.Text + " yet.");
            return NLPActionResult.None;
        }
    }
}
