using Kazyx.RemoteApi;
using Kazyx.WPPMM.CameraManager;
using Kazyx.WPPMM.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace Kazyx.WPPMM.DataModel
{
    public class ControlPanelViewData : INotifyPropertyChanged
    {
        private readonly CameraStatus status;
        private CameraManager.CameraManager manager;

        public ControlPanelViewData(CameraStatus status)
        {
            this.status = status;
            this.manager = CameraManager.CameraManager.GetInstance();

            status.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "AvailableApis":
                        OnPropertyChanged("CpIsAvailableSelfTimer");
                        OnPropertyChanged("CpIsAvailableShootMode");
                        OnPropertyChanged("CpIsAvailablePostviewSize");
                        OnPropertyChanged("CpIsAvailableStillImageFunctions");
                        OnPropertyChanged("CpIsAvailableExposureMode");
                        OnPropertyChanged("CpIsAvailableExposureCompensation");
                        OnPropertyChanged("CpDisplayValueExposureCompensation");
                        OnPropertyChanged("CpSelectedIndexExposureCompensation");
                        break;
                    case "PostviewSizeInfo":
                        if (status.IsAvailable("setPostviewImageSize"))
                        {
                            OnPropertyChanged("CpCandidatesPostviewSize");
                            OnPropertyChanged("CpSelectedIndexPostviewSize");
                        }
                        break;
                    case "SelfTimerInfo":
                        if (status.IsAvailable("setSelfTimer"))
                        {
                            OnPropertyChanged("CpCandidatesSelfTimer");
                            OnPropertyChanged("CpSelectedIndexSelfTimer");
                        }
                        break;
                    case "ShootModeInfo":
                        if (status.IsAvailable("setShootMode"))
                        {
                            OnPropertyChanged("CpCandidatesShootMode");
                            OnPropertyChanged("CpSelectedIndexShootMode");
                        }
                        OnPropertyChanged("CpIsAvailableStillImageFunctions");
                        break;
                    case "ExposureMode":
                        if (status.IsAvailable("setExposureMode"))
                        {
                            OnPropertyChanged("CpSelectedIndexExposureMode");
                            OnPropertyChanged("CpCandidatesExposureMode");
                        }
                        break;
                    case "EvInfo":
                        if (status.IsAvailable("setExposureCompensation"))
                        {
                            OnPropertyChanged("CpSelectedIndexExposureCompensation");
                            OnPropertyChanged("CpCandidatesExposureCompensation");
                            OnPropertyChanged("CpMaxExposureCompensation");
                            OnPropertyChanged("CpMinExposureCompensation");
                            OnPropertyChanged("CpDisplayValueExposureCompensation");
                        }
                        break;
                    default:
                        break;
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(name));
                    }
                    catch (COMException)
                    {
                        Debug.WriteLine("Caught COMException: ControlPanelViewData");
                    }
                    catch (NullReferenceException)
                    {
                        Debug.WriteLine("Caught NullReferenceException: ControlPanelViewData");
                    }
                    catch (System.InvalidOperationException e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                });
            }
        }

        public int CpSelectedIndexSelfTimer
        {
            get
            {
                return SettingsValueConverter.GetSelectedIndex(status.SelfTimerInfo);
            }
            set
            {
                if (status.SelfTimerInfo != null)
                {
                    if (status.SelfTimerInfo.candidates.Length > value)
                    {
                        status.SelfTimerInfo.current = status.SelfTimerInfo.candidates[value];
                    }
                    else
                    {
                        status.SelfTimerInfo.current = 0;
                    }
                }
            }
        }

        public string[] CpCandidatesSelfTimer
        {
            get
            {
                return SettingsValueConverter.FromSelfTimer(status.SelfTimerInfo).candidates;
            }
        }

        public bool CpIsAvailableSelfTimer
        {
            get
            {
                return status.IsAvailable("setSelfTimer") &&
                    status.SelfTimerInfo != null &&
                    manager != null &&
                    !manager.IntervalManager.IsRunning;
            }
        }

        public int CpSelectedIndexPostviewSize
        {
            get
            {
                return SettingsValueConverter.GetSelectedIndex(status.PostviewSizeInfo);
            }
            set
            {
                if (status.PostviewSizeInfo != null)
                {
                    if (status.PostviewSizeInfo.candidates.Length > value)
                    {
                        status.PostviewSizeInfo.current = status.PostviewSizeInfo.candidates[value];
                    }
                    else
                    {
                        status.PostviewSizeInfo.current = null;
                    }
                }
            }
        }

        public string[] CpCandidatesPostviewSize
        {
            get
            {
                return SettingsValueConverter.FromPostViewSize(status.PostviewSizeInfo).candidates;
            }
        }

        public bool CpIsAvailablePostviewSize
        {
            get
            {
                return status.IsAvailable("setPostviewImageSize") &&
                    status.PostviewSizeInfo != null &&
                    manager != null &&
                    !manager.IntervalManager.IsRunning;
            }
        }

        public int CpSelectedIndexShootMode
        {
            get
            {
                return SettingsValueConverter.GetSelectedIndex(status.ShootModeInfo);
            }
            set
            {
                if (status.ShootModeInfo != null)
                {
                    if (status.ShootModeInfo.candidates.Length > value)
                    {
                        status.ShootModeInfo.current = status.ShootModeInfo.candidates[value];
                    }
                    else
                    {
                        status.ShootModeInfo.current = null;
                    }
                }
            }
        }

        public string[] CpCandidatesShootMode
        {
            get
            {
                return SettingsValueConverter.FromShootMode(status.ShootModeInfo).candidates;
            }
        }

        public bool CpIsAvailableShootMode
        {
            get
            {
                return status.IsAvailable("setShootMode") &&
                    status.ShootModeInfo != null &&
                    manager != null &&
                    !manager.IntervalManager.IsRunning;
            }
        }

        public int CpSelectedIndexExposureMode
        {
            get
            {
                return SettingsValueConverter.GetSelectedIndex(status.ExposureMode);
            }
            set
            {
                if (status.ExposureMode != null)
                {
                    if (status.ExposureMode.candidates.Length > value)
                    {
                        status.ExposureMode.current = status.ExposureMode.candidates[value];
                    }
                    else
                    {
                        status.ExposureMode.current = null;
                    }
                }
            }
        }

        public string[] CpCandidatesExposureMode
        {
            get { return SettingsValueConverter.FromExposureMode(status.ExposureMode).candidates; }
        }

        public bool CpIsAvailableExposureMode
        {
            get
            {
                return status.IsAvailable("setExposureMode") && status.ExposureMode != null && manager != null && !manager.IntervalManager.IsRunning;
            }
        }

        public bool CpIsAvailableExposureCompensation
        {
            get
            {
                return status.IsAvailable("setExposureCompensation") && status.EvInfo != null && manager != null && !manager.IntervalManager.IsRunning;
            }
        }

        public int CpSelectedIndexExposureCompensation
        {
            get
            {
                if (status == null || status.EvInfo == null || !status.IsAvailable("setExposureCompensation"))
                {
                    return 0;
                }
                return SettingsValueConverter.GetSelectedIndex(status.EvInfo);
            }
            set
            {
                if (status.EvInfo != null)
                {
                    if (value <= status.EvInfo.Candidate.MaxIndex && value >= status.EvInfo.Candidate.MinIndex)
                    {
                        status.EvInfo.CurrentIndex = value;
                    }
                    else
                    {
                        status.EvInfo.CurrentIndex = 0;
                    }
                }
            }
        }

        public string[] CpCandidatesExposureCompensation
        {
            get { return SettingsValueConverter.FromExposureCompensation(status.EvInfo); }
        }

        public int CpMaxExposureCompensation
        {
            get
            {
                if (status == null || status.EvInfo == null)
                {
                    return 0;
                }
                return status.EvInfo.Candidate.MaxIndex;
            }
        }

        public int CpMinExposureCompensation
        {
            get
            {
                if (status == null || status.EvInfo == null)
                {
                    return 0;
                }
                return status.EvInfo.Candidate.MinIndex;
            }
        }

        public string CpDisplayValueExposureCompensation
        {
            get
            {
                if (status == null || status.EvInfo == null || !status.IsAvailable("setExposureCompensation"))
                {
                    return "--";
                }
                var value = EvConverter.GetEv(status.EvInfo.CurrentIndex, status.EvInfo.Candidate.IndexStep);
                if (value > 0)
                {
                    return "+" + Math.Round(value, 1, MidpointRounding.AwayFromZero).ToString("0.0");
                }
                else
                {
                    return Math.Round(value, 1, MidpointRounding.AwayFromZero).ToString("0.0");
                }
            }
        }


        public bool CpIsAvailableStillImageFunctions
        {
            get
            {
                if (status == null || status.ShootModeInfo == null)
                {
                    return false;
                }
                return status.ShootModeInfo.current == ShootModeParam.Still &&
                    manager != null && !manager.IntervalManager.IsRunning;
            }
        }

        public void OnControlPanelPropertyChanged(string name)
        {
            OnPropertyChanged(name);
        }
    }
}