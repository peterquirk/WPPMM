﻿using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using WPPMM.Utils;

namespace WPPMM.DataModel
{
    public class ApplicationSettings : INotifyPropertyChanged
    {
        private static ApplicationSettings sSettings = new ApplicationSettings();

        private ApplicationSettings()
        {
            IsPostviewTransferEnabled = Preference.IsPostviewTransferEnabled();
        }

        public static ApplicationSettings GetInstance()
        {
            return sSettings;
        }

        private bool _IsPostviewTransferEnabled = true;

        public bool IsPostviewTransferEnabled
        {
            set
            {
                if (_IsPostviewTransferEnabled != value)
                {
                    Preference.SetPostviewTransferEnabled(value);
                    _IsPostviewTransferEnabled = value;
                    OnPropertyChanged("IsPostviewTransferEnabled");
                    OnPropertyChanged("SelectedIndexPostviewTransferEnabled");
                }
            }
            get
            {
                return _IsPostviewTransferEnabled;
            }
        }

        public string[] CandidatesPostviewTransferEnabled
        {
            get
            {
                return new string[] { "On", "Off" };
            }
        }

        public int SelectedIndexPostviewTransferEnabled
        {
            get
            {
                return IsPostviewTransferEnabled ? 0 : 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
                catch (COMException)
                {
                }
            }
        }
    }
}
