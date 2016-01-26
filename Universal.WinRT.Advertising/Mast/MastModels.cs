namespace Microsoft.Media.Advertising
{
    using System.Collections.Generic;

    public sealed class MAST
    {
        private IList<Trigger> _Triggers;
        public IList<Trigger> Triggers
        {
            get { if (_Triggers == null) _Triggers = new List<Trigger>(); return _Triggers; }
            private set { _Triggers = value; }
        }
    }

    public sealed class Trigger
    {
        private IList<Condition> _StartConditions;
        public IList<Condition> StartConditions
        {
            get { if (_StartConditions == null) _StartConditions = new List<Condition>(); return _StartConditions; }
            private set { _StartConditions = value; }
        }

        private IList<Condition> _EndConditions;
        public IList<Condition> EndConditions
        {
            get { if (_EndConditions == null) _EndConditions = new List<Condition>(); return _EndConditions; }
            private set { _EndConditions = value; }
        }

        private IList<Source> _Sources;
        public IList<Source> Sources
        {
            get { if (_Sources == null) _Sources = new List<Source>(); return _Sources; }
            private set { _Sources = value; }
        }

        private string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Description { get; set; }
    }

    public sealed class Condition
    {
        private IList<Condition> _Conditions;
        public IList<Condition> Conditions
        {
            get { if (_Conditions == null) _Conditions = new List<Condition>(); return _Conditions; }
            private set { _Conditions = value; }
        }

        public ConditionType? Type { get; set; }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Value;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

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
        private IList<Target> _Targets;
        public IList<Target> Targets
        {
            get { if (_Targets == null) _Targets = new List<Target>(); return _Targets; }
            private set { _Targets = value; }
        }

        private string _Region;
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
    }

    public sealed class Source
    {
        private IList<Source> _Sources;
        public IList<Source> Sources
        {
            get { if (_Sources == null) _Sources = new List<Source>(); return _Sources; }
            private set { _Sources = value; }
        }

        private IList<Target> _Targets;
        public IList<Target> Targets
        {
            get { if (_Targets == null) _Targets = new List<Target>(); return _Targets; }
            private set { _Targets = value; }
        }

        private string _Uri;
        public string Uri
        {
            get { return _Uri; }
            set { _Uri = value; }
        }

        private string _AltReference;
        public string AltReference
        {
            get { return _AltReference; }
            set { _AltReference = value; }
        }

        private string _Format;
        public string Format
        {
            get { return _Format; }
            set { _Format = value; }
        }
    }
}
