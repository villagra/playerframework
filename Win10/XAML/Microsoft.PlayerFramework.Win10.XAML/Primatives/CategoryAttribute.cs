
namespace System.ComponentModel
{
    /// <summary>
    /// Identifies the category of member for Blend.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of CategoryAttribute.
        /// </summary>
        /// <param name="category">The category name.</param>
        public CategoryAttribute(string category)
        { }
    }
}
