using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using FlashbackUwp.Services.FileServices;
using GalaSoft.MvvmLight.Messaging;

namespace FlashbackUwp.ViewModels
{
    public class ManageForumlistViewModel : FlashbackViewModelBase
    {
        private string _addPath = "";
        public string AddPath
        {
            get => _addPath;
            set => Set(ref _addPath, value);
        }

        private string _addName = "";
        public string AddName
        {
            get => _addName;
            set => Set(ref _addName, value);
        }

        private ObservableCollection<FbItem> _extraForums;
        public ObservableCollection<FbItem> ExtraForumList
        {
            get => _extraForums;
            set => Set(ref _extraForums, value);
        }

        private FileService _fileService;

        public ManageForumlistViewModel()
        {           
            ExtraForumList = new ObservableCollection<FbItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                ExtraForumList = SampleData.SampleData.GetDefaultExtraForums();
            }            
        }

        public async Task DeleteForum(FbItem item)
        {            
            ExtraForumList.Remove(item);
            await _fileService.SaveExtraForums(ExtraForumList.ToList());
        }

        public async Task AddForum()
        {
            var forum = new FbItem
            {
                Id = AddPath,
                Name = AddName,
                Type = FbItemType.Forum,
                ShowForumColor = true
            };

            if (!ValidateAndFixPaths(forum))
            {                                
                Messenger.Default.Send("Ej kompletta uppgifter (fyll i namn, och sökvägen)", FlashbackConstants.MessengerShowWarning);
                return;
            }

            if (ExtraForumList.Any(x => x.Id == forum.Id))
            {
                AddName = "";
                AddPath = "";
                return;
            }
            
            ExtraForumList.Add(forum);
            AddName = "";
            AddPath = "";
            
            await _fileService.SaveExtraForums(ExtraForumList.ToList());
        }

        public async Task LoadViewModel()
        {
            _fileService = new FileService();
            var forums = await _fileService.GetExtraForums();

            ExtraForumList =  new ObservableCollection<FbItem>(forums);
        }

        private bool ValidateAndFixPaths(FbItem forum)
        {
            if (string.IsNullOrWhiteSpace(forum.Id) || string.IsNullOrWhiteSpace(forum.Name))
                return false;

            forum.Id = forum.Id.Replace("/", "");
            forum.Id = forum.Id.ToLower();

            return forum.Id.Contains("f");
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await LoadViewModel();
        }
    }
}
