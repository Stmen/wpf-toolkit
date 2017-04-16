﻿using Aronium.Wpf.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _integerProperty;
        private decimal _decimalProperty;
        private string _themeName = "Dark";
        private List<string> _themes;
        private bool _showTabControlBorder;
        private string _selectedTag;
        List<User> _users;

        public MainWindow()
        {
            InitializeComponent();

            Themes = new List<string>(new[] { "Light", "Dark" });

            DataContext = this;

            IntegerProperty = new Random().Next(0, 1000);

            closableTabControl.ItemClosing += OnClosableTabControlItemClosing;

            Tags = new ObservableCollection<string>(new[] { "New York", "Los Angeles", "Seattle", "San Francisco", "Belgrade" });

            CreateGuidedTour();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ThemeName
        {
            get { return _themeName; }
            set
            {
                if (_themeName != value)
                {
                    var currentResourceIndex = GetResourceIndex();

                    _themeName = value;

                    OnPropertyChanged("ThemeName");

                    ChangeTheme(currentResourceIndex);
                }
            }
        }

        private int GetResourceIndex()
        {
            string source = string.Format("pack://application:,,,/Aronium.Wpf.Toolkit;component/Themes/Brushes/{0}.xaml", this.ThemeName);

            var existing = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.Equals(source));

            if (existing != null)
                return Application.Current.Resources.MergedDictionaries.IndexOf(existing);
            else return -1;
        }

        private void ChangeTheme(int dictionaryIndex)
        {
            if (dictionaryIndex >= 0)
            {
                this.Cursor = Cursors.Wait;
                string source = string.Format("pack://application:,,,/Aronium.Wpf.Toolkit;component/Themes/Brushes/{0}.xaml", this.ThemeName);

                Application.Current.Resources.MergedDictionaries.RemoveAt(dictionaryIndex);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(source, UriKind.RelativeOrAbsolute) });

                this.Cursor = Cursors.Arrow;
            }
        }

        public List<string> Themes
        {
            get { return _themes; }
            set
            {
                this._themes = value;
                OnPropertyChanged("Themes");
            }
        }

        public int IntegerProperty
        {
            get { return _integerProperty; }
            set { _integerProperty = value; OnPropertyChanged("IntegerProperty"); }
        }

        public IEnumerable<Dock> TabPlacementValues
        {
            get
            {
                return Enum.GetValues(typeof(Dock)).Cast<Dock>();
            }
        }

        public bool ShowTabControlBorder
        {
            get { return _showTabControlBorder; }
            set
            {
                _showTabControlBorder = value;

                tabControl.BorderThickness = new Thickness(value ? 1 : 0);
                tabControl.Padding = new Thickness(value ? 5 : 0);

                OnPropertyChanged("ShowTabControlBorder");
            }
        }

        public List<User> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new List<User>();

                    for (int i = 1; i <= 20; i++)
                    {
                        _users.Add(new User()
                        {
                            FirstName = "User",
                            LastName = "User " + i,
                            DateOfBirth = DateTime.Today.Date,
                            Email = string.Format("user.{0}@email.com", i)
                        });
                    }
                }

                return _users;
            }
        }

        public decimal DecimalProperty
        {
            get
            {
                return _decimalProperty;
            }
            set
            {
                _decimalProperty = value; OnPropertyChanged("DecimalProperty");
            }
        }

        public ObservableCollection<string> Tags { get; private set; }

        public string SelectedTag
        {
            get
            {
                return _selectedTag;
            }
            set
            {
                _selectedTag = value;

                OnPropertyChanged("SelectedTag");
            }
        }

        private void OnCalendarPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void OnAddTabItem(object sender, RoutedEventArgs e)
        {
            var item = new ClosableTabItem() { Header = "Added item" };
            closableTabControl.Items.Add(item);
        }

        private void OnClosableTabControlItemClosing(object sender, ClosableItemEventArgs e)
        {
            e.Cancel = MessageBox.Show($"Close item {e.Item.Header.ToString()}?", "Confirm close", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
        }

        private void ShowFlyout(object sender, RoutedEventArgs e)
        {
            flyout.IsOpen = true;
        }

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1);
        }

        #region - Guided Tour -

        private void CreateGuidedTour()
        {
            guide.Items = new[]
            {
                new GuidedTourItem() {Target = tabItemGuidedTour, Content = "Click to see guided tour in action", Placement = GuidedTourItem.ItemPlacement.Right, Title = "Start guided tour" },
                new GuidedTourItem() {Target = guideElement1, Content = "Text for guide element 1", Placement = GuidedTourItem.ItemPlacement.Bottom , Title = "Click tour items"},
                new GuidedTourItem() {Target = guideElement2, Content = "Text for guide element 2", Placement = GuidedTourItem.ItemPlacement.Left},
                new GuidedTourItem() {Target = guideElement3, Content = "Text for guide element 3", Placement = GuidedTourItem.ItemPlacement.Right },
                new GuidedTourItem() {Target = guideElement4, Content = "Text for guide element 4", Placement = GuidedTourItem.ItemPlacement.Top}
            };
        }

        private void OnResetGuide(object sender, RoutedEventArgs e)
        {
            guide.Reset();
        }

        private void OnGuidedTourClosed(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Guided tour closed!");
        }

        private void OnGuidedTourFinished(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Guided tour finished!");
        } 

        #endregion
    }
}
