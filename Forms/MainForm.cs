using System;
using System.Windows.Forms;

namespace GolfTracker.Forms
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Golf Tracker";
            Width = 400;
            Height = 300;

            var btnAddRound = new Button { Text = "Add Round", Top = 30, Left = 30, Width = 150 };
            var btnEnterScores = new Button { Text = "Enter Scores", Top = 80, Left = 30, Width = 150 };
            var btnLeaderboard = new Button { Text = "Leaderboard", Top = 130, Left = 30, Width = 150 };

            btnAddRound.Click += (s, e) => new AddRoundForm().ShowDialog();
            btnEnterScores.Click += (s, e) => new EnterScoresForm().ShowDialog();
            btnLeaderboard.Click += (s, e) => new LeaderboardForm().ShowDialog();

            Controls.Add(btnAddRound);
            Controls.Add(btnEnterScores);
            Controls.Add(btnLeaderboard);
        }
    }
}
