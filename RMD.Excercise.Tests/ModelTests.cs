using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMD.TextureManager.Model;
using RMD.Excercise.Interface;
using RMD.Excercise.CompareConditions;
using System.IO;

namespace RMD.Excercise.Tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void TestCreateNewTextureModel()
        {
            // Arrange
            string path = @"c:\foo";
            string fileName = "testimage";
            string extension = ".png";
            string id = Path.Combine(path, fileName) + extension;

            // Act
            TextureModel model = new TextureModel(id);

            // Assert
            Assert.AreEqual(path, model.Path, "Path doesn't match!");
            Assert.AreEqual(fileName, model.FileNameWithoutExtension, "File name without extension doesn't match!");
            Assert.AreEqual(id, model.Id, "Id doesn't match!");
        }

        [TestMethod]
        public void TestAreModelsEqual()
        {
            // Arrange
            string path = @"c:\foo";
            string fileName = "testimage";
            string extension = ".png";
            string id = Path.Combine(path, fileName) + extension;

            // Act
            TextureModel modelA = new TextureModel(id);
            TextureModel modelB = modelA;
            bool equals = modelA.Equals(modelB);

            // Assert
            Assert.AreEqual(path, modelA.Path, "Path doesn't match in model A!");
            Assert.AreEqual(fileName, modelA.FileNameWithoutExtension, "File name without extension doesn't match in model A!");
            Assert.AreEqual(id, modelA.Id, "Id doesn't match in model A!");
            Assert.AreEqual(path, modelB.Path, "Path doesn't match in model B!");
            Assert.AreEqual(fileName, modelB.FileNameWithoutExtension, "File name without extension doesn't match in model B!");
            Assert.AreEqual(id, modelB.Id, "Id doesn't match in model B!");
            Assert.IsTrue(equals, "Models aren't equal!");
        }
    }
}
