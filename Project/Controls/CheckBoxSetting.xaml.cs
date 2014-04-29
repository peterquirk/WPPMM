﻿using Kazyx.WPPMM.DataModel;
using System.Windows;
using System.Windows.Controls;

namespace Kazyx.WPPMM.Controls
{
    public partial class CheckBoxSetting : UserControl
    {
        private SettingType type;

        public enum SettingType
        {
            displayShootbutton,
            postviewImageTransfer,
        };

        public CheckBoxSetting(string title, string guide, SettingType setting)
        {
            InitializeComponent();

            SettingGuide.Text = guide;
            _init(title, setting);
        }

        public CheckBoxSetting(string title, SettingType setting)
        {
            InitializeComponent();
            SettingGuide.Visibility = System.Windows.Visibility.Collapsed;
            _init(title, setting);
        }

        private void _init(string title, SettingType setting)
        {


            SettingName.Text = title;
            type = setting;

            SettingCheckBox.Checked += SettingCheckBox_Checked;
            SettingCheckBox.Unchecked += SettingCheckBox_Unchecked;

            var isChecked = false;
            switch (setting)
            {
                case SettingType.displayShootbutton:
                    isChecked = ApplicationSettings.GetInstance().IsShootButtonDisplayed;
                    break;
                case SettingType.postviewImageTransfer:
                    isChecked = ApplicationSettings.GetInstance().IsPostviewTransferEnabled;
                    break;
            }

            SettingCheckBox.IsChecked = isChecked;
        }

        void SettingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case SettingType.displayShootbutton:
                    ApplicationSettings.GetInstance().IsShootButtonDisplayed = false;
                    break;
                case SettingType.postviewImageTransfer:
                    ApplicationSettings.GetInstance().IsPostviewTransferEnabled = false;
                    break;
            }
        }

        void SettingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case SettingType.displayShootbutton:
                    ApplicationSettings.GetInstance().IsShootButtonDisplayed = true;
                    break;
                case SettingType.postviewImageTransfer:
                    ApplicationSettings.GetInstance().IsPostviewTransferEnabled = true;
                    break;
            }
        }
    }
}