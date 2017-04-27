using AboditNLP;
using Autofac;
using NLPHelloWorldRules;
using System;
using AboditNLP.IoC.Autofac;

namespace NLPHelloWorld
{
    class Initialization
    {
        /// <summary>
        /// Get a static singleton instance of the NLP engine
        /// </summary>
        public static INLP NLPInstance => nlpInstance.Value;

        private static readonly Lazy<INLP> nlpInstance = new Lazy<INLP>(() =>
        {
            var container = AutofacContainer.Value;

            // IMPORTANT: Must set the resolver for NLP
            NLP.SetResolver(new AboditNLP.IoC.Autofac.AutofacResolver(container));

            // Add a logger if you want one
            NLP.SetLogger(new LogFacade());

            // Resolve the NLP engine and all of its dependencies (TokenFactories, ...)
            var nlp = container.Resolve<INLP>();
            
            // Async initialization - call this as early as possible so it can run in the background
            // while you start up the rest of your user interface
            nlp.InitializeAsync();
            return nlp;
        });

        /// <summary>
        /// AutoFac configuration would normally be called from your global.asax or Program.cs
        /// startup code and would be extended to configure all of the other dependencies you
        /// use in your own dependency-injected classes. If you move this configuration code out
        /// of this Lazy property you still need to pass the built-container to NLP using 
        /// SetResolver (see above).
        /// Other DI containers are possible but the Autofac extension is the only one on Nuget presently
        /// </summary>
        public static Lazy<IContainer> AutofacContainer = new Lazy<IContainer>(() =>
        {
            var builder = new ContainerBuilder();

            // Register any Autofac modules including the one defined below for NLP
            builder.RegisterAssemblyModules(typeof(Initialization).Assembly);

            // Register any classes that contain the actual code to run when a sentence is recognized.
            // So called "Intent" classes. (This isn't a required part of AboditNLP but it's a good
            // way to structure your code using dependency injection and separating the NLP rules
            // from the services that they call.)
            builder.RegisterAssemblyTypes(typeof(SampleRules).Assembly)
                .Where(t => t.IsAssignableTo<IIntent>())
                .AsImplementedInterfaces();

            // ----------- BUILD ---------------
            return builder.Build();
        });

    }

    /// <summary>
    /// Autofac encourages you to structure your configuration code in a <see cref="Module"/>
    /// </summary>
    internal class HelloWorldModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // A LexemeStore is used to store all in-memory words in an efficient TernaryTree structure
            builder.RegisterType<LexemeStore>().SingleInstance().AsImplementedInterfaces();

            // You may need access to the LexemeStore for things like adding metamodels or performing
            // spelling correction lookups

            // NLP should be registered as SingleInstance as it requires a lot of memory and many structures in it are static
            builder.RegisterType<NLP>().SingleInstance().AsImplementedInterfaces();

            // Register all of the ITokenFactory, ILexemeStore and INLPRule classes in all relevant assemblies
            // the interface INLPPart is provided to make this convenient (and future proof) and the RegisterAllNlpParts
            // extension method makes it even easier to register them all.

            // The NLP assembly contains many Production Rules and some TokenFactories that need to be registered
            builder.RegisterAllNlpParts(typeof(NLP).Assembly);

            // Register your own rules-containing Assembly
            builder.RegisterAllNlpParts(typeof(SampleRules).Assembly);

            // Register the Assemblies containing Wordnet words, definitions and synset relationships
            builder.RegisterAllNlpParts(typeof(WordnetCore).Assembly);
            builder.RegisterAllNlpParts(typeof(WordnetExtended).Assembly);
        }
    }
}