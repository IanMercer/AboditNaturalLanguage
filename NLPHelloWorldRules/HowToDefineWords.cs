using AboditNLP;

// ReSharper disable InconsistentNaming

// Typically you will define each of your synonym interfaces in namespaces like the ones provided
// for Noun, Verb, Adjective, ... You can use inheritance on these to define classes of words that
// you want to recognize in one group, e.g. all positive adjectives
//
// We use .NET interfaces and not .NET classes, or enums or strings because we need 
// multiple-inheritance to represent language concepts.
//
// Your tokens will derive from Noun, Singular, Plural, Verb, Present, Past, ... ProperNoun ...
// These exist in namespaces Noun, Verb, ...

// By convention we use lower case interface names in these namespaces to indicate specific instances
// of a word or meaning and capitalized interface names in these namespaces to indicate a class of meanings
// e.g. `dog1` is a dog, but `Dog1` is a category of words which can all mean `dog`. 

namespace Noun
{
    public interface hello : INoun { }
    public interface world : INoun { }
    public interface goodbye : INoun { }

    // You can also distinguish between singular or plural nouns by using interface inheritance like this:

    public interface report : AboditNLP.Noun.Type.Singular { }
}

// When it comes to verbs you might want to define specific tenses of them, or have some catch-all interface that handles 
// any tense of a given verb (or verb synset)

namespace Verb
{
    public interface report : AboditNLP.Verb.Tense.Present { }
    public interface reported : AboditNLP.Verb.Tense.Past { }
    public interface debug : AboditNLP.Verb.Tense.Present { }
}

namespace NLPHelloWorldRules
{
    // But you can also just define them in whatever namespace you want
    public interface CompanyName : AboditNLP.Noun.Type.ProperNoun { }

    // And you can use inheritance on your interfaces:

    public interface Microsoft : CompanyName { }
    public interface Google : CompanyName { }
    public interface Facebook : CompanyName { }

    /// <summary>
    /// To store words in AboditNLP's built-in dictionary you need to implement an `ILexemeGenerator`
    /// </summary>
    public class SampleWords : ILexemeGenerator
    {
        /// <summary>
        /// Use any short prefix for the Uri for your words
        /// </summary>
        const string dummyNamespace = "yns:";

        public void CreateWords(ILexemeStore store)
        {
            // For Uri's you can use anything.  I suggest using a terse form of Uri
            // like that used in Turtle/N3 (http://www.w3.org/TeamSubmission/turtle/)

            // Each word is define using a fluent builder interface that can build graphs of words
            string synsetForHello = dummyNamespace + "hello";
            store.Store(Lexeme.New.Noun(synsetForHello, "hello", typeof(Noun.hello)));

            string synsetForWorld = dummyNamespace + "world";
            store.Store(Lexeme.New.Noun(synsetForWorld, "world", typeof(Noun.world)));

            // Alternatives to building dictionaries this way include:
            //  1. Defining new Tokens with a parser method that can parse strings or perform lookups against a database or web service
            //  2. Reading text files or XML files to create a dictionary
            //  3. Using the provided Wordnet reader project which can load the whole of Wordnet into memory and can apply interfaces to Synsets

            string synsetForGoodbye = dummyNamespace + "goodbye";
            store.Store(Lexeme.New.Noun(synsetForGoodbye, "goodbye", typeof(Noun.goodbye)));
            store.Store(Lexeme.New.Noun(synsetForGoodbye, "bye", typeof(Noun.goodbye)));

            string synsetForMicrosoft = dummyNamespace + "Microsoft";
            store.Store(Lexeme.New.Noun(synsetForMicrosoft, "Microsoft", typeof(Microsoft)));
            store.Store(Lexeme.New.Noun(synsetForMicrosoft, "MSFT", typeof(Microsoft)));

            string synsetForGoogle = dummyNamespace + "Google";
            store.Store(Lexeme.New.Noun(synsetForGoogle, "Google", typeof(Google)));

            string synsetForFacebook = dummyNamespace + "Facebook";
            store.Store(Lexeme.New.Noun(synsetForFacebook, "Facebook", typeof(Facebook)));

            // If you never need to refer to a Lexeme specifically there's no need to define an interface for it
            // or to have a specific Synset name, one will be created for you

            store.Store(Lexeme.New.Noun("Nintendo", typeof(CompanyName)));

            // Included rules can do normal verb conjugations (single, plural, past, participle, ...) so often you only need to define one word
            // For each token you can attach any number of interfaces (though normally you only need one since it inherits all the others)
            // Verb.Type.Present in this case is not needed and Verb.Type.Transitive could have been specified in the interface definition instead

            store.Store(Lexeme.New.Verb("report", typeof(Verb.report), typeof(AboditNLP.Verb.Tense.Present), typeof(AboditNLP.Verb.Type.Transitive))
                // If you need to override an irregular verb, or add an interface to a specific tense you can do that
                .Past("reported", typeof(Verb.reported))
                // Verbs can have associated Noun forms, Adverb forms and Adjective forms, e.g. when you report something the thing is a 'report'
                .Noun("report", typeof(Noun.report)));

            store.Store(Lexeme.New.Verb("debug", typeof(Verb.debug), typeof(AboditNLP.Verb.Tense.Present)));
        }
    }
}