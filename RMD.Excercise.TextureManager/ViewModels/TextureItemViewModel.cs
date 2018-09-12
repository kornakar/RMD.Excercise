using Caliburn.Micro;
using RMD.TextureManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.TextureManager.ViewModels
{
    public class TextureItemViewModel : PropertyChangedBase
    {
        private TextureModel _model;
        public TextureModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Id
        {
            get { return _model.Id; }
        }

        public string Path
        {
            get { return _model.Path; }
        }

        private bool? _hasDuplicate;
        public bool? HasDuplicate
        {
            get { return _hasDuplicate; }
            private set
            {
                _hasDuplicate = value;
                NotifyOfPropertyChange(() => HasDuplicate);
            }
        }

        private TextureModel _duplicateModel;
        public TextureModel DuplicateModel
        {
            get { return _duplicateModel; }
            private set
            {
                _duplicateModel = value;
                NotifyOfPropertyChange(() => DuplicateModel);
            }
        }

        public void SetDuplicateModel(TextureModel model)
        {
            DuplicateModel = model;
            HasDuplicate = model != null;
        }

        public void ClearDuplicateModel()
        {
            DuplicateModel = null;
            HasDuplicate =  null;
        }
    }
}
