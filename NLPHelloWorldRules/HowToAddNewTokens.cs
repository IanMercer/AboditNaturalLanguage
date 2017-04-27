using System.Collections.Generic;
using System.Linq;
using Abodit;
using AboditNLP;
using AboditNLP.Tokens;

namespace HelloWorld
{
    /// <summary>
    /// This sample TokenFactory parses strings of the form '#' followed by a sequence of digits.
    /// You might have a similar Token for recognizing ISBN numbers, UPC codes, ...
    /// 
    /// A token class can do database look ups too, but if you do, mark it as 'slow'.
    /// 
    /// TokenFactories are useful for words that change over time such as names in a database, or for text that is
    /// best parsed using a RegEx or other method.
    /// 
    /// Static text like words in the dictionary is best stored using Lexemes (see How to Define Words).
    /// 
    /// You can split the Token class from the TokenFactory class or you can combine them as in this example.
    /// 
    /// TokenFactories can take part in DependencyInjection (e.g. you could inject a Database service into a
    /// TokenFactory that needs a database lookup.
    /// 
    /// TokenFactories can also be added at 'Execute' time by passing them in as additional objects to the Execute
    /// method. This enables scenarios like a per-user dictionary of 'friends'.
    /// 
    /// </summary>

    // If you add a Produces attribute to say what types this parser can produce, you can instruct the engine to skip
    // trying to use this parser when there is no possible match in the input string.
    [Produces(typeof(ParsedToken))]
    public class ParsedToken : Token, ITokenFactory
    {
        // A Parsed Token can have strongly typed values on it, here was have a simple int
        public int Value { get; set; }

        // A Token class must implement the Parse method which returns all the tokens it can recognize
        // at the given start position in the input string.

        // It is also passed the types that are allowed at this point in the input so if your returned types
        // aren't one of those types you can simply yield break.

        // The provided IDependencyScope also allows you to resolve additional services using your preferred dependency injection
        // technology

        public IEnumerable<TokenResult> Parse(NLPOptions options, int start, string input, params AllowedToken[] types)
        {
            int i = start;

            // The included Claim method makes it easy to claim a single word from the input BUT it requires a space after it ...
            // if (!Claim("#", input, ref i)) yield break;

            // You can use RegExes or any other parsing techniques to look at the rest of the string
            // or you can call out to other Tokens to see if on of them can be parsed from the string
            // You can even call out to a web service, database or whatever else

            // Here's a VERY simple parser ...
            bool found = false;

            if (input.Length < start + 3) yield break;
            if (input[0] != '#') yield break;
            i++;

            while (i < input.Length && "0123456789".Contains(input[i]))
            {
                i++;
                found = true;
            }

            if (!found) yield break;

            string extract = input.Substring(1, i - 1);

            TokenHelper.SkipWhiteSpace(input, ref i);       // clean up at end

            int value = 0;
            int.TryParse(extract, out value);
            var token = new ParsedToken() { Original = input.Substring(i), Value = value };

            // Finally yield return each match that you have - there may be more than one

            yield return new TokenResult(token, input, start, i, Sensitivity.Default);
        }
    }
}
