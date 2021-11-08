using System;
using System.Collections.Generic;
using System.Text;
using eventula_entrance_client.Services;

namespace eventula_entrance_client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public MainWindowViewModel(Database db)
        {
            db.InitializeDB();
            NewUsers = new NewUsersViewModel(db.GetNewUsers());
            SelectedUsers = new SelectedUsersViewModel(db.GetSelectedUsers());
        }
        
        public NewUsersViewModel NewUsers { get; }
        public SelectedUsersViewModel SelectedUsers { get; }

    }
}
