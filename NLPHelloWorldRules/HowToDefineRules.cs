using System;
using AboditNLP;
using AboditNLP.Adjective;
using HelloWorld;
using AboditNLP.Attributes;
using AboditNLP.Verb;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// Sentence rules exist in classes that are much like the controller classes in ASP.NET MVC
    /// The NLP Engine uses dependency injection to inject dependencies into your controller classes
    /// A connection between AutoFac and AboditNLP is provided, others can easily be created.
    ///
    /// You can also inject additional objects into the Execute method which are then passed to
    /// controller classes like this. Do this to avoid having to create a new lifetime scope.
    ///
    /// You can also use AboditNLP without any additional dependency injection framework, just
    /// inject all the objects needed by your rule classes into the Execute method.
    /// </summary>
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// Typically you will take at least a dependency on an object implementing the IListener
        /// interface (or your own equivalent). This object represents a single conversation with
        /// an individual user. This object conveys messages back to the user over whatever channel
        /// the user is using (chat, email, SMS, Slack, ...) and it might also track a conversation
        /// history allowing responses that refer back to previous components of the conversation
        /// e.g. answers to questions asked of the user, resolution of 'it', 'them' and other words
        /// that refer back to objects discussed in a prior exchange of messages.
        /// </summary>
        /// <remarks>
        /// As with logging and dependency injection, AboditNLP does not force you into a particular
        /// technology or approach. Interfaces like `IListener` are provided but you don't have to use them.
        ///
        /// For example, for a web application you may well want to expand IListener to allow partial
        /// page updates to be communicated back to the browser like you saw in the demo on the website.
        /// </remarks>
        private readonly IListener st;

        /// <summary>
        /// The nlp class provides some methods that may be usedful inside rules, for example
        /// looking up the definition of words, or performing approximate matches against
        /// the dictionary to suggest possible words to the user.
        /// </summary>
        private readonly INLP nlp;

        /// <summary>
        /// You can of course extend this with any kind of state information you want to pass into the constructor using dependency injection
        /// </summary>
        private readonly ExtraStateInformation es;

        /// <summary>
        /// Instead of including the code that executes for each rule it's often better to separate
        /// that code into its own class. This is analagous to avoiding the 'fat controller'
        /// anti-pattern in MVC applications. This also makes testing easier - you can mock the
        /// intent class and test each sentence to make sure it calls the appropriate intent method.
        /// </summary>
        private readonly IIntentDemo intent;

        public SampleRules(IListener st, INLP nlp, IIntentDemo intent, ExtraStateInformation es)
        {
            // store the dependencies
            this.st = st;
            this.nlp = nlp;
            this.intent = intent;
            this.es = es;
        }

        public NLPActionResult Hello(Noun.hello hello)
        {
            string now = DateTime.Now.ToShortTimeString();
            st.Say("Hello to you too, it's now " + now);
            st.Remember(new SampleMemory(now));               // an example of how you can remember things that you spoke about
            return NLPActionResult.None;
        }

        public NLPActionResult Goodbye(Noun.goodbye goodbye)
        {
            Goodbye();
            return NLPActionResult.None;
        }

        // Using the synset 'leave1' will capture leave, quit, ...
        public NLPActionResult Goodbye(leave1 leave)
        {
            Goodbye();
            return NLPActionResult.None;
        }

        // Two rules use this method. Typically it would be in an intent,
        // but leaving it here for now as it's showing something else: use of GetLast.
        private void Goodbye()
        {
            st.Say("Goodbye");

            // This is how you retrieve items from a per-user memory.
            // You are free to implement your own scheme for this - the engine itself is agnostic.
            var lastHello = st.GetLast<SampleMemory>();

            if (lastHello != null)
            {
                st.Say("We were chatting for " + Math.Round((DateTime.UtcNow - lastHello.DateTimeWeDiscussedIt).TotalMinutes) + " minutes");
                st.Say("We started chatting at " + lastHello.Thing.Message + " which I got from the Memory object");
            }
            es.Running = false;
        }

        /// <summary>
        /// This sample rule recognizes just the special parsed Token we defined earlier
        /// </summary>
        public NLPActionResult ProductNumber(ParsedToken parsedToken)
        {
            st.Say("You entered a product number recognized by your custom token definition class");
            st.Say("It had a strongly typed value on it of " + parsedToken.Value);
            return NLPActionResult.None;
        }

        /// <summary>
        /// Optional parameters can be used and so can the Permute attribute, so this one rule will recognize
        ///   report Microsoft, Microsoft, Microsoft report
        ///
        /// Permute attributes can specify a group number allowing multiple independent permutation cycles within a single rule
        /// </summary>
        public NLPActionResult SentenceOne([Optional][Permute(1)] Verb.report report, [Permute(1)] CompanyName companyName)
        {
            // Note how we can conjugate the verb to create the appropriate response even if the interface we used
            // maps onto many different verbs we can still reply with the correct verb and the correct ending

            st.Say(report.PresentParticiple.Text + " on " + companyName.Text + " (using the gerund)");
            st.Say("After I have " + report.Past.Text + " on " + companyName.Text + " I will ... (using the past tense)");
            //st.Say(report.NounForm.Singular.Text + " completed (using the noun form, singular)");
            return NLPActionResult.None;
        }

        public NLPActionResult DebugOn(Verb.debug debug, on prepOn)
        {
            intent.DebugOn(st);
            return NLPActionResult.None;
        }

        public NLPActionResult DebugOff(Verb.debug debug, off prepOff)
        {
            intent.DebugOff(st);
            return NLPActionResult.None;
        }
    }
}