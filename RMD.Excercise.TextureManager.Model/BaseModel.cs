using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.TextureManager.Model
{
    public class BaseModel : PropertyChangedBase
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public override int GetHashCode()
        {
            return new { Id }.GetHashCode();
        }
    }
}
