using RMD.Excercise.TextureManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMD.TextureManager.Model;
using System.ComponentModel.Composition;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using Caliburn.Micro;

namespace RMD.Excercise.TextureProviders
{
    [Export(typeof(ITextureProvider))]
    public class FolderTextureProvider : ITextureProvider
    {
        private ILog _log = LogManager.GetLog(typeof(FolderTextureProvider));

        #region Interface methods

        public void GetTextures(string source, string filter, BlockingCollection<TextureModel> textureModels)
        {
            GetTextureModelsFromFolder(source, filter, textureModels);
            textureModels.CompleteAdding();
        }

        public bool DeleteTexture(TextureModel model)
        {
            if (model == null)
            {
                return true;
            }

            if (File.Exists(model.Id))
            {
                File.Delete(model.Id);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Interface methods

        private void GetTextureModelsFromFolder(string folder, string filter, BlockingCollection<TextureModel> textureModels)
        {
            List<TextureModel> retval = new List<TextureModel>();
            IEnumerable<string> directories = null;

            try
            {
                string[] files = GetFiles(folder, filter, SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    textureModels.Add(new TextureModel(file));

#if DEBUG
                    // To test UI blocking
                    //Thread.Sleep(50);
#endif
                }

                directories = Directory.EnumerateDirectories(folder);
            }
            catch (UnauthorizedAccessException ex)
            {
                _log.Error(ex);
            }
            catch (PathTooLongException ex)
            {
                _log.Error(ex);
            }

            if (directories != null)
            {
                foreach (string dir in directories)
                {
                    GetTextureModelsFromFolder(dir, filter, textureModels);
                }
            }
        }
        
        private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption = SearchOption.AllDirectories)
        {
            return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }
    }
}
