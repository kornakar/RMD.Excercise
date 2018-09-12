using RMD.TextureManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.Interface
{
    public interface ITextureComparer<T> where T : BaseModel
    {
        /// <summary>
        /// Gets or sets the CompareConditions on this comparer
        /// </summary>
        List<ICompareCondition<T>> CompareConditions { get; set; }

        /// <summary>
        /// Finds the duplicate model for the model from given models
        /// </summary>
        /// <param name="model">The model to find duplicate for</param>
        /// <param name="allModels">Models to search the duplicate</param>
        /// <returns></returns>
        T FindDuplicateForTexture(T model, IEnumerable<T> allModels);

        /// <summary>
        /// Returns the model that can be deleted (is less important duplicate)
        /// </summary>
        /// <param name="model1">Duplicate model 1</param>
        /// <param name="model2">Duplicate model 2</param>
        /// <returns></returns>
        TextureModel GetDeletableModel(TextureModel model1, TextureModel model2);
    }
}
