using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _27
{
    public partial class MainWindow : Window
    {
        private class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string AvatarUrl { get; set; }
        }

        private List<User> users = new List<User>();
        private User currentUser;

        public MainWindow()
        {
            InitializeComponent();
            ApplyTheme("Light");
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsername.Text.Trim();
            string password = RegisterPassword.Password;
            string confirmPassword = ConfirmPassword.Password;
            string avatarUrl = AvatarUrl.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(avatarUrl))
            {
                RegisterError.Text = "All fields are required!";
                return;
            }

            if (password != confirmPassword)
            {
                RegisterError.Text = "Passwords do not match!";
                return;
            }

            if (users.Exists(u => u.Username == username))
            {
                RegisterError.Text = "Username already exists!";
                return;
            }

            users.Add(new User
            {
                Username = username,
                Password = password,
                AvatarUrl = avatarUrl
            });

            RegisterError.Text = "Registration successful! Please login.";
            RegisterUsername.Text = "";
            RegisterPassword.Password = "";
            ConfirmPassword.Password = "";
            AvatarUrl.Text = "";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsername.Text.Trim();
            string password = LoginPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LoginError.Text = "All fields are required!";
                return;
            }

            currentUser = users.Find(u => u.Username == username && u.Password == password);
            if (currentUser == null)
            {
                LoginError.Text = "Invalid username or password!";
                return;
            }

            AuthPanel.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;

            ProfileUsername.Text = $"Welcome, {currentUser.Username}!";
            try
            {
                UserAvatar.Source = new BitmapImage(new Uri(currentUser.AvatarUrl));
            }
            catch
            {
                UserAvatar.Source = null;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthPanel.Visibility = Visibility.Visible;
            MainContent.Visibility = Visibility.Collapsed;
            LoginUsername.Text = "";
            LoginPassword.Password = "";
            LoginError.Text = "";
            currentUser = null;
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string theme = selectedItem.Content.ToString();
                ApplyTheme(theme == "Light Theme" ? "Light" : "Dark");
            }
        }

        private void ApplyTheme(string theme)
        {
            ResourceDictionary themeDict = new ResourceDictionary();

            if (theme == "Light")
            {
                themeDict.Add("BackgroundColor", new SolidColorBrush(Color.FromRgb(245, 245, 245)));
                themeDict.Add("ForegroundColor", new SolidColorBrush(Colors.Black));
                themeDict.Add("ButtonBackground", new SolidColorBrush(Color.FromRgb(76, 175, 80)));
                themeDict.Add("ButtonForeground", new SolidColorBrush(Colors.White));
                themeDict.Add("BorderColor", new SolidColorBrush(Colors.Gray));
            }
            else
            {
                themeDict.Add("BackgroundColor", new SolidColorBrush(Color.FromRgb(33, 33, 33)));
                themeDict.Add("ForegroundColor", new SolidColorBrush(Colors.White));
                themeDict.Add("ButtonBackground", new SolidColorBrush(Color.FromRgb(100, 100, 100)));
                themeDict.Add("ButtonForeground", new SolidColorBrush(Colors.White));
                themeDict.Add("BorderColor", new SolidColorBrush(Color.FromRgb(150, 150, 150)));
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDict);

            Application.Current.Resources["ButtonStyle"] = new Style(typeof(Button))
            {
                Setters = {
                    new Setter(Button.BackgroundProperty, new DynamicResourceExtension("ButtonBackground")),
                    new Setter(Button.ForegroundProperty, new DynamicResourceExtension("ButtonForeground")),
                    new Setter(Button.PaddingProperty, new Thickness(10, 5, 10, 5)),
                    new Setter(Button.MarginProperty, new Thickness(5)),
                    new Setter(Button.BorderThicknessProperty, new Thickness(0))
                }
            };

            Application.Current.Resources["TextBoxStyle"] = new Style(typeof(TextBox))
            {
                Setters = {
                    new Setter(TextBox.BackgroundProperty, new DynamicResourceExtension("BackgroundColor")),
                    new Setter(TextBox.ForegroundProperty, new DynamicResourceExtension("ForegroundColor")),
                    new Setter(TextBox.PaddingProperty, new Thickness(5)),
                    new Setter(TextBox.MarginProperty, new Thickness(5)),
                    new Setter(TextBox.BorderBrushProperty, new DynamicResourceExtension("BorderColor"))
                }
            };

            Application.Current.Resources["PasswordBoxStyle"] = new Style(typeof(PasswordBox))
            {
                Setters = {
                    new Setter(PasswordBox.BackgroundProperty, new DynamicResourceExtension("BackgroundColor")),
                    new Setter(PasswordBox.ForegroundProperty, new DynamicResourceExtension("ForegroundColor")),
                    new Setter(PasswordBox.PaddingProperty, new Thickness(5)),
                    new Setter(PasswordBox.MarginProperty, new Thickness(5)),
                    new Setter(PasswordBox.BorderBrushProperty, new DynamicResourceExtension("BorderColor"))
                }
            };

            Application.Current.Resources["TabControlStyle"] = new Style(typeof(TabControl))
            {
                Setters = {
                    new Setter(TabControl.BackgroundProperty, new DynamicResourceExtension("BackgroundColor")),
                    new Setter(TabControl.ForegroundProperty, new DynamicResourceExtension("ForegroundColor")),
                    new Setter(TabControl.BorderBrushProperty, new DynamicResourceExtension("BorderColor"))
                }
            };

            RefreshUI();
        }

        private void RefreshUI()
        {
            Dispatcher.Invoke(() =>
            {
                UpdateLayout();
                InvalidateVisual();
                foreach (Window window in Application.Current.Windows)
                {
                    window.InvalidateVisual();
                }
            });
        }
    }
}