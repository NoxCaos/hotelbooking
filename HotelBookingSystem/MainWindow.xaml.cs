using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelBookingSystem {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public enum MessageType { User, Ophelia }

        private OpheliaInterface ophelia;

        public MainWindow() {
            InitializeComponent();
            this.ophelia = new OpheliaInterface(this);
        }

        public void ConstructMessage(string content, MessageType type) {
            Dispatcher.Invoke(new Action(() => this.WriteToChat(content, type)));
        }

        public void AddData(string subject, string data) {
            Dispatcher.Invoke(new Action(() => this.AddToDataview(subject, data)));
        }

        public void SetInputActive(bool active) {
            Dispatcher.Invoke(new Action(() => {
                this.AskButton.IsEnabled = active;
            }));
        }

        private void WriteToChat(string content, MessageType type) {
            this.InputBlock.Text = "";
            var block = new TextBlock();
            block.Foreground = Brushes.White;
            block.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            block.Margin = new Thickness(7);
            block.Padding = new Thickness(5);
            block.MinWidth = 50;
            block.MinHeight = 20;
            block.MaxWidth = 320;
            block.FontSize = 16;
            block.TextWrapping = TextWrapping.Wrap;
            block.FontFamily = new FontFamily("Segoe UI Semilight");
            block.Text = content;
            if (type == MessageType.Ophelia) {
                block.Background = new SolidColorBrush(Color.FromRgb(63, 142, 255));
                block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            }
            else {
                block.Background = new SolidColorBrush(Color.FromRgb(125, 125, 125));
                block.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            }
            this.DockPanel.Children.Add(block);
            this.Viewer.ScrollToBottom();
        }

        private void AddToDataview(string subject, string data) {
            var block = new TextBlock();
            block.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
            block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            block.Margin = new Thickness(0,0,0,5);
            block.FontSize = 20;
            block.FontFamily = new FontFamily("Segoe UI Semilight");
            block.Text = subject + ": " + data;
            block.MouseUp += new MouseButtonEventHandler(this.DataTextBlock_Click);

            this.DataView.Children.Add(block);
        }

        #region WindowEvents

        private void WindowMouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e) {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        #endregion

        #region UIEvents

        private void AskButton_Click(object sender, RoutedEventArgs e) {
            if (this.InputBlock.Text == String.Empty) return;
            this.ophelia.PushUserInput(this.InputBlock.Text);
            this.ConstructMessage(this.InputBlock.Text, MessageType.User);
        }

        private void InputBlock_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (this.InputBlock.Text == String.Empty) return;
                this.ophelia.PushUserInput(this.InputBlock.Text);
                this.ConstructMessage(this.InputBlock.Text, MessageType.User);
            }
        }

        private void DataTextBlock_Click(object sender, MouseButtonEventArgs e) {
            var block = sender as TextBlock;
            this.ophelia.PushTaskWithWrongData(block.Text.Split(':')[0].Trim());
            this.DataView.Children.Remove(block);
        }

        #endregion
    }
}
