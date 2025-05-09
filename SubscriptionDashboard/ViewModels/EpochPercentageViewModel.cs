using System.Collections.Generic;

namespace SubscriptionDashboard.ViewModels;

public class EpochPercentageViewModel(int epoch, float percentage)
{
    public int Epoch { get; } = epoch;
    public string Percentage => $"{percentage * 100:0.00} %";
}