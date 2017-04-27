using System.Collections.Generic;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// You can pass additional state information to your controllers in the Execute method
    /// It will be injected into the constructor of the controller
    /// </summary>
    public class ExtraStateInformation
    {
        public bool Running = true;

        /// <summary>
        /// Clearly you would want to implement a better state machine than this to handle conversations of a pre-programmed nature
        /// </summary>
        public IEnumerable<string> ConversationSequence()
        {
            yield return "Try typing 'hello', this will be recognized by a simple rule 'public NLPActionResult Hello(Noun.hello hello)...'";
            yield return "Try typing 'last friday after 5pm', this will be recognized by 'public NLPActionResult UserEnteredATemporalExpression (TemporalSet ts)'";
            yield return "Try typing 'every wednesday in May', this is an infinite temporal expression";
            yield return "Try typing '#12345' to see how a custom Token can be parsed";

            yield return "Try typing 'define sugar' to see how Wordnet can be used";
            yield return "Try typing 'define' followed by any other common word from Wordnet";
            while (true)
            {
                yield return "Try typing 'goodbye'";
            }
        }
    }
}
