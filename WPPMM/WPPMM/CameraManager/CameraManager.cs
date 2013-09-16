﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPPMM.Liveview;

namespace WPPMM.CameraManager
{
    class CameraManager
    {

        // singleton instance
        private static CameraManager cameraManager = new CameraManager();


        private static int TIMEOUT = 10;
        private static String dd_location = null;

        private static String endpoint = null;
        private static String liveViewUrl = null;
        private Liveview.LVProcessor lvProcessor = null;

        private static Action<String> wifiStatusListener = null;

        private CameraManager()
        {
           
        }

        public static CameraManager GetInstance()
        {
            return cameraManager;
        }

        public void InitializeConnection()
        {
            requestSearchDevices();
        }

        public void StartRecmodeAndStartLiveView()
        {
            if (endpoint == null)
            {
                Debug.WriteLine("error: endpoint is null");
            }

            Debug.WriteLine("endpoint: " + endpoint);
            String jsonReq = Json.Request.startRecMode();
            Debug.WriteLine("request json: " + jsonReq);
            
            // for NEX-5R, change recmode before startLiveView
            Json.XhrPost.Post(
                endpoint,
                jsonReq,
                OnStartRecmode,
                OnError);
    
        }

        public void OnStartRecmode(String json)
        {
            Debug.WriteLine("OnStartRecmode: " + json);
        }

        // live view
        public void StartLiveView()
        {
            

            if (endpoint == null)
            {
                Debug.WriteLine("error: endpoint is null");
                return;
            }

            String requestJson = Json.Request.startLiveview();
            Debug.WriteLine("requestJson: " + requestJson);

            Json.XhrPost.Post(endpoint, requestJson, OnStartLiveViewRetrieved, OnError);
        }

        public void OnStartLiveViewRetrieved(String json)
        {
            Debug.WriteLine("StartLiveView retrieved: " + json);

        }

        // callback methods (liveview)
        public void OnJpegRetrieved(byte[] data)
        {
            Debug.WriteLine("Jpeg retrived.");
        }

        public void OnLiveViewClosed()
        {
            Debug.WriteLine("liveView connection closed.");
        }


        private static void requestSearchDevices()
        {
            WPPMM.Ssdp.DeviceDiscovery.SearchScalarDevices(TIMEOUT, OnDDLocationFound, OnTimeout);
        }


        // callback methods (search)
        public static void OnDDLocationFound(String location)
        {
            dd_location = location;
            Debug.WriteLine("found dd_location: " + location);
            Deployment.Current.Dispatcher.BeginInvoke(() => { wifiStatusListener("found dd_location" + location); });
            

            // get endpoint
            Ssdp.DeviceDiscovery.RetrieveEndpoints(dd_location, OnRetrieveEndpoints, OnError);
        }


        public static void OnRetrieveEndpoints(Dictionary <String, String> result)
        {
            Debug.WriteLine("retrived endpoint");

            if (result.ContainsKey("camera"))
            {
                endpoint = result["camera"];
                Debug.WriteLine("camera url found: " + endpoint);
            }
            else
            {
                Debug.WriteLine("camera url not found from retrived dictionary");
            }

            
        }

        public static void OnTimeout()
        {
            Debug.WriteLine("request timeout.");
            Deployment.Current.Dispatcher.BeginInvoke(
                () => { wifiStatusListener("hoge"); });
            
        }

        public static void OnError()
        {
            Debug.WriteLine("Error, something wrong.");
        }

        public static void OnError(int errno)
        {
            Debug.WriteLine("Error: " + errno.ToString());
        }



        // callback for UI
        public void SetWiFiStatusListener(Action<String> listener)
        {
            wifiStatusListener = listener;
                
        }

    }
}