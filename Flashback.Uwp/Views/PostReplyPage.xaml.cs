using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FlashbackUwp.Views
{   
    public sealed partial class PostReplyPage : Page
    {
        public PostReplyPage() => InitializeComponent();

        bool _confirmLeaving;

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Det här är ett jävla hack...
            // 1) Det funkar inte korrekt med att sätta e.cancel = true / false
            // 2) template10 buggar om man gör detta i vymodellen
            // 3) om navingeringen bort sker till ett menyalternativ i hamburgarmenyn så sätts den default till selected även om man avbryter
            // Vi sätter om menyns selected till null då sidan ändå inte hänger på någon meny och vi använder oss av manuell-backnavigering om man väljer ja            
            // vi måste sätta den sist annars triggar menyn sig själv ytterligare en navigering. Jag orkar inte mer än så här just nu.
            if (!string.IsNullOrWhiteSpace(MessageField.Text) && !_confirmLeaving)
            {
                e.Cancel = true;
                
                var dialog = new Windows.UI.Popups.MessageDialog("Är du säker på att du vill lämna sidan?", "Bekräfta");

                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ja") {Id = 0});
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Avbryt") {Id = 1});

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                if ((await dialog.ShowAsync()).Label == "Ja")
                {
                    _confirmLeaving = true;
                    ViewModel.NavigationService.GoBack();
                }
                else
                {
                    Shell.HamburgerMenu.Selected = null;
                }
            }
        }
    }
}
