using System.Windows;
using System.Windows.Controls;

namespace GolfTracker.Forms
{
    public class MainForm : Window
    {
        public MainForm()
        {
            Title = "Golf Tracker";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (screenWidth <= 430 || screenHeight <= 900)
            {
                Width = Math.Min(390, screenWidth - 20);
                Height = Math.Min(844, screenHeight - 40);
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.CanResize;
            }

            var header = new TextBlock
            {
                Text = "Wallingford Invitational",
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            var btnAddRound = new Button { Content = "Add Round", Width = 150, Height = 40, Margin = new Thickness(0, 0, 0, 10) };
            var btnEnterScores = new Button { Content = "Enter Scores", Width = 150, Height = 40, Margin = new Thickness(0, 0, 0, 10) };
            var btnCheckRoundScores = new Button { Content = "Check Round Scores", Width = 180, Height = 40, Margin = new Thickness(0, 0, 0, 10) };
            var btnLeaderboard = new Button { Content = "Leaderboard", Width = 150, Height = 40 };

            btnAddRound.Click += (_, _) => new AddRoundForm().ShowDialog();
            btnEnterScores.Click += (_, _) => new EnterScoresForm().ShowDialog();
            btnCheckRoundScores.Click += (_, _) => new RoundScoresForm().ShowDialog();
            btnLeaderboard.Click += (_, _) => new LeaderboardForm().ShowDialog();

            var stack = new StackPanel { Margin = new Thickness(20) };
            stack.Children.Add(header);
            stack.Children.Add(btnAddRound);
            stack.Children.Add(btnEnterScores);
            stack.Children.Add(btnCheckRoundScores);
            stack.Children.Add(btnLeaderboard);

            Content = stack;
        }
    }
}
