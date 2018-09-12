using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMD.Excercise.CompareConditions;
using RMD.Excercise.Interface;
using RMD.TextureManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Excercise.Tests
{
    [TestClass]
    public class CompareTests
    {
        [TestMethod]
        public void TestTextureModelDuplicates_OtherFileIsNull()
        {
            // Arrange
            TextureModel model1 = new TextureModel(@"C:\test\test2\hienokuva1.png");
            TextureModel model2 = null;
            ICompareCondition<TextureModel> comparer = new HasSamePathAndFileNameCompareCondition();

            // Act
            bool isDuplicate = comparer.IsDuplicate(model1, model2);

            // Assert
            Assert.IsFalse(isDuplicate, "The files were considered as duplicates when the other file was null!");
        }

        [TestMethod]
        public void TestTextureModelDuplicates_FilesAreDuplicates()
        {
            // Arrange
            TextureModel modelA = new TextureModel(@"C:\test\test2\hienokuva1.png");
            TextureModel modelB = new TextureModel(@"C:\test\test2\hienokuva1.tga");
            ICompareCondition<TextureModel> comparer = new HasSamePathAndFileNameCompareCondition();

            // Act
            bool isDuplicate = comparer.IsDuplicate(modelA, modelB);

            // Assert
            Assert.IsTrue(isDuplicate, "The files were NOT considered as duplicates when they should!");
        }

        [TestMethod]
        public void TestTextureModelDuplicates_FilesAreNotDuplicates()
        {
            // Arrange
            TextureModel modelA = new TextureModel(@"C:\test\test2\hienokuva1.png");
            TextureModel modelB = new TextureModel(@"C:\test\test2\hienokuva2.tga");
            ICompareCondition<TextureModel> comparer = new HasSamePathAndFileNameCompareCondition();

            // Act
            bool isDuplicate = comparer.IsDuplicate(modelA, modelB);

            // Assert
            Assert.IsFalse(isDuplicate, "The files were considered as duplicates when they should NOT!");
        }

        [TestMethod]
        public void TestGetLessImportantDuplicate()
        {
            // Arrange
            TextureModel modelA = new TextureModel(@"C:\test\test2\hienokuva1.png");
            TextureModel modelB = new TextureModel(@"C:\test\test2\hienokuva2.tga");
            ICompareCondition<TextureModel> comparer = new HasSamePathAndFileNameCompareCondition();

            // Act
            TextureModel duplicateModel = comparer.GetLessImportantDuplicate(modelA, modelB);

            // Assert
            Assert.IsNotNull(duplicateModel, "Duplicate model was null!");
            Assert.AreSame(modelB, duplicateModel, "Duplicate model not model B!");
        }

        [TestMethod]
        public void TestGetLessImportantDuplicate_BothAreImportant()
        {
            // Arrange
            TextureModel modelA = new TextureModel(@"C:\test\test2\hienokuva1.png");
            TextureModel modelB = new TextureModel(@"C:\test\test2\hienokuva2.png");
            ICompareCondition<TextureModel> comparer = new HasSamePathAndFileNameCompareCondition();

            // Act
            TextureModel duplicateModel = comparer.GetLessImportantDuplicate(modelA, modelB);

            // Assert
            Assert.IsNull(duplicateModel, "Duplicate model was NOT null!");
        }
    }
}
