#define CODE_ANALYSIS

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Used to bind strings in Xaml.
    /// This is necessary merely because the auto-generated resource code file has an internal constructor which cannot be created from Xaml.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Can be used from xaml")]
    public class LocalizedStrings
    {
        private static Resources resources = new Resources();

        /// <summary>
        /// Gets a reference to the Resources object.
        /// </summary>
        public static Resources ResourceFile { get { return resources; } }
    }

}
