using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using SubscriptionDashboard.Services;
using SubscriptionDashboard.Views;

namespace SubscriptionDashboard.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SourceCache<ValidatorViewModel, string> _sourceCache = new(x => x.Key);
    private readonly BehaviorSubject<string> _searchTerm = new(string.Empty);
    private readonly Subject<string> _searchTermSubject = new();
    private ReadOnlyObservableCollection<ValidatorViewModel> _validatorViewModels;

    public MainWindowViewModel()
    {
        if (DataFetcher.Instance == null)
        {
            throw new Exception("DataFetcher.Instance is null");
        }
        DataFetcher.Instance.DataUpdated += _onUpdated;
        _sourceCache.Connect()
            .Filter(_searchTerm.Select(term =>
                {
                    if (string.IsNullOrEmpty(term))
                    {
                        return x => true;
                    }
                    return new Func<ValidatorViewModel, bool>(vm =>
                        vm.Name.ToLower().Contains(term.ToLower()));
                }).ObserveOn(ThreadPoolScheduler.Instance))
            .Sort(
                SortExpressionComparer<ValidatorViewModel>
                    .Descending(x => x.UnpaidInvoicesCount))
            .ObserveOn(Scheduler.CurrentThread)
            .Bind(out _validatorViewModels)
            .Subscribe();
        _searchTermSubject
            .Throttle(TimeSpan.FromMilliseconds(200))
            .DistinctUntilChanged()
            .Subscribe(value => _searchTerm.OnNext(value));
    }

    public ReadOnlyObservableCollection<ValidatorViewModel> Validators => _validatorViewModels;
    
    public IEnumerable<EpochPercentageViewModel> EpochPercentageViewModels { get; private set; } = [];

    public ICommand UpdateData => new UpdateDataCommand();

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            _searchTermSubject.OnNext(value);  
        } 
    }

    private void _onUpdated()
    {
        Dispatcher.UIThread.Post(() =>
        {
            foreach (var validator in Validators)
            {
                validator.Selected -= VmOnSelected;
            }
            if (DataFetcher.Instance?.Validators == null)
                return;
            _sourceCache.Edit(updater =>
            {
                updater.Clear();
                updater.AddOrUpdate( DataFetcher.Instance.Validators
                    .Select(v =>
                    {
                        var vm = new ValidatorViewModel(v);
                        vm.Selected += VmOnSelected;
                        return vm;
                    })
                    .OrderByDescending(v => v.UnpaidInvoicesCount));
            });
            OnPropertyChanged(nameof(Validators));
            EpochPercentageViewModels = DataFetcher.Instance.EpochPercentages
                .Select(ep => new EpochPercentageViewModel(ep.Key, ep.Value))
                .OrderByDescending(ep => ep.Epoch);
            OnPropertyChanged(nameof(EpochPercentageViewModels));
        });
    }
    
    public ValidatorViewModel SelectedValidator { get; private set; }

    [RelayCommand]
    public void ShowSettingsDialog()
    {
        DialogService.Instance?.ShowDialog(() => new SettingsDialog());
    }

    private void VmOnSelected(object? sender, EventArgs e)
    {
        if (sender is ValidatorViewModel vm)
        {
            SelectedValidator = vm;
            OnPropertyChanged(nameof(SelectedValidator));
        }
    }
}

internal class UpdateDataCommand : ICommand, IDisposable
{

    private readonly System.Timers.Timer _timer;
    public static DateTime CanUpdate;

    public UpdateDataCommand()
    {
        CanUpdate = DateTime.Now.AddSeconds(30);
        _timer = new System.Timers.Timer(TimeSpan.FromSeconds(5));
        _timer.Elapsed += async (sender, args) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        };
        _timer.Start();
    }
    public bool CanExecute(object? parameter)
    {
        if (DateTime.Now < CanUpdate)
        {
            return false;
        }
        return true;
    }

    public void Execute(object? parameter)
    {
        CanUpdate = DateTime.Now.AddSeconds(30);
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        DataFetcher.Instance?.RefreshData();
    }

    public event EventHandler? CanExecuteChanged;
    
    public void Dispose()
    {
        _timer.Dispose();
    }
}

public class IsPaidConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (double)value == 0 ? "Paid" : "Unpaid";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsPaidBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (double)value == 0 ? Brushes.Green : Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class NotNullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return true;
        if (value is IEnumerable enumerable)
        {
            return !enumerable.Cast<object>().Any();
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class NotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return false;
        if (value is IEnumerable enumerable)
        {
            return enumerable.Cast<object>().Any();
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
