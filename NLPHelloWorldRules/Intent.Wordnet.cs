using System;
using System.Collections.Generic;
using AboditNLP;
using System.Linq;
using AboditNLP.Noun;
using AboditNLP.Noun.Type;
using Abodit.Graph;

namespace NLPHelloWorldRules
{
    public interface IIntentWordnet
    {
        void GrammarDump(IListener st, ITokenText token);
        void GrammarDump(IListener st, IAmbiguous<ITokenText> tokenAmbiguous);
        void RecognizeMammals(IListener st, Mammal1 mammal);
        void AnswerIsXaY(IListener st, IAmbiguous<INoun> nounAmbiguous, IAmbiguous<INoun> classAmbiguous);
        void AnswerIsXaY(IListener st, INoun noun, INoun @class);
    }

    // Typically you would have more than one intent, but for this demo we use a partial class instead
    // to break out the different classes of intention

    public partial class Intent : IIntentWordnet
    {
        readonly Type[] wordFormTypes = new[] { typeof(IAdjective)
            ,typeof(AboditNLP.Adjective.Type.Normal)
            ,typeof(AboditNLP.Adjective.Type.Comparative)
            ,typeof(AboditNLP.Adjective.Type.Superlative)
            ,typeof(AboditNLP.Noun.Type.Singular)
            ,typeof(AboditNLP.Noun.Type.Plural)
            ,typeof(IAdverb)
            ,typeof(AboditNLP.Verb.Tense.Present)
            ,typeof(AboditNLP.Verb.Tense.Past)
            ,typeof(AboditNLP.Verb.Tense.PresentSelf)
            ,typeof(AboditNLP.Verb.Tense.PresentThirdPerson)
            ,typeof(AboditNLP.Verb.Tense.PresentPlural)
            ,typeof(AboditNLP.Verb.Tense.PresentParticiple)
            ,typeof(AboditNLP.Verb.Tense.PastSingular)
            ,typeof(AboditNLP.Verb.Tense.PastPlural)
            ,typeof(AboditNLP.Verb.Tense.PastParticiple)
            ,typeof(AboditNLP.Verb.Tense.Future)
            ,typeof(AboditNLP.Verb.Tense.Infinitive)
            ,
            };

        public void GrammarDump(IListener st, ITokenText token)
        {
            IVerb verb = token as IVerb;
            INoun noun = token as INoun;
            IAdjective adjective = token as IAdjective;

            if (token.Synset != null && token.Synset != SynSet.Empty)
            {
                string definition = nlp.GetDefinition(token);
                if (!string.IsNullOrEmpty(definition))
                {
                    st.Say(definition);
                }

                // Token is not a Lexeme, it's an Imporomptu proxy acting like all interfaces are present
                var ints = token.GetType().GetInterfaces();

                var edges = SynSet.Graph.Follow(token.Synset, null);

                var synonyms = edges.Where(e => e.Predicate == Relation.WordFor).Select(e => e.End)
                    .OfType<Lexeme>()
                    .Where(e => ints.Intersect(e.Interfaces).Intersect(wordFormTypes).Any())
                    .Select(e => e.Text)
                    .Where(e => e != token.Text)
                    .ToList();

                if (synonyms.Any())
                {
                    st.Say("Synonyms " + string.Join(", ", synonyms));
                }

                // Dump the relations of this Lexeme
                foreach (var outboundEdge in edges)
                {
                    if (outboundEdge.Predicate != Relation.WordFor)
                    {
                        st.Say(outboundEdge.Predicate.ToString() + " " + outboundEdge.End.ToString());
                    }
                }
            }

            if (verb != null)
            {
                st.Say("Present self : I " + verb.PresentSelf.Text +
                ", Present third person: He " + verb.PresentThirdPerson.Text +
                ", Present plural: We " + verb.PresentPlural.Text +
                ", Present participle: " + verb.PresentParticiple.Text +
                ", Past I/He:" + verb.Past.Text +
                ", Past participle: " + verb.PastParticiple.Text +
                ", Past plural: We " + verb.PastPlural.Text +
                ", Infinitive: To " + verb.Infinitive.Text);
            }
            else if (noun != null)
            {
                if (noun is Countable)
                {
                    st.Say("Noun singular " + ((Countable)noun).Singular.Text +
                    ", plural " + ((Countable)noun).Plural.Text);
                }
                else if (noun is Uncountable)
                {
                    st.Say("Uncountable noun " + ((Uncountable)noun).Singular.Text);
                }
            }
            else if (adjective != null)
            {
                st.Say("Adjective " + adjective.Normal.Text +
                    (adjective.Comparative == null ? "" : ",  Comparative " + adjective.Comparative.Text) +
                    (adjective.Comparative == null ? "" : ",  Superlative " + adjective.Superlative.Text));
            }
            else
            {
                st.Say("Some other form of word " + token.Text);
            }
            st.Say(string.Join(", ", token.GetType().GetInterfaces().Select(x => x.Name).Except(new[] { "IActLikeProxyInitialize", "IActLikeProxy", "ISerializable" })));
        }

        public void GrammarDump(IListener st, IAmbiguous<ITokenText> tokenAmbiguous)
        {
            if (tokenAmbiguous.Count() > 1)
                st.Say(tokenAmbiguous.First().Text + " could mean one of several things:");
            int i = 0;
            foreach (var tokenGroup in tokenAmbiguous.GroupBy(t => t.Synset ?? SynSet.Empty))
            {
                st.Say(++i + ". " + tokenGroup.Key.Name);
                if (tokenGroup.Key == SynSet.Empty)
                {
                    foreach (var t in tokenGroup)
                    {
                        this.GrammarDump(st, t);
                    }
                }
                else
                {
                    this.GrammarDump(st, tokenGroup.First());
                }
            }
        }

        public void RecognizeMammals(IListener st, Mammal1 mammal)
        {
            st.Say("Yes, all " + mammal.Plural.Text + " are mammals");
            string description = nlp.GetDefinition(mammal);
            if (!string.IsNullOrEmpty(description))
                st.Say("Where " + mammal.Text + ": " + description);
            else
            {
                st.Say("Where " + mammal.Text + " is " + mammal.Synset.Name);
            }
        }

        public void AnswerIsXaY(IListener st, IAmbiguous<INoun> nounAmbiguous, IAmbiguous<INoun> classAmbiguous)
        {
            var found = FindConnections(nounAmbiguous, classAmbiguous);

            if (found != null)
            {
                st.Say(found.Text);
            }
            else
            {
                var reverse = FindConnections(classAmbiguous, nounAmbiguous);

                var possibleNouns = nounAmbiguous.Select(n => n.Synset);
                var possibleClasses = classAmbiguous.Select(n => n.Synset);

                var nounNames = possibleNouns.Select(possible =>
                        possible.Lexemes
                        .OfType<LexNoun>()
                        .Select(x => x.Plural)
                        .Select(x => x.Text)
                        .First())
                        .Distinct();

                var classNames = possibleClasses.Select(possible =>
                        possible.Lexemes
                        .OfType<LexNoun>()
                        .Select(x => x.Singular)
                        .Select(x => x.Text)
                        .First())
                        .Distinct();

                string pluralAsEntered = nounAmbiguous.Select(n => n.Plural.Text).First();
                string classPluralAsEntered = classAmbiguous.Select(n => n.Plural.Text).First();

                if (reverse != null)
                    st.Say("No, but some " + classPluralAsEntered + " are " + pluralAsEntered);
                else
                    st.Say("No, no " + pluralAsEntered + " are " + classPluralAsEntered + " and no " + classPluralAsEntered + " are " + pluralAsEntered + ".");

                var commonAncestors = FindCommonAncestors(classAmbiguous, nounAmbiguous).ToList();

                var commonNames = commonAncestors.Select(ca => ca.Lexemes.OfType<LexNoun>()
                    .Select(x => x.Plural)
                    .Select(x => x.Text)
                    .DefaultIfEmpty("-").First())
                    .Distinct()
                    .ToList();

                if (commonNames.Any())
                {
                    st.Say("They are both " + JoinOr(commonNames) + ".");
                    st.Say($"The  {commonAncestors.Count} SynSets for these are: {JoinOr(commonAncestors.Select(c => $"`{c.Name}`"))}");
                }
                else
                {
                    st.Say("No common ancestry!?!?");
                }
            }
        }

        public void AnswerIsXaY(IListener st, INoun noun, INoun @class)
        {
            var edgeSuccessors = SynSet.Graph.Successors<SynSet>(noun.Synset, Relation.RDFSType).ToList();
            var classSuccessors = SynSet.Graph.Successors<SynSet>(@class.Synset, Relation.RDFSType).ToList();

            if (edgeSuccessors.Any(e => e == @class.Synset))
            {
                st.Say("Yes, all " + noun.Plural.Text + " are " + @class.Plural.Text);
            }
            else
            {
                var intersects = edgeSuccessors.Intersect(classSuccessors).ToList();
                if (intersects.Any())
                {
                    var plurals = intersects.SelectMany(x => x.Lexemes)
                        .Where(y => y.Interfaces.Contains(typeof(Plural)))
                        .Select(y => y.Text)
                        .Distinct();

                    st.Say("No, a " + noun.Singular.Text + " is not a " + @class.Singular.Text + " but they are both " + string.Join(",", plurals));
                }
                else
                {
                    st.Say("No, no " + noun.Singular.Text + " is a " + @class.Singular.Text);
                }
            }
        }


        private class FoundConnection
        {
            public string Text { get; set; }
        }

        private FoundConnection FindConnections(IAmbiguous<INoun> nounAmbiguous, IAmbiguous<INoun> classAmbiguous)
        {
            bool onlyOne = nounAmbiguous.Count() == 1 && classAmbiguous.Count() == 1;

            foreach (var @class in classAmbiguous)
            {
                foreach (var noun in nounAmbiguous)
                {
                    if (@class.Synset == noun.Synset)
                    {
                        return new FoundConnection { Text = "Yes, those words can be synonyms." };
                    }

                    var edgeSuccessors = SynSet.Graph.Successors<SynSet>(noun.Synset, Relation.RDFSType).ToList();

                    if (edgeSuccessors.Any(e => e == @class.Synset))
                    {
                        if (onlyOne)
                            return new FoundConnection { Text = "Yes, a " + noun.Singular.Text + " is a " + @class.Singular.Text };
                        else
                            return new FoundConnection { Text = "Yes, a " + noun.Singular.Text + " can be a " + @class.Singular.Text };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the common ancestors for a pair of ambiguous INouns
        /// </summary>
        private IEnumerable<SynSet> FindCommonAncestors(IAmbiguous<INoun> nounAmbiguous, IAmbiguous<INoun> classAmbiguous)
        {
            Graph<SynSet,Relation> classSuccessors = null;
            foreach (var @class in classAmbiguous)
            {
                var successorsForOne = SynSet.Graph.Successors<SynSet>(@class.Synset, Relation.RDFSType);
                classSuccessors = classSuccessors?.Union(successorsForOne) ?? successorsForOne;
            }

            Graph<SynSet,Relation> nounSuccessors = null;
            foreach (var noun in nounAmbiguous)
            {
                var edgeSuccessors = SynSet.Graph.Successors<SynSet>(noun.Synset, Relation.RDFSType);
                nounSuccessors = nounSuccessors?.Union(edgeSuccessors) ?? edgeSuccessors;
            }

            var commonAncestors = classSuccessors.Intersect(nounSuccessors);

            var topSort = commonAncestors.TopologicalSortApprox();

            return topSort;
        }

        /// <summary>
        /// A helper method to format an enumerable as a comma separated list with an 'and' at the end
        /// </summary>
        private static string JoinAnd(IEnumerable<object> values)
        {
            if (values.Count() == 1) return values.First().ToString();
            return string.Join(", ", values.Take(values.Count() - 1))
                + " and " + values.Last().ToString();
        }

        /// <summary>
        /// A helper method to format an enumerable as a comma separated list with an 'or' at the end
        /// </summary>
        private static string JoinOr(IEnumerable<object> values)
        {
            if (values.Count() == 1) return values.First().ToString();
            return string.Join(", ", values.Take(values.Count() - 1))
                + " or " + values.Last().ToString();
        }
    }
}
