using System.Linq;
using AboditNLP;
using Abodit.Temporal;
using Abodit.Units;

namespace NLPHelloWorldRules
{
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// This sample rule recognizes any temporal expressions
        /// </summary>
        public NLPActionResult UserEnteredATemporalExpression(TemporalSet ts)
        {
            if (ts is TemporalSetNever) { st.Say("No time in range"); return NLPActionResult.None; }
            if (ts is TemporalSetUnknown) { st.Say("Please be specific about the time range you want to use."); return NLPActionResult.None; }

            IInfinite tsinfinite = ts as IInfinite;
            if (tsinfinite != null)
            {
                st.Say("Infinite time " + tsinfinite.Describe(true));
                var backward = ts.EnumerateBackward(TimeProvider.Current.CalendarNow);
                var firstsix = backward.Take(6).ToList();
                foreach (var range in firstsix.Take(5))
                {
                    st.Say(range.ToString());
                }
                if (firstsix.Count == 6) st.Say("...");
                return NLPActionResult.None;
            }

            IFinite tsfinite = ts as IFinite;

            if (tsfinite != null)
            {
                st.Say("You entered " + tsfinite);
                st.Say("That's a finite set so you can enumerate all of the time periods in it");
            }
            return NLPActionResult.None;
        }
    }
}
