using CommunityToolkit.Mvvm.Input;
using FinanceTrackerApp.Models;

namespace FinanceTrackerApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}