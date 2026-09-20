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
            Width = 540;
            Height = 560;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new StackPanel { Margin = new Thickness(20) };

            panel.Children.Add(new Label { Content = "Year" });
            yearInput.Text = DateTime.Now.Year.ToString();
            yearInput.Width = 120;
            panel.Children.Add(yearInput);

            var btnLoad = new Button { Content = "Load", Width = 120, Margin = new Thickness(0, 12, 0, 0) };
            btnLoad.Click += LoadLeaderboard;
            panel.Children.Add(btnLoad);

            list.Height = 390;
            list.Margin = new Thickness(0, 12, 0, 0);
            list.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            var itemStyle = new Style(typeof(ListBoxItem));
            itemStyle.Setters.Add(new Setter(Control.BorderBrushProperty, System.Windows.Media.Brushes.LightGray));
            itemStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
            itemStyle.Setters.Add(new Setter(Control.MarginProperty, new Thickness(0, 0, 0, 8)));
            itemStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(10)));
            list.ItemContainerStyle = itemStyle;

            panel.Children.Add(list);

            Content = panel;
            Loaded += LoadCurrentYear;
        }

        private void LoadCurrentYear(object? sender, RoutedEventArgs e)
        {
            LoadLeaderboard(sender, e);
        }

        private void LoadLeaderboard(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(yearInput.Text, out var year))
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var service = new LeaderboardService();
            var results = service.GetYearRoundResults(year);

            list.Items.Clear();
            foreach (var r in results)
            {
                var resultText = r.Player1Score == r.Player2Score
                    ? "Tie"
                    : r.Player1Score < r.Player2Score
                        ? $"{r.Player1} beat {r.Player2} by {r.StrokeDifference} strokes"
                        : $"{r.Player2} beat {r.Player1} by {r.StrokeDifference} strokes";

                list.Items.Add($"Round {r.RoundId} | {r.DatePlayed:yyyy-MM-dd}\n" +
                    $"{r.Player1}: {r.Player1Score} strokes    {r.Player2}: {r.Player2Score} strokes\n" +
                    resultText);
            }

            if (!results.Any())
                list.Items.Add("No rounds found for this year.");
        }
    }
}
