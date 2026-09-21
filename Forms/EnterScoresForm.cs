using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class EnterScoresForm : Window
    {
        private readonly TextBox txtRoundId = new TextBox();
        private readonly TextBlock playerNameLabel = new TextBlock();
        private readonly TextBlock totalScoreText = new TextBlock();
        private readonly TextBlock scoreStatusText = new TextBlock();
        private readonly TextBlock frontNineTotalText = new TextBlock();
        private readonly TextBlock backNineTotalText = new TextBlock();
        private readonly TextBox[] holeInputs = new TextBox[18];
        private readonly int[] yardages = { 439, 370, 426, 224, 544, 170, 420, 435, 397, 320, 148, 325, 465, 360, 460, 190, 337, 417 };
        private readonly int[] holePar = { 4, 4, 4, 3, 5, 3, 4, 4, 4, 4, 3, 4, 5, 4, 5, 3, 4, 4 };
        private readonly int[] strokeIndex = { 1, 3, 7, 15, 11, 5, 17, 13, 9, 8, 12, 10, 18, 14, 2, 4, 16, 6 };
        private readonly string playerName;
        private readonly int roundId;

        public EnterScoresForm() : this("Phil", GetRoundIdForEntry())
        {
        }

        private static int GetRoundIdForEntry()
        {
            var roundService = new RoundService();
            return roundService.GetNextRoundId();
        }

        public EnterScoresForm(string playerName, int roundId)
        {
            this.playerName = playerName;
            this.roundId = roundId;

            Title = "Enter Scores";
            Width = 1120;
            Height = 760;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var root = new StackPanel { Margin = new Thickness(26) };

            var topRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 20) };

            playerNameLabel.Text = $"Player: {playerName}";
            playerNameLabel.FontSize = 24;
            playerNameLabel.FontWeight = FontWeights.Bold;
            playerNameLabel.VerticalAlignment = VerticalAlignment.Center;
            playerNameLabel.Margin = new Thickness(0, 0, 10, 0);
            topRow.Children.Add(playerNameLabel);

            topRow.Children.Add(new TextBlock { Text = "Round ID:", FontSize = 16, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(20, 0, 8, 0) });
            txtRoundId.Width = 120;
            txtRoundId.Height = 28;
            txtRoundId.Text = roundId.ToString();
            topRow.Children.Add(txtRoundId);

            root.Children.Add(topRow);

            root.Children.Add(CreateScorecardSection("Front 9", 0, "OUT"));
            root.Children.Add(CreateScorecardSection("Back 9", 9, "IN"));

            var summaryRow = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
            totalScoreText.Text = "Total: 0";
            totalScoreText.FontSize = 20;
            totalScoreText.FontWeight = FontWeights.Bold;
            totalScoreText.Margin = new Thickness(0, 0, 16, 0);

            scoreStatusText.Text = "Par: E";
            scoreStatusText.FontSize = 18;
            scoreStatusText.Margin = new Thickness(0, 2, 0, 0);
            summaryRow.Children.Add(totalScoreText);
            summaryRow.Children.Add(scoreStatusText);
            root.Children.Add(summaryRow);

            var btnSave = new Button
            {
                Content = "Save Scores",
                Width = 160,
                Height = 42,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            btnSave.Click += SaveScores;
            root.Children.Add(btnSave);

            Content = root;
            UpdateScoreSummary();
            Loaded += FocusFirstHole;
        }

        private void FocusFirstHole(object? sender, RoutedEventArgs e)
        {
            holeInputs[0].Focus();
            Keyboard.Focus(holeInputs[0]);
        }

        private Grid CreateScorecardSection(string title, int startHole, string totalLabel)
        {
            var section = new StackPanel { Margin = new Thickness(0, 8, 0, 16) };
            section.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(52) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });

            var columnCount = 11;
            for (var column = 0; column < columnCount; column++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = column == 0 ? new GridLength(145) : new GridLength(1, GridUnitType.Star) });

            AddCell(grid, "HOLE", 0, 0, true);
            AddCell(grid, totalLabel, 0, 10, true);
            AddCell(grid, "YARDS", 1, 0, false);
            AddCell(grid, "PAR", 2, 0, false);
            AddCell(grid, "STROKE\nINDEX", 3, 0, false);
            AddCell(grid, "SCORE", 4, 0, false);
            AddCell(grid, "ADJ. SCORE", 5, 0, false);

            for (var offset = 0; offset < 9; offset++)
            {
                var hole = startHole + offset;
                var column = offset + 1;
                AddCell(grid, (hole + 1).ToString(), 0, column, true);
                AddCell(grid, yardages[hole].ToString(), 1, column, false);
                AddCell(grid, holePar[hole].ToString(), 2, column, false);
                AddCell(grid, strokeIndex[hole].ToString(), 3, column, false);

                var input = new TextBox
                {
                    Height = 34,
                    Margin = new Thickness(5),
                    TextAlignment = TextAlignment.Center,
                    FontSize = 16,
                    Background = Brushes.White,
                    BorderBrush = Brushes.Silver
                };
                input.TextChanged += (_, _) => UpdateScoreSummary();
                holeInputs[hole] = input;
                Grid.SetRow(input, 4);
                Grid.SetColumn(input, column);
                grid.Children.Add(input);
            }

            var nineTotal = startHole == 0 ? frontNineTotalText : backNineTotalText;
            nineTotal.Text = "0";
            nineTotal.FontSize = 16;
            nineTotal.HorizontalAlignment = HorizontalAlignment.Center;
            nineTotal.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(nineTotal, 4);
            Grid.SetColumn(nineTotal, 10);
            grid.Children.Add(nineTotal);

            AddCell(grid, totalLabel == "OUT" ? "3425" : "3022", 1, 10, false);
            AddCell(grid, holePar.Skip(startHole).Take(9).Sum().ToString(), 2, 10, false);
            AddCell(grid, "", 3, 10, false);
            AddCell(grid, "0", 5, 10, false);

            section.Children.Add(grid);
            return new Grid { Children = { section } };
        }

        private static void AddCell(Grid grid, string text, int row, int column, bool header)
        {
            var cell = new Border
            {
                Background = header ? Brushes.DarkBlue : Brushes.WhiteSmoke,
                BorderBrush = Brushes.Gainsboro,
                BorderThickness = new Thickness(0.5),
                Child = new TextBlock
                {
                    Text = text,
                    FontSize = header ? 15 : 13,
                    FontWeight = header ? FontWeights.Bold : FontWeights.Normal,
                    Foreground = header ? Brushes.White : Brushes.DarkBlue,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            Grid.SetRow(cell, row);
            Grid.SetColumn(cell, column);
            grid.Children.Add(cell);
        }

        private void UpdateScoreSummary()
        {
            var total = 0;
            var frontNineTotal = 0;
            var backNineTotal = 0;

            for (var i = 0; i < holeInputs.Length; i++)
            {
                if (int.TryParse(holeInputs[i].Text, out var score))
                {
                    total += score;
                    if (i < 9)
                        frontNineTotal += score;
                    else
                        backNineTotal += score;
                }
            }

            frontNineTotalText.Text = frontNineTotal.ToString();
            backNineTotalText.Text = backNineTotal.ToString();

            const int par = 71;
            totalScoreText.Text = $"Total: {total}";

            var relativeToPar = total - par;
            if (relativeToPar == 0)
            {
                scoreStatusText.Text = "Par: E";
                scoreStatusText.Foreground = Brushes.DarkSlateGray;
            }
            else if (relativeToPar > 0)
            {
                scoreStatusText.Text = $"Par: +{relativeToPar}";
                scoreStatusText.Foreground = Brushes.DarkRed;
            }
            else
            {
                scoreStatusText.Text = $"Par: {relativeToPar}";
                scoreStatusText.Foreground = Brushes.DarkGreen;
            }
        }

        private void SaveScores(object sender, RoutedEventArgs e)
        {
            var service = new ScoreService();
            var playerId = playerName == "Phil" ? 1 : 2;

            for (int i = 0; i < 18; i++)
            {
                if (!int.TryParse(holeInputs[i].Text, out var score))
                {
                    MessageBox.Show($"Hole {i + 1} score must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                service.AddScore(roundId, playerId, i + 1, score);
            }

            if (playerName == "Phil")
            {
                var brianForm = new EnterScoresForm("Brian", roundId);
                brianForm.Owner = Owner;
                brianForm.ShowDialog();
                Close();
                return;
            }

            MessageBox.Show("Scores saved for Phil and Brian.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
