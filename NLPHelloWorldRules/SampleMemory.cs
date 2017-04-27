namespace NLPHelloWorldRules
{
    /// <summary>
    /// One way to create a conversation flow is to put objects into a 'memory' for the conversation
    /// Here's a very simple memory object
    /// </summary>
    public class SampleMemory
    {
        public string Message { get; private set; }

        public SampleMemory(string message)
        {
            this.Message = message;
        }
    }
}
