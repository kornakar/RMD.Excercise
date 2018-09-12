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
    /// TextureProvider for testing the application with duplicates
    /// </summary>
    //[Export(typeof(ITextureProvider))]
    public class DuplicateTextureProvider : ITextureProvider
    {
        private int _numberOfFilesToGenerate = 256;
        private IList<string> _fileExtensions;
        Random _rnd = new Random();

        public DuplicateTextureProvider()
        {
        }

        public DuplicateTextureProvider(int numberOfFilesToGenerate)
        {
            _numberOfFilesToGenerate = numberOfFilesToGenerate;
        }

        public bool DeleteTexture(TextureModel model)
        {
            return true;
        }

        public void GetTextures(string source, string filter, BlockingCollection<TextureModel> textureModels)
        {
            _fileExtensions = filter.Split('|').Select(x => x.TrimStart('*')).ToList();
            int numberOfExtensions = _fileExtensions.Count;

            for(int i = 0; i < _numberOfFilesToGenerate; i += numberOfExtensions)
            {
                string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

                for (int j = 0; j < numberOfExtensions; j += 1)
                {
                    string extension = _fileExtensions[j];
                    textureModels.Add(new TextureModel(Path.Combine(source, fileName + extension)));
                }
            }

            textureModels.CompleteAdding();
        }
    }
}
