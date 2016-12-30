using System;
using System.Windows;
using Doobry.Features.QueryDeveloper;
using Doobry.Settings;
using Dragablz;

namespace Doobry.Infrastructure
{
    //TODO delete
    public interface ITabInstanceManager
    {
        TabViewModel CreateManagedTabViewModel();

        TabViewModel CreateManagedTabViewModel(Guid id, Connection connection);        

        ItemActionCallback ClosingTabItemCallback { get; }

        void Manage(Window window);
    }
}