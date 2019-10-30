using System;
using System.IO;
using log4net;
using AboditNLP;
using System.Threading;
using NLPHelloWorldRules;

namespace NLPHelloWorld
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string location = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("Starting in " + location);

            //// Set up the static Logger from XML ...
            string loggingFileLocation = Path.Combine(location, "log4net.config");
            FileInfo loggingFileInfo = new FileInfo(loggingFileLocation);

            if (!loggingFileInfo.Exists) throw new ApplicationException("Cannot start without our log4net file");

            log4net.Config.XmlConfigurator.ConfigureAndWatch(loggingFileInfo);
            var log = LogManager.GetLogger("Global");

            log.Info("                                                 *** STARTING ***\r\n\r\n");
            log.Info(" Please wait while the dictionaries and rules are loaded.");
            log.Info(" Since WordNet is included in this demo that may take up to a minute.");
            log.Info("");

            // ----------------------

            // The Wordnet add-on now builds its dictionary in RAM on load and contains interface definitions for
            // all possible Synsets in Wordnet.

            // The Abodit NLP Engine can use any dependency injection container but for the moment
            // the only adapter implemented is for Autofac. I can provide details on how to implement
            // IDependencyScope and IDependencyResolver if you need to use a different container.
            //
            // You should create your container with the registrations needed by your rule classes,
            // token factories and lexeme generators.
            //

            // Although not required (you can inject any classes you like) it's typical to inject
            // two objects into your rules. Here we inject just one (a collector) that contains
            // the other one (a remembered history). Interfaces are provided for these two common
            // objects.

            // A history object allowed rules to look back on a conversation to find things of a given
            // type (interface) in order to respond to apply responses to questions, clatifications, ...

            var rh = new RememberedHistory();

            // A collector object can be either synchronous or asynchronous. A synchronous collector
            // collects all the responses from the call to nlp.Execute and then returns them, you might
            // use this approach for an email-based collector. An asynchronous collector provides a way
            // for rules to continue executing even after the call to nlp.Execute() has completed, you
            // should use this approach for any chat-like connections. Rules can thus fire off tasks that
            // keep running and provide messages back to the user about progress on a long task, or updates
            // when something changes.

            var collector = new ConsoleCollector(rh);

            // Create the Dependency Injection container and get an instance of the NLP engine
            var nlp = Initialization.NLPInstance;

            collector.Say("Starting... please wait...");

            // Use the async call so loading tokens and rules happens in the background
            // and the UI isn't blocked.

            // Create a logger for NLP (calls through to log4net in this case)
            var logger = new LogFacade();

            nlp.InitializeAsync(logger).ContinueWith((t) =>
            {
                collector.Say("OK, I'm ready now");
                collector.Say("Hello");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("> ");
                Console.ResetColor();
            });

            // You can pass additional state objects into the Execute method and they will find
            // their way into your rule classes by way of constructor injection or you can
            // just rely on your dependency injection framework to supply them.
            //
            // Using an object like this passed into the Execute method is useful when you have
            // dependencies that vary on a per-user or a per-request basis, e.g. an IUser object
            //
            var extraState = new ExtraStateInformation();

            // Turn off all debugging
            NLP.Debugging = false;

            var conversationSequence = extraState.ConversationSequence().GetEnumerator();

            while (extraState.Running)
            {
                // Rather simplistic approach to telling you stuff one line at a time
                string line = Console.ReadLine();
                bool success = nlp.Execute(line, collector, extraState);

                if (!success)
                {
                    collector.Say("Sorry, didn't understand that");

                    // Implement a very simple state machine to push the conversation along
                    // each time the user doesn't enter a valid sentence
                    if (conversationSequence.MoveNext())
                    {
                        Console.WriteLine();
                        Console.WriteLine(conversationSequence.Current);
                    }
                }
                else
                {
                    Console.WriteLine();
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("> ");
                Console.ResetColor();
            }

            Thread.Sleep(4000);
        }
    }
}