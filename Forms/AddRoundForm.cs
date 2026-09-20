using System;
using System.Windows;
using System.Windows.Controls;
using GolfTracker.Services;

namespace GolfTracker.Forms
{
    public class AddRoundForm : Window
    {
        private readonly DatePicker datePicker = new DatePicker();
        private readonly TextBox txtP1 = new TextBox();
        private readonly TextBox txtP2 = new TextBox();

        public AddRoundForm()
        {
            Title = "Add Round";
            Width = 300;
            Height = 260;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new Label { Content = "Date Played" });
            datePicker.SelectedDate = DateTime.Today;
            stack.Children.Add(datePicker);

            stack.Children.Add(new Label { Content = "Player 1 ID", Margin = new Thickness(0, 12, 0, 0) });
            txtP1.Width = 200;
            stack.Children.Add(txtP1);

            stack.Children.Add(new Label { Content = "Player 2 ID", Margin = new Thickness(0, 12, 0, 0) });
            txtP2.Width = 200;
            stack.Children.Add(txtP2);

            var btnSave = new Button { Content = "Save", Width = 120, Margin = new Thickness(0, 20, 0, 0) };
            btnSave.Click += SaveRound;
            stack.Children.Add(btnSave);

            Content = stack;
        }

        private void SaveRound(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtP1.Text, out var p1) || !int.TryParse(txtP2.Text, out var p2))
            {
                MessageBox.Show("Please enter valid player IDs.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var service = new RoundService();
            var roundId = service.CreateRound(datePicker.SelectedDate ?? DateTime.Today, p1, p2);
            MessageBox.Show($"Round saved. ID: {roundId}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
