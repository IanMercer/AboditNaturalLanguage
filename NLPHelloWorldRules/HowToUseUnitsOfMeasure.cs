using AboditNLP;
using Abodit.Units;

namespace NLPHelloWorldRules
{
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// This sample rule recognizes any Distance
        /// </summary>
        /// <remarks>
        /// Many other units of measure are provided
        /// </remarks>
        public NLPActionResult Distance(Distance distance)
        {
            st.Say("You entered a distance " + distance.Describe(true) + " or " + distance.Describe(false));
            return NLPActionResult.None;
        }

        /// <summary>
        /// This sample rule recognizes any Weight
        /// </summary>
        /// <remarks>
        /// Many other units of measure are provided
        /// </remarks>
        public NLPActionResult Weight(Weight weight)
        {
            st.Say("You entered a weight " + weight.Describe(true) + " or " + weight.Describe(false));
            return NLPActionResult.None;
        }
    }
}