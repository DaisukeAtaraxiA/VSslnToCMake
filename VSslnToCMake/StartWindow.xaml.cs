/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VSslnToCMake
{
    /// <summary>
    /// StartWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartWindow : System.Windows.Window
    {
        public string[] targetPlatforms { get; set; }
        public string[] targetConfigurations { get; set; }

        private class ConfigurationItem : INotifyPropertyChanged
        {
            private bool isChecked;
            public bool IsChecked
            {
                get
                {
                    return isChecked;
                }
                set
                {
                    isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
            public string Label { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string name)
            {
                var h = PropertyChanged;
                if (h == null)
                {
                    return;
                }
                h(this, new PropertyChangedEventArgs(name));
            }
        }

        public StartWindow()
        {
            InitializeComponent();
        }

        new public bool? ShowDialog()
        {
            this.platformComboBox.ItemsSource = targetPlatforms;
            this.platformComboBox.SelectedItem = "x64";

            var items = targetConfigurations.Select(
                x =>
                {
                    var item = new ConfigurationItem() { IsChecked = true, Label = x };
                    item.PropertyChanged += PersonPropertyChanged;
                    return item;
                }
            ).ToArray();
            this.configurationsListBox.ItemsSource = items;

            return base.ShowDialog();
        }

        public string SelectedPlatform()
        {
            return platformComboBox.SelectedValue.ToString();
        }

        public string[] SelectedConfigurations()
        {
            var items = new List<string>();
            foreach (ConfigurationItem item in configurationsListBox.Items)
            {
                if (item.IsChecked)
                {
                    items.Add(item.Label);
                }
            }
            return items.ToArray();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void PersonPropertyChanged(object sender,
                                           PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                bool anyChecked = false;
                foreach (ConfigurationItem item in configurationsListBox.Items)
                {
                    if (item.IsChecked)
                    {
                        anyChecked = true;
                        break;
                    }
                }
                this.okButton.IsEnabled = anyChecked;
            }
        }
    }
}
