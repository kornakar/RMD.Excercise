using RMD.TextureManager.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.TextureManager.Interface
{
    public interface ITextureProvider
    {
        /// <summary>
        /// Gets all the textures from the source
        /// </summary>
        /// <param name="source">Source identification (path, connection string etc.)</param>
        /// <param name="filter">Filter for textures (search pattern, where clause etc.)</param>
        /// <param name="textureModels">TextureModels to be returned</param>
        void GetTextures(string source, string filter, BlockingCollection<TextureModel> textureModels);

        /// <summary>
        /// Deletes the texture from the source
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteTexture(TextureModel model);
    }
}
