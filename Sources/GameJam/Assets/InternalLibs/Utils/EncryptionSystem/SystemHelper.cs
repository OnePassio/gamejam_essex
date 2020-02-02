using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Security.Cryptography;
using Gamejam.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Gamejam.Utils
{
    public class SystemHelper
    {

        private static string _deviceUniqueID;
        private static string _networkUniqueID;
        private static string _product_encode;
        public static void SetNewDeviceId(string deviceId)
        {
            _deviceUniqueID = deviceId;
            PlayerPrefs.SetString("DEVICE_ID_PREF", deviceId);
        }
        public static string deviceUniqueID
        {
            get
            {
                try
                {
                    if (_deviceUniqueID == null)
                        computeDeviceUniqueID();
                }
                catch (Exception ex)
                {
                    _deviceUniqueID = "Default";
                    Debug.LogError("SG Can't not get DeviceID " + ex.Message);
                }
                return _deviceUniqueID;
            }
        }

        public static string deviceNetworkUniqueID
        {
            get
            {
                if (_networkUniqueID == null)
                {
                    _networkUniqueID = deviceUniqueID;
                }

                return _networkUniqueID;
            }
        }

        public static void ChangeNetworkIdForTesting(string testDeviceId)
        {
            _networkUniqueID = testDeviceId;
        }
#if UNITY_EDITOR && UNITY_IPHONE
    	private static string getProductName()
    	{
    		if(_product_encode==null)
    		{
    			string text=PlayerSettings.productName;
    			System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
    			Byte[] encodedBytes = utf8.GetBytes(text);
    			Byte[] convertedBytes =
    				Encoding.Convert(Encoding.UTF8, Encoding.ASCII, encodedBytes);
    			System.Text.Encoding ascii = System.Text.Encoding.ASCII;
    			
    			_product_encode=ascii.GetString(convertedBytes);
    			//Debug.LogError(_product_encode);
    		}
    		return _product_encode;
    	}
#endif
#if UNITY_EDITOR && UNITY_ANDROID
    private static string getProductName()
    {
    	if(_product_encode==null)
    	{
    		string text=PlayerSettings.productName;
    		System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
    		Byte[] encodedBytes = utf8.GetBytes(text);
    		Byte[] convertedBytes =
    			Encoding.Convert(Encoding.UTF8, Encoding.ASCII, encodedBytes);
    		System.Text.Encoding ascii = System.Text.Encoding.ASCII;
    		
    		_product_encode=ascii.GetString(convertedBytes);
    		//Debug.LogError(_product_encode);
    	}
    	return _product_encode;
    }
#endif
        private static void computeDeviceUniqueID()
        {
            string systemID = "";
#if UNITY_EDITOR && UNITY_IPHONE
    		systemID = "E_IOS_"+SystemInfo.deviceUniqueIdentifier;
#elif UNITY_EDITOR && UNITY_ANDROID
    		systemID = "E_ANR_"+SystemInfo.deviceUniqueIdentifier;
#elif UNITY_IPHONE
    		systemID = "IOS_"+SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
    		systemID = "ANR_"+SystemInfo.deviceUniqueIdentifier;
#else
            systemID = "OTH_" + SystemInfo.deviceUniqueIdentifier;
#endif

            _deviceUniqueID = systemID.Replace("-", "").ToLower();

            /*int len = systemID.Length;
    		int numLong = len / 15;
    		if (len % 15 > 0)
    			numLong++;

    		_deviceUniqueID = MathfEx.BaseConvert(16, 36, systemID);*/

            /*byte[] bytes = new Byte[len / 2];
    		int numBytes = len / 2;
    		for (int i = 0; i < numBytes; i++)
    		{
    			string hexByte = systemID.Substring(i * 2, 2);
    			bytes[i] = Convert.ToByte(hexByte, 16);
    		}
    		_deviceUniqueID = System.Convert.ToBase64String(bytes);*/
        }
    }
}