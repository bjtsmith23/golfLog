using System;
using System.Windows;
using System.Windows.Controls;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class LeaderboardForm : Window
    {
        private readonly TextBox yearInput = new TextBox();
        private readonly ListBox list = new ListBox();

        public LeaderboardForm()
        {
            Title = "Leaderboard";
            Width = 330;
            Height = 430;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new StackPanel { Margin = new Thickness(20) };

            panel.Children.Add(new Label { Content = "Year" });
            yearInput.Text = DateTime.Now.Year.ToString();
            yearInput.Width = 120;
            panel.Children.Add(yearInput);

            var btnLoad = new Button { Content = "Load", Width = 120, Margin = new Thickness(0, 12, 0, 0) };
            btnLoad.Click += LoadLeaderboard;
            panel.Children.Add(btnLoad);

            list.Height = 250;
            list.Margin = new Thickness(0, 12, 0, 0);
            panel.Children.Add(list);

            Content = panel;
        }

        private void LoadLeaderboard(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(yearInput.Text, out var year))
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var service = new LeaderboardService();
            var results = service.GetYearTotals(year);

            list.Items.Clear();
            foreach (var r in results)
                list.Items.Add($"{r.Player}: {r.TotalStrokes} strokes");
        }
    }
}
