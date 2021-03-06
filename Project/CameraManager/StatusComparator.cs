using Kazyx.RemoteApi;
using Kazyx.RemoteApi.Camera;
using Kazyx.WPPMM.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kazyx.WPPMM.CameraManager
{
    class StatusUpdater
    {
        internal static void AvailableApis(CameraStatus status, List<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.AvailableApis = latest.ToArray();
        }

        internal static void CameraStatus(CameraStatus status, string latest)
        {
            if (latest == null)
            {
                return;
            }
            status.Status = latest;
        }

        internal static void ZoomInfo(CameraStatus status, ZoomInfo latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ZoomInfo = latest;
        }

        internal static void LiveviewAvailability(CameraStatus status, bool latest)
        {
            status.IsLiveviewAvailable = latest;
        }

        internal static void PostviewSize(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.PostviewSizeInfo = latest;
        }

        internal static void SelfTimer(CameraStatus status, Capability<int> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.SelfTimer = latest;
        }

        internal static void ShootMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ShootMode = new ExtendedInfo<string>(latest);
        }

        internal static void ExposureMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ExposureMode = latest;
        }

        internal static void ShutterSpeed(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ShutterSpeed = latest;
        }

        internal static void ISO(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ISOSpeedRate = latest;
        }

        internal static void FNumber(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.FNumber = latest;
        }

        internal static void EvInfo(CameraStatus status, EvCapability latest)
        {
            if (latest == null)
            {
                return;
            }
            status.EvInfo = latest;
        }

        internal static void ProgramShift(CameraStatus status, bool? latest)
        {
            if (latest == null)
            {
                return;
            }
            var activated = latest.Value;
            status.ProgramShiftActivated = activated;
        }

        internal static void FocusStatus(CameraStatus status, string latest)
        {
            if (latest == null)
            {
                return;
            }
            status.FocusStatus = latest;
        }

        internal static void BeepMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.BeepMode = latest;
        }

        internal static void SteadyMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.SteadyMode = latest;
        }

        internal static void ViewAngle(CameraStatus status, Capability<int> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ViewAngle = latest;
        }

        internal static void MovieQuality(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.MovieQuality = latest;
        }

        internal static void Storages(CameraStatus status, List<StorageInfo> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.Storages = latest;
        }

        internal static void LiveviewOrientation(CameraStatus status, string latest)
        {
            if (latest == null)
            {
                return;
            }
            status.LiveviewOrientation = latest;
        }

        internal static void PictureUrls(CameraStatus status, List<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.PictureUrls = latest.ToArray();
        }

        internal static void FlashMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.FlashMode = latest;
        }

        internal static void FocusMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.FocusMode = latest;
        }

        internal static void TouchFocusStatus(CameraStatus status, TouchFocusStatus latest)
        {
            if (latest == null)
            {
                return;
            }
            status.TouchFocusStatus = latest;
        }

        internal static void ZoomSetting(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ZoomSetting = latest;
        }
        internal static void StillQuality(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.StillQuality = latest;
        }
        internal static void ContShootingMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ContShootingMode = latest;
        }
        internal static void ContShootingSpeed(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ContShootingSpeed = latest;
        }
        internal static void ContShootingResult(CameraStatus status, List<ContShootingResult> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ContShootingResult = latest;
        }
        internal static void FlipMode(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.FlipMode = latest;
        }
        internal static void SceneSelection(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.SceneSelection = latest;
        }
        internal static void IntervalTime(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.IntervalTime = latest;
        }
        internal static void ColorSetting(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.ColorSetting = latest;
        }
        internal static void MovieFormat(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.MovieFormat = latest;
        }
        internal static void InfraredRemoteControl(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.InfraredRemoteControl = latest;
        }
        internal static void TvColorSystem(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.TvColorSystem = latest;
        }
        internal static void TrackingFocusStatus(CameraStatus status, string latest)
        {
            if (latest == null)
            {
                return;
            }
            status.TrackingFocusStatus = latest;
        }
        internal static void TrackingFocus(CameraStatus status, Capability<string> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.TrackingFocus = latest;
        }
        internal static void BatteryInfo(CameraStatus status, List<BatteryInfo> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.BatteryInfo = latest;
        }
        internal static void RecordingTimeSec(CameraStatus status, int latest)
        {
            status.RecordingTimeSec = latest;
        }
        internal static void NumberOfShots(CameraStatus status, int latest)
        {
            status.NumberOfShots = latest;
        }
        internal static void AutoPowerOff(CameraStatus status, Capability<int> latest)
        {
            if (latest == null)
            {
                return;
            }
            status.AutoPowerOff = latest;
        }

        internal static async void StillSize(CameraStatus status, StillImageSizeEvent latest, CameraApiClient client)
        {
            if (latest == null)
            {
                return;
            }
            if (latest.CapabilityChanged)
            {
                try
                {
                    var size = await client.GetAvailableStillSizeAsync();
                    size.Candidates.Sort(CompareStillSize);
                    status.StillImageSize = size;
                }
                catch (RemoteApiException)
                {
                    DebugUtil.Log("Failed to get still image size capability");
                }
                catch (InvalidOperationException)
                {
                    DebugUtil.Log("Failed to sort still image size capability");
                }
            }
        }

        internal static async void WhiteBalance(CameraStatus status, WhiteBalanceEvent latest, CameraApiClient client)
        {
            if (latest == null)
            {
                return;
            }
            if (latest.CapabilityChanged)
            {
                try
                {
                    var wb = await client.GetAvailableWhiteBalanceAsync();
                    var candidates = new List<string>();
                    var tmpCandidates = new Dictionary<string, int[]>();
                    foreach (var mode in wb.Candidates)
                    {
                        candidates.Add(mode.WhiteBalanceMode);
                        var tmpList = new List<int>();
                        if (mode.Candidates.Count == 3)
                        {
                            for (int i = mode.Candidates[1]; i <= mode.Candidates[0]; i += mode.Candidates[2])
                            {
                                tmpList.Add(i);
                            }
                        }
                        tmpCandidates.Add(mode.WhiteBalanceMode, tmpList.ToArray());

                        DebugUtil.Log(mode.WhiteBalanceMode);
                        var builder = new System.Text.StringBuilder();
                        foreach (var val in mode.Candidates)
                        {
                            builder.Append(val).Append(", ");
                        }
                        DebugUtil.Log(builder.ToString());
                    }

                    /* mock date for testing */
#if COLOR_TEMPERTURE_MOCK
                    candidates.Add(WhiteBalanceMode.Manual);
                    var list = new List<int>();

                    for (int i = 2500; i <= 9900; i += 100)
                    {
                        list.Add(i);
                    }
                    tmpCandidates.Add(WhiteBalanceMode.Manual, list.ToArray());
#endif
                    /**/

                    status.WhiteBalance = new Capability<string> { Candidates = candidates, Current = wb.Current.Mode };
                    status.ColorTempertureCandidates = tmpCandidates;
                    status.ColorTemperture = wb.Current.ColorTemperature;
                }
                catch (RemoteApiException)
                {
                    DebugUtil.Log("Failed to get white balance capability");
                }
            }
            else
            {
                if (status.WhiteBalance != null)
                {
                    status.WhiteBalance.Current = latest.Current.Mode;
                }
                status.ColorTemperture = latest.Current.ColorTemperature;
            }
        }

        internal static void LiveviewFrameInfoAvailability(CameraStatus status, bool latest)
        {
            status.IsLiveviewFrameInfoAvailable = latest;
        }

        private static bool IsCapabilityUpdated<T>(Capability<T> current, Capability<T> latest)
        {
            if (!current.Current.Equals(latest.Current) ||
                current.Candidates.Count != latest.Candidates.Count)
            {
                return true;
            }

            for (int i = 0; i < current.Candidates.Count; i++)
            {
                if (!current.Candidates[i].Equals(latest.Candidates[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static int CompareStillSize(StillImageSize x, StillImageSize y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            if (!x.SizeDefinition.EndsWith("M") || !y.SizeDefinition.EndsWith("M"))
            {
                var comp = x.SizeDefinition.CompareTo(y.SizeDefinition);
                if (comp == 0)
                {
                    return x.AspectRatio.CompareTo(y.AspectRatio);
                }
                else
                {
                    return comp;
                }
            }

            var xv = (int)double.Parse(x.SizeDefinition.Substring(0, x.SizeDefinition.Length - 1)) * 100;
            var yv = (int)double.Parse(y.SizeDefinition.Substring(0, y.SizeDefinition.Length - 1)) * 100;

            if (xv == yv)
            {
                return x.AspectRatio.CompareTo(y.AspectRatio);
            }
            else
            {
                return xv < yv ? 1 : -1;
            }
        }
    }
}
