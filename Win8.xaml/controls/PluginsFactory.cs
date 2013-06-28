#define CODE_ANALYSIS

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
#if MEF
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
#endif
#if SILVERLIGHT
#else
using System.Reflection;
using Windows.UI.Xaml;
using System.Threading.Tasks;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A factory class to help encapsulate MEF (Managed Extensibility Framework) to load MMP: Player Framework plugins.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class PluginsFactory
    {
        internal PluginsFactory()
        {
        }
        
#if MEF
#if SILVERLIGHT
        
        internal void ImportPlugins()
        {
            CompositionInitializer.SatisfyImports(this);
        }
#else
        private async Task<IEnumerable<Assembly>> GetAssemblyListAsync()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    try
                    {
                        var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                        AssemblyName name = new AssemblyName() { Name = filename };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                    catch { /* ignore */ }
                }
            }

            return assemblies;
        }

        internal async Task ImportPlugins()
        {
            var assemblies = await GetAssemblyListAsync();
            var catalog = new AggregateCatalog(assemblies.Select(a => new AssemblyCatalog(a)));
            //var catalog = new AggregateCatalog(mainAssemblyCatalog, thisAssemblyCatalog, mmppfAssemblyCatalog);
            //var catalog = new ApplicationCatalog();
            //var catalog = new ApplicationCatalog(mainAssemblyCatalog);
            using (var service = catalog.CreateCompositionService())
            {
                service.SatisfyImportsOnce(this);
            }
        }
#endif

#else
        internal void ImportPlugins()
        {
            Plugins = new List<IPlugin>(new IPlugin[] { 
                new BufferingPlugin(),
                new CaptionSelectorPlugin(),
                new AudioSelectionPlugin(),
                new ChaptersPlugin(),
                new ErrorPlugin(),
                new LoaderPlugin(),
#if SILVERLIGHT
                new PosterPlugin(),
#elif NETFX_CORE
                new MediaControlPlugin(),
                new DisplayRequestPlugin(),
#endif
            });
        }
#endif

        /// <summary>
        /// The plugins to get imported.
        /// </summary>
#if MEF
        [System.ComponentModel.Composition.ImportMany(typeof(IPlugin))]
#endif
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
        public IEnumerable<IPlugin> Plugins { get; set; }
    }
}
