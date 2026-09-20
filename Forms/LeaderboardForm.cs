using System;
using System.Linq;
using System.Windows.Forms;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class LeaderboardForm : Form
    {
        private NumericUpDown yearInput = new NumericUpDown();
        private ListBox list = new ListBox();

        public LeaderboardForm()
        {
            Text = "Leaderboard";
            Width = 300;
            Height = 400;

            yearInput.Top = 20;
            yearInput.Minimum = 2000;
            yearInput.Maximum = 2100;
            yearInput.Value = DateTime.Now.Year;

            var btnLoad = new Button { Text = "Load", Top = 60, Left = 20 };
            btnLoad.Click += LoadLeaderboard;

            list.Top = 100;
            list.Width = 250;
            list.Height = 250;

            Controls.Add(yearInput);
            Controls.Add(btnLoad);
            Controls.Add(list);
        }

        private void LoadLeaderboard(object sender, EventArgs e)
        {
            var service = new LeaderboardService();
            var results = service.GetYearTotals((int)yearInput.Value);

            list.Items.Clear();
            foreach (var r in results)
                list.Items.Add($"{r.Player}: {r.TotalStrokes} strokes");
        }
    }
}
