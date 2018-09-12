using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMD.Excercise.TextureManager.ViewModels;
using RMD.TextureManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using RMD.Excercise.TextureManager.Interface;
using RMD.Excercise.Interface;
using System.Threading;
using RMD.Excercise.TextureProviders;

namespace RMD.Excercise.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        private Mock<ITextureProvider> _providerMock = new Mock<ITextureProvider>();
        private Mock<ITextureComparer<TextureModel>> _comparerMock = new Mock<ITextureComparer<TextureModel>>();
        private Mock<IDialogService> _dialogMock = new Mock<IDialogService>();

        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void TestTextureItemViewModel_HasDuplicate()
        {
            // Arrange
            string path = @"c:\foo";
            string fileName = "testimage";
            string extension = ".png";
            string extension2 = ".tga";
            string id = Path.Combine(path, fileName) + extension;
            string id2 = Path.Combine(path, fileName) + extension2;
            TextureModel modelA = new TextureModel(id);
            TextureModel modelB = new TextureModel(id2);

            // Act
            TextureItemViewModel vm = new TextureItemViewModel();
            vm.Model = modelA;
            vm.SetDuplicateModel(modelB);

            // Assert
            Assert.IsNotNull(vm.DuplicateModel, "DuplicateModel property was null!");
            Assert.AreSame(modelB, vm.DuplicateModel, "DuplicateModel property was not ModelB!");
            Assert.IsNotNull(vm.HasDuplicate, "HasDuplicate property was null!");
            Assert.IsTrue(vm.HasDuplicate.Value, "HasDuplicate property was not True");
        }

        [TestMethod]
        public void TestTextureItemViewModel_SetAndClearDuplicate()
        {
            // Arrange
            string path = @"c:\foo";
            string fileName = "testimage";
            string extension = ".png";
            string extension2 = ".tga";
            string id = Path.Combine(path, fileName) + extension;
            string id2 = Path.Combine(path, fileName) + extension2;
            TextureModel modelA = new TextureModel(id);
            TextureModel modelB = new TextureModel(id2);
            TextureItemViewModel vm = new TextureItemViewModel();

            // Act
            vm.Model = modelA;
            vm.SetDuplicateModel(modelB);
            vm.ClearDuplicateModel();

            // Assert
            Assert.IsNull(vm.DuplicateModel, "DuplicateModel property was NOT null!");
            Assert.IsNull(vm.HasDuplicate, "HasDuplicate property was not null!");
        }

        [TestMethod]
        public void TestShellViewModel_DeleteAllDuplicates_NoTextures()
        {
            // Arrange
            TextureManagerViewModel vm = new TextureManagerViewModel(_providerMock.Object, _comparerMock.Object, _dialogMock.Object);

            // Act
            vm.DeleteAllDuplicates();

            // Assert
            _dialogMock.Verify(
                x => x.ShowMessageBox(It.IsAny<string>(), 
                                      It.IsAny<string>(),
                                      System.Windows.MessageBoxButton.OK, 
                                      It.IsAny<System.Windows.MessageBoxImage>()));
        }
    }
}
