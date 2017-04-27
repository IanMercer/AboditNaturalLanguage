using Abodit.Meta;
using AboditNLP;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// Define a metamodel that can be used to create complex queries against data
    /// </summary>
    /// <remarks>
    /// On startup this metamodel is added to the NLP Lexemestore to enable
    /// all of the entities and fields in it to be recognized
    /// </remarks>
    public class HowToDefineAMetaModel : ILexemeGenerator
    { 
        public void CreateWords(ILexemeStore store)
        {
            store.AddMetaModel(new MockMetaModel());
        }
    }

    /// <summary>
    /// Here is a simple Metamodel with a single entity (table)
    /// </summary>
    public class MockMetaModel : Metamodel
    {
        public readonly IMetaEntity ProductSaleEntity;

        public MockMetaModel()
        {
            this.ProductSaleEntity = new ProductSaleMetaEntity();

            // You could have many different Entities (tables), here we have just one
            var allEntities = new [] { this.ProductSaleEntity };

            base.AddAllEntities(allEntities);
        }
    }
}
