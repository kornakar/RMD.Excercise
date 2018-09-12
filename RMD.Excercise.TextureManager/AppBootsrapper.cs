using Caliburn.Micro;
using RMD.Excercise.Logger;
using RMD.Excercise.TextureManager.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using System;
using System.ComponentModel.Composition.Primitives;
using RMD.Excercise.TextureManager.Interface;

namespace RMD.Excercise.TextureManager
{
    public class AppBootstrapper : BootstrapperBase
    {
        protected CompositionContainer Container { get; set; }

        public AppBootstrapper()
        {
            this.Initialize();
            LogManager.GetLog = type => new NLogLogger(type);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootViewFor<IShell>();
        }

        protected override void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        protected override object GetInstance(Type service, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;
            var exports = Container.GetExports<object>(contract);

            if (exports.Any())
                return exports.First().Value;

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override void Configure()
        {
            DirectoryCatalog directoryCatalog = new DirectoryCatalog(@"./");
            AssemblySource.Instance.AddRange(
                     directoryCatalog.Parts.Select(p => ReflectionModelServices.GetPartType(p).Value.Assembly)
            );

            AggregateCatalog catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());
            CatalogExportProvider provider = new CatalogExportProvider(catalog);

            Container = new CompositionContainer(new ApplicationCatalog());
            provider.SourceProvider = Container;

            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(Container);
            batch.AddExportedValue(catalog);
            batch.AddExportedValue(this);

            Container.Compose(batch);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetEntryAssembly() };
        }
    }
}
