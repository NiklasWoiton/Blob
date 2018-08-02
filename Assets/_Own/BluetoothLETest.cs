//#if ENABLE_WINMD_SUPPORT
//    Debug.Log("Windows Runtime Support enabled");
//    // Put calls to your custom .winmd API here
//#endif
//
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.System;
//
//using Windows.Devices.Bluetooth.Advertisement;
//using UnityEngine;
//
//public class BluetoothLETest : MonoBehaviour {
//
//	void Start () {
//        // Create Bluetooth Listener
//        var watcher = new BluetoothLEAdvertisementWatcher();
//
//        watcher.ScanningMode = BluetoothLEScanningMode.Active;
//
//        // Only activate the watcher when we're recieving values >= -80
//        watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;
//
//        // Stop watching if the value drops below -90 (user walked away)
//        watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;
//
//        // Register callback for when we see an advertisements
//        watcher.Received += OnAdvertisementReceived;
//
//        // Wait 5 seconds to make sure the device is really out of range
//        watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
//        watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);
//
//        // Starting watching for advertisements
//        watcher.Start();
//
//    }
//
//    private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
//    {
//        // Tell the user we see an advertisement and print some properties
//        Debug.Log(String.Format("Advertisement: BT_ADDR: {0} FR_NAME: {1}", eventArgs.BluetoothAddress, eventArgs.Advertisement.LocalName));
//    }
//}
