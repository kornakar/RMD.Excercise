using RMD.Excercise.Interface;
using RMD.TextureManager.Model;
using System.ComponentModel.Composition;
using System;
using System.IO;
using System.Collections.Generic;

namespace RMD.Excercise.CompareConditions
{
    /// <summary>
    /// Comparer for a case where the models are considered identical when their path and filename (without extension) are identical.
    /// If duplicates are found then a tga file is considered a duplicate.
    /// </summary>
    [Export(typeof(ICompareCondition<TextureModel>))]
    public class HasSamePathAndFileNameCompareCondition : ICompareCondition<TextureModel>
    {
        private readonly IList<string> priorityExtensions = new List<string> { ".png" };

        public TextureModel GetLessImportantDuplicate(TextureModel model1, TextureModel model2)
        {
            // TODO: return only model1 or model2?
            if (!priorityExtensions.Contains(Path.GetExtension(model1.Id)))
            {
                return model1;
            }

            if (!priorityExtensions.Contains(Path.GetExtension(model2.Id)))
            {
                return model2;
            }

            return null;
        }

        public bool IsDuplicate(TextureModel model1, TextureModel model2)
        {
            if (model1 == null || model2 == null)
            {
                return false;
            }

            bool isFileNameSame = model1.FileNameWithoutExtension == model2.FileNameWithoutExtension;
            if (!isFileNameSame)
            {
                return false;
            }

            bool isPathSame = model1.Path == model2.Path;
            if (!isPathSame)
            {
                return false;
            }

            // We compare this last because it's the most expensive compare
            if (model1.Equals(model2))
            {
                return false;
            }

            return true;
        }
    }
}
