using AboditNLP;
using Abodit.Temporal;
using AboditNLP.Attributes;

namespace NLPHelloWorldRules
{
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// This sample rule recognizes any temporal expressions
        /// </summary>
        [Priority(1000)]
        public NLPActionResult UserEnteredATemporalExpression(TemporalSet ts)
        {
            intent.DescribeTemporalSet(st, ts);
            return NLPActionResult.None;
        }
    }
}
