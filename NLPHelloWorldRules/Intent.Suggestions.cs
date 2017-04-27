using System;
using AboditNLP;
using System.Linq;
using Abodit;

namespace NLPHelloWorldRules
{
    public interface IIntentSuggestions
    {
        void DefineSuggestions(IListener st, string original);
    }

    public partial class Intent : IIntentSuggestions
    {
        public void DefineSuggestions(IListener st, string original)
        {
            var possibles = nlp.LexemeStore
                .FuzzyMatch(original, Sensitivity.InsensitiveWithTwoEdits)
                // Closest by length
                .OrderBy(x => Math.Abs(x.TreeText.Length - original.Length))
                // then by score
                .ThenBy(x => x.SensitivityUsed.Score)
                .Take(10).ToList();

            if (possibles.Any())
            {
                var possibleWords = possibles.Select(x => $"'{x.TreeText}' ({x.SensitivityUsed.Score})");
                st.Say($"Did you mean {string.Join(",", possibleWords)}");
            }
        }
    }
}
