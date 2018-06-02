using AboditNLP;
using AboditNLP.Attributes;
using System.Threading;
using Abodit.Expressions;
using Abodit.Temporal;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// AboditNLP supports recognizing integer values (4, 5, six, twenty-one), long values,
    /// double values and rational values.
    /// 
    /// It also supports numeric expressions (4 + 5 * 2) which are a subclass for 
    /// the other numeric token types
    /// </summary>
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// This sample rule recognizes a numeric expression
        /// </summary>
        /// <remarks>
        /// See also <see cref="UserEnteredATemporalExpression"/> which has a higher priority and claims
        /// TemporalSets (which inherit from TokenExpression but for which we want a different rule)
        /// </remarks>
        [Priority(500)]
        public NLPActionResult UserEnteredANumericExpression(TokenExpression tne)
        {
            if (tne.Type == typeof(double))
            {
                var result = tne.ToDouble();
                if (result != null)
                    st.Say("Hmmm .. a puzzle, I love puzzles...");
                Thread.Sleep(250);
                st.Say("...");
                Thread.Sleep(250);
                st.Say("I think the answer is " + result);
            }
            else if (tne is TemporalSet)
            {
                throw new System.Exception("Check the relative priority of this rule and UserEnteredATemporalExpression");
            }
            else
            {
                st.Say($"You entered an expression {tne.Describe(true)}");
                st.Say($"You can serialize a token expression {tne.Serialize()}");
                st.Say($"You can get the unbound variables on it {tne.UnboundVariables}");
                st.Say($"To create variables, add a TokenExpressionVariableAcces to the Lexeme store using the words you want.");
                st.Say($"You can evaluate a TokenExpression in the context of an environment containing variable values.");
                st.Say($"You can also convert it to a LINQ Expression or to a SQL Query.");
            }
            return NLPActionResult.None;
        }

        /// <summary>
        /// Whereas this rule expects an actual value (not an expression)
        /// </summary>
        [Priority(1000)]
        public NLPActionResult UserEnteredANumericValue(TokenInt ti)
        {
            st.Say("Hmmm .. so you want to know about " + ti.Text);
            st.Say("Well, it's just a single value: " + ti.ValueInt);
            return NLPActionResult.None;
        }
    }
}
