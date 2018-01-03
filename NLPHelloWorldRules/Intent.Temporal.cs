using Abodit.Expressions;
using Abodit.Expressions.Visitor;
using Abodit.Expressions.Visitor.Sql;
using Abodit.Temporal;
using Abodit.Units;
using AboditNLP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NLPHelloWorldRules
{
    public interface IIntentTemporal
    {
        void DescribeTemporalSet(IListener st, TemporalSet ts);
    }

    // Typically you would have more than one intent, but for this demo we use a partial class instead
    // to break out the different classes of intention

    public partial class Intent : IIntentTemporal
    {
        public void DescribeTemporalSet(IListener st, TemporalSet ts)
        {
            if (ts is TemporalSetNever) { st.Say("No time in range"); return; }
            if (ts is TemporalSetUnknown) { st.Say("Please be specific about the time range you want to use."); return; }

            st.Say("You entered: " + ts.Describe(true));

            if (ts is IInfinite tsinfinite)
            {
                st.Say("That's an infinite set but you can enumerate it backward");
                st.Say("(e.g. user specified an event in the past and you want the date time range for it)");
                var backward = ts.EnumerateBackward(TimeProvider.Current.CalendarNow);
                DumpEnumerationLimited(st, backward, 6);

                st.Say("or forward (e.g. user wants to set a reminder in the future)");
                var forward = ts.EnumerateForward(TimeProvider.Current.CalendarNow);
                DumpEnumerationLimited(st, forward, 6);
            }

            if (ts is IFinite tsfinite)
            {
                st.Say("You entered " + tsfinite.Describe(true));
                st.Say("That's a finite set so you can enumerate all of the time periods in it.");
            }

            // And now show how to build SQL from this:

            // Normally the expression would be returned as part of a CompleteQuery which can then be visited
            // to generate SQL but in this case let's make that expression and convert it to SQL:

            var queryExpression = new TokenExpressionMatches(new TokenExpressionVariableAccess("DateSold", typeof(DateTime)), ts);

            var visitor = new ConvertToSqlStringVisitor<ProductClass>();

            // But in this case we maybe have just an input field for a temporal expression and we want to convert that to a SQL query
            var sqlQuery = visitor.Visit(queryExpression);

            st.Say("As a SQL query this would be: " + sqlQuery.QueryString);

            st.Say("");

#if COMING_SOON
            st.Say("You can also convert it to a LINQ to object query expression");

            // The conversion visitor in this case needs a Getter which could be one of the predefined ones (using Reflection or using a DataRow)
            // or some other getter that you define. In this case we want to query against an in-memory object using reflection to get to the
            // required field "DateSold".
            var expressionVisitor = new ConvertToExpressionVisitor<ProductClass>(ConvertToExpressionVisitor<ProductClass>.GetterForObjectUsingReflection);

            // But in this case we maybe have just an input field for a temporal expression and we want to convert that to a SQL query
            var expressionQuery = expressionVisitor.Visit(queryExpression);

            st.Say("As a LINQ to objects query this would be: " + expressionQuery.ToString());
#endif
        }

        private static void DumpEnumerationLimited(IListener st, IEnumerable<CalendarDateTimeRange> sequence, int limit)
        {
            var firstsix = sequence.Take(limit).ToList();
            foreach (var range in firstsix.Take(limit-1))
            {
                st.Say("   " + range.ToString());
            }
            if (firstsix.Count == limit) st.Say("...");
        }

        /// <summary>
        /// Sample class for an object stored in a SQL table
        /// </summary>
        private class ProductClass
        {
            public DateTime DateSold { get; set; }
        }

    }
}
