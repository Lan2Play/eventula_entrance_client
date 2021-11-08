using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace eventula_entrance_client.Views
{
    public partial class SelectedUsersView : UserControl
    {
        public SelectedUsersView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}