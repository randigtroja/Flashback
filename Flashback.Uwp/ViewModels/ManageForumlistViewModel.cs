using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flashback.Model;

namespace FlashbackUwp.ViewModels
{
    public class ManageForumlistViewModel : FlashbackViewModelBase
    {
        private string _addPath = "";
        public string AddPath { get { return _addPath; } set { Set(ref _addPath, value); } }

        private string _addName = "";
        public string AddName { get { return _addName; } set { Set(ref _addName, value); } }

        public ObservableCollection<FbItem> ExtraForumList { get; set; }

        public ManageForumlistViewModel()
        {
            
        }

        public async Task AddForum()
        {
            var forum = new FbItem
            {
                Id = AddPath,
                Name = AddName,
                Type = FbItemType.Forum
            };

            if (!ValidateAndFixPaths(forum))
            {                
                await new Windows.UI.Popups.MessageDialog("Ej kompletta uppgifter (fyll i namn, och sökvägen)").ShowAsync();
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


        }

        private bool ValidateAndFixPaths(FbItem forum)
        {
            if (string.IsNullOrWhiteSpace(forum.Id) || string.IsNullOrWhiteSpace(forum.Name))
                return false;

            forum.Id = forum.Id.Replace("/", "");

            return forum.Id.Contains("f");
        }
    }
}
