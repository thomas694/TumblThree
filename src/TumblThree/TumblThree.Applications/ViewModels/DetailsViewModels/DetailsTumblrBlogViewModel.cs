﻿using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Forms;
using System.Windows.Input;
using TumblThree.Applications.Services;
using TumblThree.Applications.Views;
using TumblThree.Domain.Models.Blogs;

namespace TumblThree.Applications.ViewModels.DetailsViewModels
{
    [Export(typeof(IDetailsViewModel))]
    [ExportMetadata("BlogType", typeof(TumblrBlog))]
    public class DetailsTumblrBlogViewModel : ViewModel<IDetailsView>, IDetailsViewModel
    {
        private readonly DelegateCommand _browseFileDownloadLocationCommand;
        private readonly DelegateCommand _copyUrlCommand;

        private readonly ExportFactory<FullScreenMediaViewModel> _fullScreenMediaViewModelFactory;
        private readonly IShellService _shellService;
        private readonly IClipboardService _clipboardService;
        private readonly IDetailsService _detailsService;
        private IBlog _blogFile;
        private int _count;

        [ImportingConstructor]
        public DetailsTumblrBlogViewModel([Import("TumblrBlogView", typeof(IDetailsView))] IDetailsView view, IClipboardService clipboardService, IDetailsService detailsService,
            IShellService shellService, ExportFactory<FullScreenMediaViewModel> fullScreenMediaViewModelFactory)
            : base(view)
        {
            _clipboardService = clipboardService;
            _detailsService = detailsService;
            _copyUrlCommand = new DelegateCommand(CopyUrlToClipboard);
            _browseFileDownloadLocationCommand = new DelegateCommand(BrowseFileDownloadLocation);
            _shellService = shellService;
            _fullScreenMediaViewModelFactory = fullScreenMediaViewModelFactory;
        }

        public ICommand CopyUrlCommand => _copyUrlCommand;

        public ICommand BrowseFileDownloadLocationCommand => _browseFileDownloadLocationCommand;

        public void ViewFullScreenMedia()
        {
            _detailsService.ViewFullScreenMedia();
        }

        public void ViewLostFocus()
        {
            if (Count == 1) BlogFile?.Save();
        }

        public bool FilenameTemplateValidate(string enteredFilenameTemplate)
        {
            return _detailsService.FilenameTemplateValidate(enteredFilenameTemplate);
        }

        public IBlog BlogFile
        {
            get => _blogFile;
            set => SetProperty(ref _blogFile, value);
        }

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        private void CopyUrlToClipboard()
        {
            if (BlogFile != null)
            {
                _clipboardService.SetText(BlogFile.Url);
            }
        }

        private void BrowseFileDownloadLocation()
        {
            using (var dialog = new FolderBrowserDialog { SelectedPath = BlogFile?.FileDownloadLocation })
            {
                if (dialog.ShowDialog() == DialogResult.OK && BlogFile != null)
                {
                    BlogFile.FileDownloadLocation = dialog.SelectedPath;
                }
            }
        }
    }
}
