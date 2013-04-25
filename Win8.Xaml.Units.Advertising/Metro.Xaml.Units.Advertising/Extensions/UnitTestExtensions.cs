using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Microsoft.VisualStudio.TestPlatform.UnitTestFramework
{
    internal static class UnitTestExtensions
    {
        public static async Task ThrowsExceptionAsync<T>(Func<Task> action) where T : Exception
        {
            try
            {
                await action();
                Assert.Fail();
            }
            catch (T)
            { }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
