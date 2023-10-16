using ChatApp.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void ButtonWindowState_Click(object sender, RoutedEventArgs e)
        {
            if(Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            usernamebox.IsEnabled = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_CommandCenter(object sender, RoutedEventArgs e)
        {
            Contacts.SelectedItem = null;
        }

        private void Contacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Contacts.SelectedItem == null)
            {
                CallImage.Visibility = Visibility.Hidden;
                CameraImage.Visibility = Visibility.Hidden;
            }
            else
            {
                CallImage.Visibility = Visibility.Visible;
                CameraImage.Visibility = Visibility.Visible;
            }
        }
    }
}
