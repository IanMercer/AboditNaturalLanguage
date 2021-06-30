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
        public NLPActionResult GrammarDumpWithPrefix(Prefix prefix, IAmbiguous<ITokenText> tokenAmbiguous)
        {
            st.Say(prefix.OptionalResponsePrefix);
            intent.GrammarDump(st, tokenAmbiguous);
            return NLPActionResult.None;
        }
    }
}
