using System;
using System.Data;
using Abodit.Expressions;
using Abodit.Meta;
using Abodit.Units;
using Abodit.Units.Dimensions;
using AboditNLP;
using AboditNLP.Adjective.Type;

namespace NLPHelloWorldRules
{
    /// <summary>
    /// This is an example of a MetaEntity defining an entity and the fields on it.
    /// When you define a metaentity and store it to the LexemeStore, all manner of
    /// query operations against it become possible.
    /// </summary>
    /// <remarks>
    /// A MetaEntity can be backed by a DataTable, or by a strongly typed object.
    /// If you aren't using the Get method on a metaentity it doesn't matter too much
    /// what the backing type is.
    /// </remarks>
    public class ProductSaleMetaEntity : MetaEntity<DataRow>, ILexemeGenerator, IMetaEntity
    {
        public void CreateWords(ILexemeStore store)
        {
            // Pull all the words from MetaSynsets and define them separately in a dictionary
            store.Store(Lexeme.New.Noun("small_area-noun-1", "small area"));
            store.Store(Lexeme.New.Noun("large_area-noun-1", "large area"));
            store.Store(Lexeme.New.Noun("width-noun-1", "width"));
            store.Store(Lexeme.New.Noun("width-noun-1", "breadth"));

            store.Store(Lexeme.New.Adjective("small_area-adjective-1", "small in area")); // ??
            store.Store(Lexeme.New.Adjective("large_area-adjective-1", "large in area")); // ??

            store.Store(Lexeme.New.Adjective("expensive-adjective-1", "expensive"));
            store.Store(Lexeme.New.Adjective("cheap-adjective-1", "cheap"));

            store.Store(Lexeme.New.Noun("price-noun-2", "price"));
            store.Store(Lexeme.New.Verb("price-verb-1", "price"));

            store.Store(Lexeme.New.Adjective("close-adjective-1", "close", typeof(SmallType)));
            store.Store(Lexeme.New.Adjective("near-adjective-1", "near", typeof(SmallType)));
        }

        public readonly IField DateSoldField;
        public readonly IField AgeOfDateSoldField;
        public readonly IField NameField;
        public readonly IField ColorField;
        public readonly IField UnitPriceField;
        public readonly IField LengthField;
        public readonly IField WidthField;
        public readonly IField HeightField;
        public readonly IField WeightField;
        public readonly IField PriceField;
        public readonly IField CostField;
        public readonly IField ProfitField;
        public readonly IField RadiusField;
        public readonly IField DiameterField;
        public readonly IField Monster;
        public readonly IField DepthField;
        public readonly IField DistanceField;
        public readonly IField OpenField;
        public readonly IField NewOrUsedField;
        public readonly IField AverageTemperatureField;
        public readonly IField MaximumTemperatureField;
        public readonly IField MinimumTemperatureField;
        public readonly IField DurationField;
        public readonly IField VolumeField;
        public readonly IField AreaField;

        public ProductSaleMetaEntity() : base(Guid.NewGuid(), "product", "products",
            new[]
            {
                new MetaSynset("merchandise-noun-1", "product"),
                new MetaSynset("SKU-noun-1", "SKU")
            })
        {
            this.DateSoldField = new FieldDate("DateSold", this, "DateSold",
                typeof(DateTime),
                new[] { new MetaSynset("sell-verb-1", "sold") },   // verb "sold"
                Qualifier.None, null, null);

            // Age is (NOW - date in database)
            this.AgeOfDateSoldField =
                 new FieldNumeric("Age",
                     this,
                     // Age is (NOW - date in database)
                     new TokenExpressionPropertyAccess(
                         new TokenExpressionSubtract(
                             // TBD, what about time zone conversion?
                             // TBD, what about selecting the Years component of this?
                             TokenExpressionConstant.Create(DateTime.UtcNow),
                             new TokenExpressionVariableAccess("DateSold")),
                         typeof(double),
                         "Years"),
                     TimePeriod.UnitOneYear, new[] { new MetaSynset("age-verb-1", "age"), new MetaSynset("age-noun-1", "age") },
                     Qualifier.None,
                     null,
                     null, // TODO: AGE FORMATTER??
                     FieldQualifier.Normal);     // TODO: Show readonly field?

            this.NameField = new FieldText("Name",
                this,
                "Name",
                new[]
                {
                new MetaSynset("name-noun-1", "name"),
                new MetaSynset("name-verb-1", "name")       // named
                }
            );

            this.ColorField = new FieldText("Color", this,
                "PColor",
                new[]
                {
                new MetaSynset("color-noun-1", "color"),
                new MetaSynset("color-verb-1", "color")     // colored
                });

            this.UnitPriceField = new FieldNumeric("Unit price", this,
                "PUnitPrice",
                typeof(decimal),
                CurrencySymbol.GBP, new[]
                {
                            new MetaSynset("unit_price-noun-1", "unit price")
                },
                Qualifier.None,
                null,
                null,
                FieldQualifier.Important);

            this.LengthField = new FieldNumeric("Length", this,
                "PLength",
                typeof(decimal),
                Distance.OneCentimeter, new[]
                {
                new MetaSynset("length-noun-1", "length")
                },
                new[] { Qualifier.Length },
                null,
                null,
                FieldQualifier.Important);

            this.WidthField = new FieldNumeric("Width", this,
                "PWidth",
                typeof(decimal),
                Distance.OneCentimeter, new[]
                {
                new MetaSynset("width-noun-1", "width")
                },
                new[] { Qualifier.Width },  // Width is in a specific direction
                null,
                null,
                FieldQualifier.Important);

            this.HeightField = new FieldNumeric("Height", this,
                "PHeight",
                typeof(decimal),
                // Height field is in millimeters to make comparisons more interesting
                Distance.OneMillimeter,
                new[]
                {
                new MetaSynset("height-noun-1", "height"),
                },
                new[] { Qualifier.Height },
                null,
                null,
                FieldQualifier.Important);

            this.WeightField = new FieldNumeric("Weight", this,
                "PWeight",
                typeof(decimal),
                Weight.OneGramme, new[]
                {
                new MetaSynset("weight-noun-1", "weight", "mass"),
                new MetaSynset("weigh-verb-1", "weigh"),
                new MetaSynset("weigh-verb-1", "weighing")
                },
                Qualifier.None,
                null,
                null,
                FieldQualifier.Important);

            this.PriceField = new FieldNumeric("Price", this,
                "PPrice",
                typeof(decimal),
                CurrencySymbol.USD,
                new[] { new MetaSynset("price-noun-2", "price"), new MetaSynset("price-verb-1", "price") },
                Qualifier.None, null, null, FieldQualifier.Important);

            this.CostField = new FieldNumeric("Cost", this,
                // You can pass a calculated Expression not just a field name
                new TokenExpressionVariableAccess("PCost", typeof(Currency)),
                CurrencySymbol.GBP, new[]
                {
                new MetaSynset("cost-noun-1", "cost")
                },
                Qualifier.None,
                null, null,
                FieldQualifier.Important);

            this.ProfitField = new FieldNumeric("Profit", this,
                // You can pass a calculated Expression not just a field name
                new TokenExpressionSubtract(new TokenExpressionVariableAccess("PPrice", typeof(Currency)),
                    new TokenExpressionVariableAccess("PCost", typeof(Currency))),
                CurrencySymbol.GBP, new[]
                {
                new MetaSynset("profit-noun-1", "profit")
                },
                Qualifier.None,
                null, null,
                FieldQualifier.Important);

            this.RadiusField = new FieldNumeric("Radius", this,
                "PRadius",
                typeof(decimal), Distance.OneMeter,
                 new[] { new MetaSynset("radius-noun-1", "radius") },
                 new[] { Qualifier.Length }, null, null, FieldQualifier.Important);

            this.DiameterField = new FieldNumeric(
                "Diameter",
                this,
                new TokenExpressionMultiply(
                        new TokenExpressionVariableAccess("PRadius"),
                        TokenExpressionConstant.Create(2)),
                Distance.OneMeter,
                 new[] { new MetaSynset("diameter-noun-1", "diameter") },
                 new[] { Qualifier.Length }, null, null, FieldQualifier.Important);

            // monster products have a radius over 27
            this.Monster = new FieldBooleanExpression("Monster",
                this,
                new TokenExpressionGreater(
                    new TokenExpressionVariableAccess("PRadius"),
                    TokenExpressionConstant.Create(27)),
                new[] { new MetaSynset("monster-adjective-1", "monster") },
                null, null, FieldQualifier.Important);

            // Synonyn for HeightField, TODO: Allow multiple descriptions of each field in the constructor
            this.DepthField = new FieldNumeric("Height", this,
                "PHeight",
                typeof(double),
                Distance.OneMeter,
                new[] { new MetaSynset("depth-noun-1", "depth") },
                new[] { Qualifier.Depth, }, null, null, FieldQualifier.Important);

            // Distance in km (another type of 'length' field)

            this.DistanceField = new FieldNumeric("Distance", this,
                new TokenExpressionVariableAccess("PDistance", typeof(double)),
                Distance.OneKiloMeter,
                new[] { new MetaSynset("distance-noun-1", "distance") },
                new[] { Qualifier.Driven, }, null, null, FieldQualifier.Important
            );

            this.OpenField = new FieldEnumBool("Open", this,
                "Open",
                new EnumValue<bool>("open", true, new MetaSynset("open-adjective-1", "open")),
                new EnumValue<bool>("closed", false, new MetaSynset("closed-adjective-1", "closed")),
                null,
                new BoolFormatter("open", "closed")
            );

            this.NewOrUsedField = new FieldEnum("New or used", this, "NewOrUsed",
                new[]
                {
                new EnumValue<string>("new value", "new", new MetaSynset("new-adjective-1", "new", "unused")),
                new EnumValue<string>("used value", "used",
                    new MetaSynset("secondhand-adjectivesatellite-2", "secondhand", "second hand", "used"))
                },
                typeof(string),
                null, null
            );

            this.AverageTemperatureField = new FieldNumeric("Average temperature", this,
                "AveTemp",
                typeof(double),
                Temperature.OneCelcius,
                new[] { new MetaSynset("average_temperature-noun-1", "average temperature") },
                Qualifier.None,
                null, null,
                FieldQualifier.Average);

            this.MaximumTemperatureField = new FieldNumeric("Maximum temperature", this,
                "MaxTemp",
                typeof(double),
                Temperature.OneCelcius,
                new[] { new MetaSynset("maximum_temperature-noun-1", "maximum temperature") },
                Qualifier.None,
                null, null,
                FieldQualifier.Maximum);

            this.MinimumTemperatureField = new FieldNumeric("Minimum temperature", this,
                "MinTemp",
                typeof(double),
                Temperature.OneCelcius,
                new[] { new MetaSynset("minimum_temperature-noun-1", "minimum temperature") },
                Qualifier.None,
                null, null,
                FieldQualifier.Minimum);

            this.DurationField = new FieldNumeric("Duration", this,
                "PDuration",
                typeof(double),
                TimePeriod.UnitOneSecond,
                new[] { new MetaSynset("duration-noun-1", "length", "duration", "elapsed time", "time taken") },
                Qualifier.None,
                null, null, FieldQualifier.Important);

            // Computed field, not in database

            this.VolumeField = new FieldNumeric("Volume", this,
                new TokenExpressionMultiply(
                    new TokenExpressionVariableAccess("PWidth"),
                    new TokenExpressionVariableAccess("PLength"),
                    new TokenExpressionVariableAccess("PHeight")
                ),
                Volume.OneMeterCubed,
                new[] { new MetaSynset("volume-noun-1", "volume", "size", "capacity") },
                Qualifier.None,
                null, null);

            this.AreaField = new FieldNumeric("Area", this,
                new TokenExpressionMultiply(
                    new TokenExpressionVariableAccess("PWidth"),
                    new TokenExpressionVariableAccess("PLength")
                ),
                Area.OneMeterSquared,
                new[] { new MetaSynset("area-noun-1", "area") },
                Qualifier.None,
                null, null);
        }

        public override bool Equals(IMeta other)
        {
            return Object.ReferenceEquals(this, other);
        }
    }

    /// <summary>
    /// This object / class lets you track what the original data source was when
    /// you create a MetaEntity describing it. You could, for example just have a
    /// string field in here for the table name, or a Guid, ...
    /// </summary>
    public class MetaEntityId
    {
        public int Id { get; set; }
    }
}