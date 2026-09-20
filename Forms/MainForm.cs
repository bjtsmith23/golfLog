using System.Windows;
using System.Windows.Controls;

namespace GolfTracker.Forms
{
    public class MainForm : Window
    {
        public MainForm()
        {
            Title = "Golf Tracker";
            Width = 700;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var btnAddRound = new Button { Content = "Add Round", Width = 150, Height = 40, Margin = new Thickness(0, 0, 0, 10) };
            var btnEnterScores = new Button { Content = "Enter Scores", Width = 150, Height = 40, Margin = new Thickness(0, 0, 0, 10) };
            var btnLeaderboard = new Button { Content = "Leaderboard", Width = 150, Height = 40 };

            btnAddRound.Click += (_, _) => new AddRoundForm().ShowDialog();
            btnEnterScores.Click += (_, _) => new EnterScoresForm().ShowDialog();
            btnLeaderboard.Click += (_, _) => new LeaderboardForm().ShowDialog();

            var stack = new StackPanel { Margin = new Thickness(20) };
            stack.Children.Add(btnAddRound);
            stack.Children.Add(btnEnterScores);
            stack.Children.Add(btnLeaderboard);

            Content = stack;
        }
    }
}
