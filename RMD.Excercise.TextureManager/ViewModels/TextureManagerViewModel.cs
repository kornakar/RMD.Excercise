using Caliburn.Micro;
using RMD.Excercise.Interface;
using RMD.Excercise.TextureManager.Interface;
using RMD.Excercise.TextureManager.Properties;
using RMD.TextureManager.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace RMD.Excercise.TextureManager.ViewModels
{
    [Export(typeof(ITool))]
    public class TextureManagerViewModel : Screen, ITool
    {
        #region Fields

        private object _lockObj = new object();
        private object _texturesLock = new object();
        private bool? _sortOrder = null;
        private BackgroundWorker _loaderWorker;
        private BackgroundWorker _viewmodelWorker;
        private BackgroundWorker _sortWorker;
        private readonly ILog _log;

        private string _sourcePath = null;
        private string _searchPattern = null;
        private BlockingCollection<TextureModel> _modelsBuffer = new BlockingCollection<TextureModel>();
        private BlockingCollection<TextureModel> _models = new BlockingCollection<TextureModel>();

        private IObservableCollection<TextureItemViewModel> _textures = new BindableCollection<TextureItemViewModel>();
        private List<TextureItemViewModel> _selectedTextures = new List<TextureItemViewModel>();

#pragma warning disable 649
        [Import]
        private ITextureProvider _textureProvider;

        [Import]
        private ITextureComparer<TextureModel> _textureComparer;

        [Import]
        private IDialogService _dialogService;
#pragma warning restore 649

        #endregion Fields

        #region Properties

        public int TabOrder
        {
            get
            {
                return 1;
            }
        }

        protected ITextureProvider TextureProvider
        {
            get
            {
                return _textureProvider;
            }
        }

        protected ITextureComparer<TextureModel> TextureComparer
        {
            get
            {
                return _textureComparer;
            }
        }

        protected IDialogService DialogService
        {
            get
            {
                return _dialogService;
            }
        }

        public int NumberOfTextures
        {
            get
            {
                if (_textures == null)
                {
                    return 0;
                }

                return _textures.Count;
            }
        }

        public int NumberOfDuplicates
        {
            get
            {
                if (_textures == null)
                {
                    return 0;
                }

                return _textures.Count(x => x.HasDuplicate.HasValue && x.HasDuplicate.Value) / 2;
            }
        }

        public IObservableCollection<TextureItemViewModel> Textures
        {
            get { return _textures; }
            set
            {
                _textures = value;
                BindingOperations.EnableCollectionSynchronization(_textures, _texturesLock);
            }
        }

        // Could this be done with CollectionView instead?
        public IEnumerable<TextureItemViewModel> TexturesToShow
        {
            get
            {
                lock (_lockObj)
                {
                    if (ShowOnlyDuplicates)
                    {
                        return _textures.Where(t => t.HasDuplicate.GetValueOrDefault(false));
                    }
                    else
                    {
                        return _textures;
                    }
                }
            }
        }

        public List<TextureItemViewModel> SelectedTextures
        {
            get { return _selectedTextures; }
            set
            {
                _selectedTextures = value;
            }
        }

        private bool _showOnlyDuplicates = true;
        public bool ShowOnlyDuplicates
        {
            get { return _showOnlyDuplicates; }
            set
            {
                _showOnlyDuplicates = value;
                NotifyOfPropertyChange(() => ShowOnlyDuplicates);
                TriggerTexturesToShowRefresh();
            }
        }

        private string _statusText = "Ready.";
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;
                NotifyOfPropertyChange(() => SourcePath);
            }
        }

        public string SearchPattern
        {
            get { return _searchPattern; }
            set
            {
                _searchPattern = value;
                NotifyOfPropertyChange(() => SearchPattern);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
                UpdateCommands();
            }
        }

        public bool CanRefreshTextures
        {
            get { return !IsBusy; }
        }

        public bool CanDeleteAllDuplicates
        {
            get { return !IsBusy; }
        }

        public bool CanDeleteSelectedDuplicates
        {
            get { return !IsBusy && SelectedTextures.Any(x => x.HasDuplicate.HasValue && x.HasDuplicate.Value); }
        }

        public bool CanSortByName
        {
            get { return !IsBusy; }
        }

        #endregion Properties

        #region Constructors

        [ImportingConstructor]
        public TextureManagerViewModel(ITextureProvider textureProvider, ITextureComparer<TextureModel> textureComparer, IDialogService dialogService) : this()
        {
            _textureProvider = textureProvider;
            _textureComparer = textureComparer;
            _dialogService = dialogService;
        }

        public TextureManagerViewModel()
        {
            _log = LogManager.GetLog(typeof(ShellViewModel));

            _sourcePath = Settings.Default.TexturePath;
            _searchPattern = Settings.Default.SearchPattern;

            DisplayName = "TextureManager";
        }

        #endregion Constructors

        #region Methods

        protected void UpdateCommands()
        {
            NotifyOfPropertyChange(() => CanRefreshTextures);
            NotifyOfPropertyChange(() => CanDeleteAllDuplicates);
            NotifyOfPropertyChange(() => CanDeleteSelectedDuplicates);
            NotifyOfPropertyChange(() => CanSortByName);
        }

        private void SetStatus(string statusText = "Ready.")
        {
            StatusText = statusText;
        }

        private void StartLoaderWorker()
        {
            _log.Info("Initializing the LoaderWorker");

            IsBusy = true;
            SetStatus("Initializing the loader...");

            _loaderWorker = new BackgroundWorker();

            _loaderWorker.DoWork += (s, e) => {
                LoadTextures();
            };

            _loaderWorker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    _log.Error(e.Error);

                    IsBusy = false;
                    _dialogService.ShowMessageBox("There was an error! " + e.Error.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            _log.Info("Starting the LoaderWorker");
            _loaderWorker.RunWorkerAsync();
        }

        private void StartViewModelWorker()
        {
            _log.Info("Initializing the ViewModelWorker");
            _viewmodelWorker = new BackgroundWorker();

            _viewmodelWorker.DoWork += (s, e) => {
                LoadViewModels();
            };

            _viewmodelWorker.RunWorkerCompleted += (s, e) =>
            {
                Task.Factory.StartNew(() =>
                {
                    MarkDuplicates();
                    IsBusy = false;
                    SetStatus();
                });

                NotifyOfPropertyChange(() => NumberOfTextures);
                NotifyOfPropertyChange(() => NumberOfDuplicates);
                NotifyOfPropertyChange(() => TexturesToShow);
            };

            _log.Info("Starting the ViewModelWorker");
            _viewmodelWorker.RunWorkerAsync();
        }

        private void StartSortWorker()
        {
            _log.Info("Initializing the SortWorker");
            _sortWorker = new BackgroundWorker();

            _sortWorker.DoWork += (s, e) =>
            {
                if (!_sortOrder.HasValue || !_sortOrder.Value)
                {
                    _sortOrder = true;
                    Textures = new BindableCollection<TextureItemViewModel>(_textures.OrderBy(x => x.Model.FileNameWithoutExtension));
                }
                else if (_sortOrder.Value)
                {
                    _sortOrder = false;
                    Textures = new BindableCollection<TextureItemViewModel>(_textures.OrderByDescending(x => x.Model.FileNameWithoutExtension));
                }
            };

            _sortWorker.RunWorkerCompleted += (s, e) =>
            {
                SetStatus();
                NotifyOfPropertyChange(() => TexturesToShow);
            };

            _log.Info("Starting the SortWorker");
            _sortWorker.RunWorkerAsync();
        }

        private void TriggerTexturesToShowRefresh()
        {
            SetStatus("Updating visible textures...");

            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new System.Action(delegate { }));
                NotifyOfPropertyChange(() => TexturesToShow);
            }
            finally
            {
                SetStatus();
            }
        }

        private void LoadViewModels()
        {
            SetStatus("Loading textures...");

            foreach (TextureModel model in _modelsBuffer.GetConsumingEnumerable())
            {
                _models.Add(model);
                Textures.Add(new TextureItemViewModel { Model = model });
            }
        }

        private void MarkDuplicates()
        {
            SetStatus("Searching for duplicates...");

            Parallel.ForEach(_textures, vm =>
            {
                TextureModel duplicateModel = _textureComparer.FindDuplicateForTexture(vm.Model, _models);

                lock (_lockObj)
                {
                    vm.SetDuplicateModel(duplicateModel);
                }
            });

            NotifyOfPropertyChange(() => NumberOfDuplicates);
            NotifyOfPropertyChange(() => TexturesToShow);
        }

        private void LoadTextures()
        {
            try
            {
                _textureProvider.GetTextures(_sourcePath, _searchPattern, _modelsBuffer);
            }
            catch (Exception ex)
            {
                SetStatus("ERROR: " + ex.Message);
                IsBusy = false;
            }
        }

        private void DeleteDuplicates(IEnumerable<TextureItemViewModel> viewModels)
        {
            IsBusy = true;
            SetStatus("Removing duplicates...");

            _log.Info("Deleting duplicates");

            try
            {
                foreach (var textureVm in viewModels)
                {
                    TextureModel deletableModel = _textureComparer.GetDeletableModel(textureVm.Model, textureVm.DuplicateModel);
                    if (deletableModel != null)
                    {
                        TextureModel modelToDelete;
                        TextureItemViewModel duplicateVm;
                        TextureItemViewModel vmToDelete;

                        // Delete the actual duplicate
                        if (deletableModel.Equals(textureVm.Model))
                        {
                            modelToDelete = textureVm.DuplicateModel;
                            duplicateVm = GetViewModelByModel(textureVm.DuplicateModel);
                            vmToDelete = textureVm;
                        }
                        else
                        {
                            modelToDelete = textureVm.Model;
                            duplicateVm = GetViewModelByModel(textureVm.Model);
                            vmToDelete = GetViewModelByModel(textureVm.DuplicateModel);
                        }

                        if (_textureProvider.DeleteTexture(deletableModel))
                        {
                            lock (_lockObj)
                            {
                                if (duplicateVm != null)
                                {
                                    duplicateVm.SetDuplicateModel(null);
                                }

                                if (vmToDelete != null)
                                {
                                    Textures.Remove(vmToDelete);
                                }
                            }
                        }
                    }
                }

                NotifyOfPropertyChange(() => NumberOfTextures);
                NotifyOfPropertyChange(() => NumberOfDuplicates);
                NotifyOfPropertyChange(() => Textures);
                NotifyOfPropertyChange(() => TexturesToShow);

                SetStatus();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                SetStatus("ERROR: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private TextureItemViewModel GetViewModelByModel(TextureModel model)
        {
            return Textures.Where(x => x.Model != null && x.Model.Equals(model)).FirstOrDefault();
        }

        #region UI related methods

        public void RefreshTextures()
        {
            _modelsBuffer = new BlockingCollection<TextureModel>();
            _models = new BlockingCollection<TextureModel>();
            Textures = new BindableCollection<TextureItemViewModel>();

            NotifyOfPropertyChange(() => Textures);
            NotifyOfPropertyChange(() => NumberOfTextures);
            NotifyOfPropertyChange(() => NumberOfDuplicates);

            StartLoaderWorker();
            StartViewModelWorker();
        }

        public void DeleteAllDuplicates()
        {
            if (NumberOfDuplicates == 0)
            {
                DialogService.ShowMessageBox("No duplicates found.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (DialogService.ShowMessageBox(string.Format("Are you sure you want to delete all duplicates ({0})?", NumberOfDuplicates),
                                             "Delete all duplicates",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            var texturesToDelete = Textures.Where(x => x.HasDuplicate.HasValue && x.HasDuplicate.Value).ToList();
            DeleteDuplicates(texturesToDelete);
        }

        public void DeleteSelectedDuplicates()
        {
            if (DialogService.ShowMessageBox("Are you sure you want to delete the selected duplicates?",
                                             "Delete selected duplicates",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            var texturesToDelete = SelectedTextures.Where(x => x.DuplicateModel != null).ToList();
            DeleteDuplicates(texturesToDelete);
        }

        // http://stackoverflow.com/a/41563345/245748
        public void TexturesSelectionChanged(SelectionChangedEventArgs obj)
        {
            _selectedTextures.AddRange(obj.AddedItems.Cast<TextureItemViewModel>());
            obj.RemovedItems.Cast<TextureItemViewModel>().ToList().ForEach(t => _selectedTextures.Remove(t));

            NotifyOfPropertyChange(() => CanDeleteSelectedDuplicates);
        }

        public void SortByName()
        {
            SetStatus("Sorting the items...");

            StartSortWorker();
        }

        #endregion UI related methods

        #endregion Methods
    }
}
