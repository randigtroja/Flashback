using Windows.UI.Xaml.Controls;
using FlashbackUwp.ViewModels;

namespace FlashbackUwp.UserControls
{
    public sealed partial class ImageViewer : UserControl
    {
        public ImageViewer()
        {
            InitializeComponent();
            DataContext = new ImageViewerViewModel();
        }

        public ImageViewer(ImageViewerViewModel model) => DataContext = model;
    }
}
