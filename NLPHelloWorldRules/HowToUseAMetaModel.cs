using System.Data;
using System.Linq;
using Abodit.Expressions;
using Abodit.Meta;
using AboditNLP;
using AboditNLP.Attributes;

namespace NLPHelloWorldRules
{
    public partial class SampleRules : INLPRule
    {
        /// <summary>
        /// This rule matches a complete query against an entity and its fields (or calculated expressions)
        /// and an optional sort order. e.g. 'longest products under 3m wide'.
        /// </summary>
        /// <remarks>
        /// When you provide AboditNLP with a metamodel for your entities it can construct
        /// complex queries against that data from english language input including
        /// various ways to specify the sort order, to filter on various fields, to use
        /// SQL functions like UPPER, DATEPART, ...
        /// 
        /// For DateTime fields the capabilities are particularly advanced in that you can use
        /// complex datatime queries like 'last year on a friday in May after 5pm' and it can
        /// build the SQL query for that.
        /// </remarks>
        [Priority(1000)]
        public NLPActionResult CompleteQuery(CompleteQuery completeQuery)
        {
            st.Say("You entered a query against an entity");
            st.Say($" {completeQuery}");

            if (completeQuery.Filter == TokenExpression.True)
            {
                var entity = completeQuery.MetaEntity;

                st.Say("You entered a word or phrase that matches an entity (table) name " + entity.Name);
                st.Say("You can also enter phrases that match fields (or calculated values) for that entity");
                st.Say("And AboditNLP can understad those expressions and create SQL or other database queries from them");

                foreach (var field in entity.Fields)
                {
                    // .TokenExpression.Type.Name
                    st.Say($" {field.Unit?.Name ?? field.TokenExpression?.Type?.Name} : {field.TokenExpression.Serialize()}");
                    st.Say($"   Words: {string.Join(", ", field.MetaSynSets.SelectMany(m => m.Words).Distinct())}");
                }

                st.Say("Try asking for 'products wider than 3m'");
            }
            else
            {
                st.Say("You can convert this query to SQL or other database query languages");

                var visitor = new Abodit.Expressions.Visitor.Sql.ConvertToSqlStringVisitor<DataTable>();
                var filter = visitor.Visit(completeQuery.Filter);

                st.Say($"Filter for this would be {filter}");

                if (completeQuery.Sort != null)
                {
                    var sort = visitor.Visit(completeQuery.Sort.SortExpression);
                    st.Say($"Sort order for this would be {sort}");
                }

                st.Say("You can use these to create a parameterized SQL query");
            }

            return NLPActionResult.None;
        }
    }
}