using Caliburn.Micro;
using RMD.Excercise.TextureManager.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.TextureManager.ViewModels
{
    [Export(typeof(ITool))]
    public class OtherToolViewModel : Screen, ITool
    {
        public OtherToolViewModel()
        {
            DisplayName = "Other tool";
        }

        public int TabOrder
        {
            get
            {
                return 2;
            }
        }
    }
}
