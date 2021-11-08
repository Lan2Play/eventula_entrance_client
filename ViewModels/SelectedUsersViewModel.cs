using System.Collections.Generic;
using System.Collections.ObjectModel;
using eventula_entrance_client.Models;

namespace eventula_entrance_client.ViewModels
{
    public class SelectedUsersViewModel : ViewModelBase
    {
        public SelectedUsersViewModel(IEnumerable<User> items)
        {          
            SelectedUsers = new ObservableCollection<User>(items);
        }

        public ObservableCollection<User> SelectedUsers { get; }
    }
}