using System.Windows;
using System.Windows.Controls;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class RoundScoresForm : Window
    {
        private readonly TextBox roundIdInput = new TextBox();
        private readonly ListBox scoreList = new ListBox();

        public RoundScoresForm()
        {
            Title = "Round Scores";
            Width = 420;
            Height = 550;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new StackPanel { Margin = new Thickness(20) };

            var roundRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            roundRow.Children.Add(new Label { Content = "Round ID:", Margin = new Thickness(0, 4, 8, 0) });
            roundIdInput.Width = 100;
            roundIdInput.Height = 28;
            roundRow.Children.Add(roundIdInput);

            var btnLoad = new Button { Content = "Load", Width = 100, Margin = new Thickness(12, 0, 0, 0) };
            btnLoad.Click += LoadScores;
            roundRow.Children.Add(btnLoad);

            panel.Children.Add(roundRow);

            scoreList.Height = 420;
            panel.Children.Add(scoreList);

            Content = panel;

            var latestRoundId = new LeaderboardService().GetLatestRoundId();
            if (latestRoundId > 0)
            {
                roundIdInput.Text = latestRoundId.ToString();
                LoadScores(null, null);
            }
            else
            {
                scoreList.Items.Add("No rounds saved yet.");
            }
        }

        private void LoadScores(object? sender, RoutedEventArgs? e)
        {
            if (!int.TryParse(roundIdInput.Text, out var roundId))
            {
                MessageBox.Show("Please enter a valid round ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var service = new LeaderboardService();
            var results = service.GetRoundScores(roundId).ToList();

            scoreList.Items.Clear();

            if (!results.Any())
            {
                scoreList.Items.Add("No scores found for this round.");
                return;
            }

            var playerTotals = new Dictionary<string, int>();

            foreach (var r in results)
            {
                if (!playerTotals.ContainsKey(r.Player))
                    playerTotals[r.Player] = 0;

                playerTotals[r.Player] += r.Score;
                scoreList.Items.Add($"{r.Player} - Hole {r.HoleNumber}: {r.Score}");
            }

            scoreList.Items.Add("----");
            foreach (var player in playerTotals)
                scoreList.Items.Add($"{player.Key} total: {player.Value}");
        }
    }
}