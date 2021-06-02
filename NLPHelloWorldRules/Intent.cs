using AboditNLP;

namespace NLPHelloWorldRules
{
    public interface IIntentDemo : IIntent, IIntentDebugging, IIntentWordnet, IIntentSuggestions, IIntentTemporal
    {
    }

    /// <summary>
    /// Instead of mixing the code that runs when a rule is recognized with
    /// the code the recognizes sentences it's better to create a separate class for each.
    ///
    /// This is analgous to avoiding the 'fat controller' anti-pattern in MVC applications.
    ///
    /// </summary>
    public partial class Intent : IIntentDemo
    {
        private readonly INLP nlp;

        public Intent(INLP nlp)
        {
            this.nlp = nlp;
        }
    }
}