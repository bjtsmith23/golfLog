using System;
using System.Windows.Forms;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class EnterScoresForm : Form
    {
        private TextBox txtRoundId = new TextBox();
        private TextBox txtPlayerId = new TextBox();
        private Button btnSave = new Button();
        private NumericUpDown[] holeInputs = new NumericUpDown[18];

        public EnterScoresForm()
        {
            Text = "Enter Scores";
            Width = 350;
            Height = 600;

            txtRoundId.Top = 20; txtRoundId.PlaceholderText = "Round ID";
            txtPlayerId.Top = 60; txtPlayerId.PlaceholderText = "Player ID";

            int y = 100;
            for (int i = 0; i < 18; i++)
            {
                holeInputs[i] = new NumericUpDown { Top = y, Left = 20, Width = 60, Minimum = 1, Maximum = 15 };
                Controls.Add(holeInputs[i]);
                y += 30;
            }

            btnSave.Top = y + 20;
            btnSave.Text = "Save Scores";
            btnSave.Click += SaveScores;

            Controls.Add(txtRoundId);
            Controls.Add(txtPlayerId);
            Controls.Add(btnSave);
        }

        private void SaveScores(object sender, EventArgs e)
        {
            var service = new ScoreService();
            int roundId = int.Parse(txtRoundId.Text);
            int playerId = int.Parse(txtPlayerId.Text);

            for (int i = 0; i < 18; i++)
            {
                service.AddScore(roundId, playerId, i + 1, (int)holeInputs[i].Value);
            }

            MessageBox.Show("Scores saved.");
        }
    }
}
