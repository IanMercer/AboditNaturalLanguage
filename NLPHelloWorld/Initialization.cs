using AboditNLP;
using AboditNLP.IoC.Autofac;
using Autofac;
using NLPHelloWorldRules;
using System;

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
            builder.RegisterAllNlpParts(typeof(NLP).Assembly);

            // Assembly containing your rules
            builder.RegisterAllNlpParts(typeof(SampleRules).Assembly);

            // Include Wordnet it you are using it
            builder.RegisterAllNlpParts(typeof(WordnetCore).Assembly);

            // And if you want less common words too ...
            builder.RegisterAllNlpParts(typeof(WordnetExtended).Assembly);

            // Register any classes that contain the actual code to run when a sentence
            // is recognized. So-called "Intent" classes.
            builder.RegisterAssemblyTypes(typeof(SampleRules).Assembly)
                .Where(t => t.IsAssignableTo<IIntent>())
                .AsImplementedInterfaces();

            // ----------- BUILD ---------------
            return builder.Build();
        });
    }
}