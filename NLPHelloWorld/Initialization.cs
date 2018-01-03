using AboditNLP;
using Autofac;
using NLPHelloWorldRules;
using System;
using AboditNLP.Noun;

namespace NLPHelloWorld
{
    class Initialization
    {
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
            nlp.InitializeAsync();
            return nlp;
        });

        public static Lazy<IContainer> AutofacContainer = new Lazy<IContainer>(() =>
        {
            var builder = new ContainerBuilder();

            // A LexemeStore is used to store all in-memory words in an efficient TernaryTree structure
            builder.RegisterType<LexemeStore>().SingleInstance().AsImplementedInterfaces();

            // NLP should be registered as SingleInstance as it requires a lot of memory and many structures in it are static
            builder.RegisterType<NLP>().SingleInstance().AsImplementedInterfaces();

            // Register all of the ITokenFactory, ILexemeStore and INLPRule classes in all relevant assemblies
            // the interface INLPPart is provided to make this convenient (and future proof)

            // The NLP assembly contains many Production Rules and some TokenFactories that need to be registered
            builder.RegisterAssemblyTypes(typeof(NLP).Assembly)
                .Where(t => t.IsAssignableTo<INLPPart>())
                .AsImplementedInterfaces();

            // Assembly containing your rules
            builder.RegisterAssemblyTypes(typeof (SampleRules).Assembly)
                .Where(t => t.IsAssignableTo<INLPPart>()).AsImplementedInterfaces();

            // Assemblies containing Wordnet
            builder.RegisterAssemblyTypes(typeof(WordnetCore).Assembly)
                .Where(t => t.IsAssignableTo<INLPPart>()).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(WordnetExtended).Assembly)
                .Where(t => t.IsAssignableTo<INLPPart>()).AsImplementedInterfaces();

            // Indicate all the Assemblies that can be used by NLP for resolution of 
            // types produced by rules or in token factories.
            builder.RegisterInstance(typeof(NLP).Assembly);                            // NLP
            builder.RegisterInstance(typeof(SampleRules).Assembly);                    // NLP Dictionary
            // Always include Abodit Units as it contains all of the Temporal classes
            builder.RegisterInstance(typeof(Abodit.Units.Distance).Assembly);          // AboditUnits
            // Include Wordnet it you are using it
            builder.RegisterInstance(typeof(WordnetCore).Assembly);                  // NLP Wordnet
            builder.RegisterInstance(typeof(WordnetExtended).Assembly);                 // NLP Wordnet-Extended

            // Register any classes that contain the actual code to run when a sentence
            // is recognized. So called "Intent" classes.
            builder.RegisterAssemblyTypes(typeof(SampleRules).Assembly)
                .Where(t => t.IsAssignableTo<IIntent>())
                .AsImplementedInterfaces();

            // ----------- BUILD ---------------
            return builder.Build();
        });
    }
}