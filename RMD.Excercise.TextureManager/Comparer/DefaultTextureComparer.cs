using RMD.Excercise.Interface;
using RMD.TextureManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.TextureManager.Comparer
{
    [Export(typeof(ITextureComparer<TextureModel>))]
    public class DefaultTextureComparer : ITextureComparer<TextureModel>
    {
        private List<ICompareCondition<TextureModel>> _compareConditions = new List<ICompareCondition<TextureModel>>();
        public List<ICompareCondition<TextureModel>> CompareConditions
        {
            get { return _compareConditions; }
            set { _compareConditions = value; }
        }

        [ImportingConstructor]
        public DefaultTextureComparer([ImportMany] IEnumerable<ICompareCondition<TextureModel>> compareConditions)
        {
            _compareConditions.AddRange(compareConditions);
        }

        public TextureModel FindDuplicateForTexture(TextureModel model, IEnumerable<TextureModel> allModels)
        {
            TextureModel duplicate = null;

            foreach (ICompareCondition<TextureModel> condition in _compareConditions)
            {
                duplicate = allModels.FirstOrDefault(m => condition.IsDuplicate(model, m));
                if (duplicate != null)
                {
                    break;
                }
            }

            return duplicate;
        }

        public TextureModel GetDeletableModel(TextureModel model1, TextureModel model2)
        {
            TextureModel deletableModel = null;

            foreach (ICompareCondition<TextureModel> condition in _compareConditions)
            {
                deletableModel = condition.GetLessImportantDuplicate(model1, model2);
                if (deletableModel != null)
                {
                    break;
                }
            }

            return deletableModel;
        }
    }
}
