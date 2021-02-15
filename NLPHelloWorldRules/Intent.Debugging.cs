using AboditNLP;

namespace NLPHelloWorldRules
{
    public interface IIntentDebugging
    {
        void DebugOn(IListener st);

        void DebugOff(IListener st);
    }

    // Typically you would have more than one intent, but for this demo we use a partial class instead
    // to break out the different classes of intention

    public partial class Intent : IIntentDebugging
    {
        public void DebugOn(IListener st)
        {
            st.Say("Debugging now on");
        }

        public void DebugOff(IListener st)
        {
            st.Say("Debugging now off");
        }
    }
}