using System;
using System.Windows.Input;

namespace SubscriptionDashboard.ViewModels;

public class RelayCommand(Action execute) : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        execute();
    }

    public event EventHandler? CanExecuteChanged;
}

public class RelayCommand<T>(Action<T> execute) : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T param)
            execute(param);
    }

    public event EventHandler? CanExecuteChanged;
}
