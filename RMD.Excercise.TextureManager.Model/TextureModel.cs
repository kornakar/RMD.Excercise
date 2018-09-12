using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.TextureManager.Model
{
    // TODO: calculate a hash from the "duplicate condition"
    public class TextureModel : BaseModel
    {
        #region Properties

        private string _fileNameWithoutExtension;
        public string FileNameWithoutExtension
        {
            get { return _fileNameWithoutExtension; }
            set
            {
                _fileNameWithoutExtension = value;
                NotifyOfPropertyChange(() => FileNameWithoutExtension);
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }

        private long? _size;
        public long? Size
        {
            get { return _size; }
            set
            {
                _size = value;
                NotifyOfPropertyChange(() => Size);
            }
        }

        #endregion Properties

        #region Constructors

        public TextureModel(string fullPath)
        {
            Id = fullPath;
            FileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            Path = System.IO.Path.GetDirectoryName(fullPath);
        }

        #endregion Constructors

        #region Methods

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion Methods
    }
}
