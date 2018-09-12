using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.TextureManager.Model
{
    public class FolderModel : BaseModel
    {
        private IList<FolderModel> _subFolders;
        public IList<FolderModel> SubFolders
        {
            get { return _subFolders; }
            private set
            {
                _subFolders = value;
                NotifyOfPropertyChange(() => SubFolders);
            }
        }

        private IList<TextureModel> _textureModels;
        public IList<TextureModel> TextureModels
        {
            get { return _textureModels; }
            private set
            {
                _textureModels = value;
                NotifyOfPropertyChange(() => TextureModels);
            }
        }
    }
}
