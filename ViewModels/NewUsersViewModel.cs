using System.Collections.Generic;
using System.Collections.ObjectModel;
using eventula_entrance_client.Models;

namespace eventula_entrance_client.ViewModels
{
    public class NewUsersViewModel : ViewModelBase
    {
        public NewUsersViewModel(IEnumerable<User> items)
        {          
            NewUsers = new ObservableCollection<User>(items);
        }

        public ObservableCollection<User> NewUsers { get; }
    }
}