using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Flashback.Model;
using FlashbackUwp.ViewModels;

namespace FlashbackUwp.Views
{
    public sealed partial class ManageForumlist : Page
    {
        public ManageForumlist() => this.InitializeComponent();

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {            
            // så jävla fult men orkar inte just nu med vymodelcommandbinding. // TODO
            FbItem forumToDelete = (sender as Button).DataContext as FbItem;
            if (forumToDelete == null) return;

            if (DataContext is ManageForumlistViewModel model)
            {
                await model.DeleteForum(forumToDelete);
            }
        }
    }
}
