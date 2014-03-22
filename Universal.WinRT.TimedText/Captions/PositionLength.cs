using System;

namespace Microsoft.Media.TimedText
{
    /// <summary>
    /// Used to store a position that can either be an offset (units from edge) or an alignment
    /// </summary>
    public class PositionLength
    {
        public PositionLength(PositionLengthUnit unit)
        {
            Unit = unit;
        }

        public PositionLength(PositionLengthUnit unit, double value)
            : this(unit)
        {
            Value = value;
        }

        public double Value { get; private set; }
        public PositionLengthUnit Unit { get; private set; }

        public static PositionLength Parse(string value)
        {
            switch (value)
            {
                case "left":
                case "top":
                    return new PositionLength(PositionLengthUnit.NearAlign);
                case "center":
                    return new PositionLength(PositionLengthUnit.CenterAlign);
                case "right":
                case "bottom":
                    return new PositionLength(PositionLengthUnit.FarAlign);
                case "inherit":
                    return new PositionLength(PositionLengthUnit.Inherit);
                default:
                    var vau = ValueAndUnit.Parse(value.Trim());
                    if (vau.Unit == "%")
                    {
                        return new PositionLength(PositionLengthUnit.Percentage, vau.Value / 100);
                    }
                    else
                    {
                        // QUESTION: are other units supported? Right now we just treat anything but a percentage as absolute (pixels)
                        return new PositionLength(PositionLengthUnit.Absolute, vau.Value);
                    }
            }
        }
    }

    internal class ValueAndUnit
    {
        public double Value { get; set; }
        public string Unit { get; set; }

        public static ValueAndUnit Parse(string stringValue)
        {
            for (int i = stringValue.Length; i > 0; i--)
            {
                double result = 0;
                if (double.TryParse(stringValue.Substring(0, i), out result))
                {
                    return new ValueAndUnit()
                    {
                        Value = result,
                        Unit = stringValue.Substring(i)
                    };
                }
            }
            throw new ArgumentException();
        }
    }

    public enum PositionLengthUnit
    {
        Percentage,
        Absolute,
        NearAlign,
        CenterAlign,
        FarAlign,
        Inherit
    }
}
