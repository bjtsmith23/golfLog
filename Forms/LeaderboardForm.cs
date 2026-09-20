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
        private readonly TextBlock annualLeaderText = new TextBlock();

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

            var loadButtons = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };
            var btnLoad = new Button { Content = "Load Year", Width = 120 };
            btnLoad.Click += LoadLeaderboard;
            loadButtons.Children.Add(btnLoad);

            var btnLoadAll = new Button { Content = "All Rounds", Width = 120, Margin = new Thickness(10, 0, 0, 0) };
            btnLoadAll.Click += LoadAllRounds;
            loadButtons.Children.Add(btnLoadAll);
            panel.Children.Add(loadButtons);

            var annualLeaderBox = new Border
            {
                Background = System.Windows.Media.Brushes.LightGray,
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 16, 0, 0)
            };
            annualLeaderText.FontSize = 18;
            annualLeaderText.FontWeight = FontWeights.Bold;
            annualLeaderText.TextAlignment = TextAlignment.Center;
            annualLeaderBox.Child = annualLeaderText;
            panel.Children.Add(annualLeaderBox);

            list.Height = 390;
            list.Margin = new Thickness(0, 12, 0, 0);
            list.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            list.Background = System.Windows.Media.Brushes.Gainsboro;

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
            LoadAllRounds(sender, e);
        }

        private void LoadAllRounds(object? sender, RoutedEventArgs e)
        {
            LoadLeaderboardResults(null);
        }

        private void LoadLeaderboard(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(yearInput.Text, out var year))
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoadLeaderboardResults(year);
        }

        private void LoadLeaderboardResults(int? year)
        {
            var service = new LeaderboardService();
            var results = service.GetYearRoundResults(year).ToList();

            UpdateAnnualLeader(results);

            list.Items.Clear();
            foreach (var r in results)
            {
                var resultText = r.Player1Score == r.Player2Score
                    ? "WASH"
                    : r.Player1Score < r.Player2Score
                        ? $"{r.Player1} wins by {r.StrokeDifference} strokes"
                        : $"{r.Player2} wins by {r.StrokeDifference} strokes";

                list.Items.Add($"Round {r.RoundId} | {r.DatePlayed:yyyy-MM-dd}\n" +
                    $"{r.Player1}: {r.Player1Score} strokes    {r.Player2}: {r.Player2Score} strokes\n" +
                    resultText);
            }

            if (!results.Any())
                list.Items.Add("No rounds found for this year.");
        }

        private void UpdateAnnualLeader(List<(int RoundId, DateTime DatePlayed, string Player1, int Player1Score, string Player2, int Player2Score, int StrokeDifference)> results)
        {
            if (!results.Any())
            {
                annualLeaderText.Text = "No scores for this year";
                return;
            }

            var totals = new Dictionary<string, int>();
            foreach (var result in results)
            {
                totals[result.Player1] = totals.GetValueOrDefault(result.Player1) + result.Player1Score;
                totals[result.Player2] = totals.GetValueOrDefault(result.Player2) + result.Player2Score;
            }

            var standings = totals.OrderBy(pair => pair.Value).ToList();
            if (standings.Count < 2)
            {
                annualLeaderText.Text = "Waiting for both players";
                return;
            }

            var difference = standings[1].Value - standings[0].Value;

            annualLeaderText.Text = difference == 0
                ? "TIE"
                : $"{standings[0].Key} UP {difference}";
        }
    }
}
