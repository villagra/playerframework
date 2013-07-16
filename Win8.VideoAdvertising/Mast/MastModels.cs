namespace Microsoft.VideoAdvertising
{
    using System.Collections.Generic;

    public sealed class MAST
    {
        public MAST()
        {
            Triggers = new List<Trigger>();
        }

        public IList<Trigger> Triggers { get; private set; }
    }

    public sealed class Trigger
    {
        public Trigger()
        {
            Sources = new List<Source>();
            EndConditions = new List<Condition>();
            StartConditions = new List<Condition>();
        }

        public IList<Condition> StartConditions { get; private set; }

        public IList<Condition> EndConditions { get; private set; }

        public IList<Source> Sources { get; private set; }

        public string Id { get; set; }

        public string Description { get; set; }
    }

    public sealed class Condition
    {
        public Condition()
        {
            Conditions = new List<Condition>();
        }

        public IList<Condition> Conditions { get; private set; }

        public ConditionType? Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Operator? Operator { get; set; }
    }

    public enum ConditionType
    {
        Property,
        Event,
    }

    public enum Operator
    {
        EQ,
        NEQ,
        GTR,
        GEQ,
        LT,
        LEQ,
        MOD,
    }

    public sealed class Target
    {
        public Target()
        {
            Targets = new List<Target>();
        }

        public IList<Target> Targets { get; private set; }

        public string Region { get; set; }

        public string Type { get; set; }
    }

    public sealed class Source
    {
        public Source()
        {
            Targets = new List<Target>();
            Sources = new List<Source>();
        }

        public IList<Source> Sources { get; private set; }

        public IList<Target> Targets { get; private set; }

        public string Uri { get; set; }

        public string AltReference { get; set; }

        public string Format { get; set; }
    }
}
