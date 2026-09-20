using System;
using System.Windows.Forms;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class AddRoundForm : Form
    {
        private DateTimePicker datePicker = new DateTimePicker();
        private TextBox txtP1 = new TextBox();
        private TextBox txtP2 = new TextBox();
        private Button btnSave = new Button();

        public AddRoundForm()
        {
            Text = "Add Round";
            Width = 300;
            Height = 250;

            datePicker.Top = 20;
            txtP1.Top = 60; txtP1.PlaceholderText = "Player 1 ID";
            txtP2.Top = 100; txtP2.PlaceholderText = "Player 2 ID";
            btnSave.Top = 150; btnSave.Text = "Save";

            btnSave.Click += SaveRound;

            Controls.Add(datePicker);
            Controls.Add(txtP1);
            Controls.Add(txtP2);
            Controls.Add(btnSave);
        }

        private void SaveRound(object sender, EventArgs e)
        {
            var service = new RoundService();
            service.CreateRound(datePicker.Value, int.Parse(txtP1.Text), int.Parse(txtP2.Text));
            MessageBox.Show("Round saved.");
        }
    }
}
