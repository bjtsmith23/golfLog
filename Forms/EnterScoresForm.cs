using System;
using System.Windows;
using System.Windows.Controls;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class EnterScoresForm : Window
    {
        private readonly TextBox txtRoundId = new TextBox();
        private readonly TextBox txtPlayerId = new TextBox();
        private readonly TextBox[] holeInputs = new TextBox[18];

        public EnterScoresForm()
        {
            Title = "Enter Scores";
            Width = 420;
            Height = 620;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new StackPanel { Margin = new Thickness(20) };

            panel.Children.Add(new Label { Content = "Round ID" });
            txtRoundId.Width = 120;
            panel.Children.Add(txtRoundId);

            panel.Children.Add(new Label { Content = "Player ID", Margin = new Thickness(0, 12, 0, 0) });
            txtPlayerId.Width = 120;
            panel.Children.Add(txtPlayerId);

            var inputWrap = new WrapPanel { Margin = new Thickness(0, 12, 0, 0) };
            for (int i = 0; i < 18; i++)
            {
                var label = new Label { Content = $"{i + 1}", Width = 30, VerticalContentAlignment = VerticalAlignment.Center };
                var input = new TextBox { Width = 60, Text = "0", Margin = new Thickness(0, 0, 8, 4) };
                holeInputs[i] = input;
                inputWrap.Children.Add(label);
                inputWrap.Children.Add(input);
            }

            panel.Children.Add(new Label { Content = "Scores by hole", Margin = new Thickness(0, 12, 0, 0) });
            panel.Children.Add(inputWrap);

            var btnSave = new Button { Content = "Save Scores", Width = 140, Margin = new Thickness(0, 20, 0, 0) };
            btnSave.Click += SaveScores;
            panel.Children.Add(btnSave);

            Content = panel;
        }

        private void SaveScores(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtRoundId.Text, out var roundId) || !int.TryParse(txtPlayerId.Text, out var playerId))
            {
                MessageBox.Show("Please enter valid Round ID and Player ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var service = new ScoreService();
            for (int i = 0; i < 18; i++)
            {
                if (!int.TryParse(holeInputs[i].Text, out var score))
                {
                    MessageBox.Show($"Hole {i + 1} score must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                service.AddScore(roundId, playerId, i + 1, score);
            }

            MessageBox.Show("Scores saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
