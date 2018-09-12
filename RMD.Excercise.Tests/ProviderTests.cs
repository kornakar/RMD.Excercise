using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMD.TextureManager.Model;
using RMD.Excercise.Interface;
using RMD.Excercise.CompareConditions;
using RMD.Excercise.TextureManager.Interface;
using RMD.Excercise.TextureProviders;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections.Concurrent;
using System.Windows.Data;
using System.Threading.Tasks;

namespace RMD.Excercise.Tests
{
    [TestClass]
    public class ProviderTests
    {
        private readonly string sourcePath = @"D:\dev\RMD.Excercise\textures";
        private readonly string searchPattern = "*.png|*.tga";

        [Ignore]
        [TestMethod]
        public void TestFolderTextureProvider()
        {
            // Arrange
            ITextureProvider provider = new FolderTextureProvider();
            object lockObject = new object();
            BlockingCollection<TextureModel> models = new BlockingCollection<TextureModel>();
            BindingOperations.EnableCollectionSynchronization(models, lockObject);

            // Act
            provider.GetTextures(sourcePath, searchPattern, models);
            foreach (TextureModel model in models)
            {
                Trace.WriteLine("Worker: " + model.Id);
            }

            // Assert
            // Not a unit test; just to test the folder
        }

        [TestMethod]
        public void TestLargeAmountTextureProvider_NumberOfFiles()
        {
            // Arrange
            string sourcePath = @"D:\dev\RMD.Excercise\textures";
            string searchPattern = "*.png|*.tga";
            int numberOfFiles = 123;
            ITextureProvider provider = new LargeAmountTextureProvider(numberOfFiles);
            BlockingCollection<TextureModel> models = new BlockingCollection<TextureModel>();

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            provider.GetTextures(sourcePath, searchPattern, models);

            // Assert
            sw.Stop();
            Assert.AreEqual(numberOfFiles, models.Count, "Model count doesn't match!");
        }
    }
}
