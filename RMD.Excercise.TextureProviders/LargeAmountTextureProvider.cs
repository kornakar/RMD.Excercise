using RMD.Excercise.TextureManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMD.TextureManager.Model;
using System.IO;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;

namespace RMD.Excercise.TextureProviders
{
    /// <summary>
    /// TextureProvider for testing the application with large amounts of data
    /// </summary>
    //[Export(typeof(ITextureProvider))]
    public class LargeAmountTextureProvider : ITextureProvider
    {
        private int _numberOfFilesToGenerate = 256;
        Random _rnd = new Random();

        public LargeAmountTextureProvider()
        {
        }

        public LargeAmountTextureProvider(int numberOfFilesToGenerate)
        {
            _numberOfFilesToGenerate = numberOfFilesToGenerate;
        }

        public bool DeleteTexture(TextureModel model)
        {
            return true;
        }

        public void GetTextures(string source, string filter, BlockingCollection<TextureModel> textureModels)
        {
            var fileExtensions = filter.Split('|').Select(x => x.TrimStart('*')).ToList();

            // always add 2 duplicates for testing purposes
            if (filter.Length > 2)
            {
                textureModels.Add(new TextureModel(Path.Combine(source, "testfile" + fileExtensions[0])));
                textureModels.Add(new TextureModel(Path.Combine(source, "testfile" + fileExtensions[1])));
            }

            Parallel.For(2, _numberOfFilesToGenerate, i => 
            {
#if DEBUG
                // To simulate slow loading
                //Thread.Sleep(2);
#endif
                string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                string extension = fileExtensions[_rnd.Next(0, fileExtensions.Count - 1)];

                textureModels.Add(new TextureModel(Path.Combine(source, fileName + extension)));
            });

            textureModels.CompleteAdding();
        }
    }
}
