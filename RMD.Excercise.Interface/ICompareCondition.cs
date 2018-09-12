using RMD.TextureManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.Interface
{
    public interface ICompareCondition<T> where T : BaseModel
    {
        /// <summary>
        /// Returns true if the given models are considered duplicated by this CompareCondition
        /// </summary>
        /// <param name="model1">Model 1</param>
        /// <param name="model2">Model 2</param>
        /// <returns></returns>
        bool IsDuplicate(T model1, T model2);

        /// <summary>
        /// Returns one of the two models which is not as important as the other one (i.e. can be deleted)
        /// </summary>
        /// <param name="model1">Model 1</param>
        /// <param name="model2">Model 2</param>
        /// <returns></returns>
        T GetLessImportantDuplicate(T model1, T model2);
    }
}
