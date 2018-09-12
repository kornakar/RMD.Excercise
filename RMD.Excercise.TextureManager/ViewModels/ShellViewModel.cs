using Caliburn.Micro;
using RMD.Excercise.TextureManager.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace RMD.Excercise.TextureManager.ViewModels
{
    // TODO:
    // - siirrä kaikki tekstit resourcesiin
    // - testaa että kääntyy toisella koneella
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<ITool>.Collection.OneActive, IShell
    {
        [ImportMany(typeof(ITool))]
        private IEnumerable<ITool> _tools;

        public IEnumerable<ITool> Tools
        {
            get { return _tools.OrderBy(x => x.TabOrder); }
        }

        public ShellViewModel()
        {
            _tools = new BindableCollection<ITool>();
            DisplayName = "Remedy Tools Manager";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }
    }
}
