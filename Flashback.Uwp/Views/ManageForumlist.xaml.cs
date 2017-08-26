using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using FlashbackUwp.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlashbackUwp.Views
{    
    public sealed partial class ManageForumlist : Page
    {
        public ManageForumlist()
        {
            this.InitializeComponent();
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {            
            // så jävla fult men orkar inte just nu med vymodelcommandbinding. // TODO
            FbItem forumToDelete = (sender as Button).DataContext as FbItem;
            if (forumToDelete == null) return;

            var model = DataContext as ManageForumlistViewModel;
            if (model != null)
            {
                await model.DeleteForum(forumToDelete);
            }            
        }
    }
}
