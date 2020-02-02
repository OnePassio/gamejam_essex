using UnityEngine;
using System.IO;
//using FullSerializer;

namespace Gamejam.Utils
{
    /// <summary>
    /// Helper class to deal with every task related to files and folder
    /// </summary>
    public class FileUtilities
    {
        public static bool IS_ENCYPT_GAME = true; // encypt for secure data
        public const string ENCYPT_GAME_KEY = "12FWsa";
        /// <summary>
        /// Encode from text data 
        /// </summary>
#pragma warning disable
        public static string EncodeData(string rawText)
        {
            if (IS_ENCYPT_GAME) {
                string encode = Encryption.Encrypt (rawText,ENCYPT_GAME_KEY);
                return encode;
            } 
            else {
                return rawText;
            }
        }
        #pragma warning disable

        /// <summary>
        /// Decode form encypt data
        /// </summary>
        public static string DecodeData(string encodeText)
        {
            if (IS_ENCYPT_GAME) {
                string final = "";
                string decode = Encryption.Decrypt (encodeText, ENCYPT_GAME_KEY, res => {
                    if (res == false) {
                        final = encodeText;
                    }
                });
                if (final.Length > 0)
                    return final;
                else
                    return decode;
            }
            else {
                return encodeText;
            }
        }


        /// <summary>
        /// Return a path to a writable folder on a supported platform
        /// </summary>
        /// <param name="relativeFilePath">A relative path to the file, from the out most writable folder</param>
        /// <returns></returns>
        public static string GetWritablePath(string relativeFilePath)
        {
            string result = "";
#if UNITY_EDITOR
            result = Application.dataPath.Replace("Assets", "DownloadedData") + "/" + relativeFilePath;
#elif UNITY_ANDROID
		    result = Application.persistentDataPath + "/" + relativeFilePath;
#elif UNITY_IPHONE
		    result = Application.persistentDataPath + "/" + relativeFilePath;
#elif UNITY_WP8 || NETFX_CORE
		    result = Application.persistentDataPath + "/" + relativeFilePath;
#endif
            return result;
        }
         public static string GetWritableFolder()
        {
            string result = "";
#if UNITY_EDITOR
            result = Application.dataPath.Replace("Assets", "DownloadedData") ;
#elif UNITY_ANDROID
		    result = Application.persistentDataPath ;
#elif UNITY_IPHONE
		    result = Application.persistentDataPath ;
#elif UNITY_WP8 || NETFX_CORE
		    result = Application.persistentDataPath ;
#endif
            return result;
        }

        public static void CreateDirectory(string folder){
            string folderName=GetWritableFolder()+"/"+folder;
            if (!Directory.Exists(folderName))
			{
				Directory.CreateDirectory(folderName);
			}
        }

        /// <summary>
        /// Read a file at specified path
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="isAbsolutePath">Is this path an absolute one?</param>
        /// <returns>Data of the file, in byte[] format</returns>
        public static byte[] LoadFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }

            string absolutePath = filePath;
            if (!isAbsolutePath) { absolutePath = GetWritablePath(filePath); }

            if (System.IO.File.Exists(absolutePath))
            {
                return System.IO.File.ReadAllBytes(absolutePath);
            }
            else
            {
                return null;
            }
        }

        public static string[] GetFiles(string directoryPath, bool isAbsolutePath = false)
        {
            if (string.IsNullOrEmpty(directoryPath)) return null;
            string absolutePath = directoryPath;
            if (!isAbsolutePath)
            {
                absolutePath = GetWritablePath(directoryPath);
            }

            if (System.IO.Directory.Exists(absolutePath))
            {
                return Directory.GetFiles(absolutePath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check if a file is existed or not
        /// </summary>
        public static bool IsFileExist(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return false;
            }

            string absolutePath = filePath;
            if (!isAbsolutePath) { absolutePath = GetWritablePath(filePath); }

			string folderName = Path.GetDirectoryName(absolutePath);
			//Debug.Log("Folder name: " + folderName+","+filePath);

			if (!Directory.Exists(folderName))
			{
				Directory.CreateDirectory(folderName);
			}

            bool result= (System.IO.File.Exists(absolutePath));
            return result;
        }



        /// <summary>
        /// Save a byte array to storage at specified path and return the absolute path of the saved file
        /// </summary>
        /// <param name="bytes">Data to write</param>
        /// <param name="filePath">Where to save file</param>
        /// <param name="isAbsolutePath">Is this path an absolute one or relative</param>
        /// <returns>Absolute path of the file</returns>
        public static string SaveFile(byte[] bytes, string filePath, bool isAbsolutePath = false, bool isSaveResource = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }
            //		if (isSaveResource) {
            //			SaveFileToResource (bytes, filePath);
            //		}
            //path to the file, in absolute format
            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }
            //create a directory tree if not existed
            string folderName = Path.GetDirectoryName(path);
            //Debug.Log("Folder name: " + folderName);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            //write file to storage
            File.WriteAllBytes(path, bytes);
#if UNITY_IPHONE
		    UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
            return path;
        }


        public static string SaveFileOtherThread(byte[] bytes, string filePath)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }
            //		if (isSaveResource) {
            //			SaveFileToResource (bytes, filePath);
            //		}
            //path to the file, in absolute format
            string path = filePath;

            //create a directory tree if not existed
            string folderName = Path.GetDirectoryName(path);
            //Debug.Log("Folder name: " + folderName);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            //write file to storage
            File.WriteAllBytes(path, bytes);
            return path;
        }


        /// <summary>
        /// Delete a file from storage using default setting
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="isAbsolutePath">Is this file path an absolute path or relative one?</param>
        public static bool DeleteFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
                return false;

            if (isAbsolutePath)
            {
                if (System.IO.File.Exists(filePath))
                {
                    //Debug.Log("Delete file : " + absoluteFilePath);
                    System.IO.File.Delete(filePath);
                    return true;
                }

                return false;
            }
            else
            {
                string file = GetWritablePath(filePath);
                return DeleteFile(file, true);
            }
        }
    }
}