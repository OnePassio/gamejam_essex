using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Gamejam.Utils
{
    public class ResourceLoaderManager : Singleton<ResourceLoaderManager>
    {
        public Dictionary<string, WWW> GetLoader()
        {
            return dicResourceLoader;
        }
        private Dictionary<string, WWW> dicResourceLoader = new Dictionary<string, WWW>();
        private Dictionary<string, Texture2D> dicTextureLoader = new Dictionary<string, Texture2D>();
        private Dictionary<string, Sprite> dicSpriteLoader = new Dictionary<string, Sprite>();
        private Dictionary<string, Action<Sprite>> dicWait = new Dictionary<string, Action<Sprite>>();


        public void SaveToLocalCacheFile(Texture2D tex, string file)
        {
            string pathFolder = FileUtilities.GetWritablePath("");
            string path = "";
            if (file.Contains(".png"))
            {
                path = pathFolder + file;
            }
            else
            {
                path = pathFolder + file + ".png";
            }
            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);

#if UNITY_IPHONE
		    UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
        }

        public bool CheckExistCacheFile(string file)
        {
            string pathFolder = FileUtilities.GetWritablePath("");
            string path = "";
            if (file.Contains(".png"))
            {
                path = pathFolder + file;
            }
            else
            {
                path = pathFolder + file + ".png";
            }
            if (System.IO.File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetPathFileCache(string file)
        {
            string pathFolder = FileUtilities.GetWritablePath("");
            string path = "";
            if (file.Contains(".png"))
            {
                path = pathFolder + file;
            }
            else
            {
                path = pathFolder + file + ".png";
            }
            path = "file://" + path;
            return path;
        }


        public void DownloadSprite(string url, Action<Sprite> callback = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            if (dicSpriteLoader.ContainsKey(url))
            {
                callback(dicSpriteLoader[url]);
            }
            else if (dicWait.ContainsKey(url))
            {
                callback = dicWait[url];
            }
            else
            {
                dicWait[url] = callback;
                DownLoadTexture(url, texture =>
                {
                    if (texture != null)
                    {
                        Rect rect;
                        if (texture.width > texture.height)
                        {
                            rect = new Rect(texture.width / 2 - texture.height / 2, 0, texture.height, texture.height);
                        }
                        else
                        {
                            rect = new Rect(0, texture.height / 2 - texture.width / 2, texture.width, texture.width);
                        }

                        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                        dicSpriteLoader[url] = sprite;
                        if (callback != null)
                        {
                            callback(sprite);
                            if (dicWait.ContainsKey(url))
                                dicWait.Remove(url);
                        }
                    }
                });
            }

        }

        public void DownloadSpriteDefault(string url, Action<Sprite> callback = null, bool cacheLocal = false)
        {

            //Util.Logger.LogNuna ("url 1: "+ url);

            bool isLocal = false;
            string fileNamePath = url.Replace("\\", ",");
            fileNamePath = fileNamePath.Replace("/", ",");
            string[] clipName = fileNamePath.Split(',');
            if (clipName.Length > 0)
            {
                fileNamePath = clipName[clipName.Length - 1];
            }

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            if (cacheLocal)
            {
                if (CheckExistCacheFile(fileNamePath))
                {
                    url = GetPathFileCache(fileNamePath);
                    isLocal = true;
                }
            }

            //Util.Logger.LogNuna ("url checked local: "+ url);

            if (dicSpriteLoader.ContainsKey(url))
            {
                if (dicSpriteLoader[url] != null)
                {
                    callback(dicSpriteLoader[url]);
                    return;
                }
                else
                {

                    dicSpriteLoader.Remove(url);

                }
            }

            // else {
            DownLoadTexture(url, texture =>
            {
                if (texture != null)
                {
                    Rect rect = new Rect(0, 0, texture.width, texture.height);

                    Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                    //Util.Logger.LogNuna ("url save dic: "+ url);

                    dicSpriteLoader[url] = sprite;

                    if (cacheLocal && !isLocal)
                    {
                        SaveToLocalCacheFile(texture, fileNamePath);
                    }

                    if (callback != null)
                    {
                        callback(sprite);
                    }
                }
            });
            // }
        }

        public void DownLoadTexture(string url, Action<Texture2D> callback = null, Action error = null, bool needRetry = false, int maxRetry = 0, float wait = 0)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (error != null)
                {
                    error();
                }
                return;
            }
            if (dicTextureLoader.ContainsKey(url))
            {
                callback(dicTextureLoader[url]);
            }
            else
            {
                WWW w = GetWWW(url);
                if (w != null)
                {
                    if (w.texture != null)
                    {
                        dicTextureLoader[url] = w.texture;
                        if (callback != null)
                        {
                            callback(w.texture);
                        }
                    }
                }
                else
                {
                    DownloadFileUtil.Instance.OnDownloadFile(url,
                        wwwCallBack =>
                        {
                            if (wwwCallBack != null && string.IsNullOrEmpty(wwwCallBack.error))
                            {
                                dicTextureLoader[url] = wwwCallBack.texture;
                                if (callback != null)
                                {
                                    callback(wwwCallBack.texture);
                                }
                            }
                            else
                            {
                                //Debug.LogError(wwwCallBack.error);
                                if (needRetry)
                                {
                                    if (maxRetry <= 0)
                                    {
                                        if (error != null)
                                        {
                                            error();
                                        }
                                    }
                                    else
                                    {
                                        maxRetry -= 1;
                                        DownLoadTexture(url, callback, error, needRetry, maxRetry, wait);
                                        Debug.LogError("retry: " + maxRetry);
                                    }
                                }
                            }

                        },
                        process =>
                        {
                        }, 5,true, 3, wait);
                }
            }
        }
        public WWW GetWWW(string url)
        {
            WWW w = null;
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            if (dicResourceLoader.TryGetValue(url, out w))
            {
                if (w != null && string.IsNullOrEmpty(w.error))
                {
                    return w;
                }
            }
            return null;
        }
        public void ClearWWW(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            if (dicResourceLoader.ContainsKey(url))
            {
                dicResourceLoader.Remove(url);
            }

        }
        /*public void ClearWWW(string url){
		    WWW w = GetWWW (url);
		    if (w != null) {
			    if (w.assetBundle != null) {
				    w.assetBundle.Unload (true);
			    }
			    w.Dispose ();
			    dicResourceLoader.Remove (url);
			    System.GC.Collect ();
		    }
	    }*/
        public void AddWWW(string key, WWW w)
        {
            if (w != null && string.IsNullOrEmpty(w.error))
            {
                dicResourceLoader[key] = w;
            }
        }

        public void FreeAudioAndAssetBundle()
        {
            try
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, WWW> pair in dicResourceLoader)
                {
                    if (pair.Value != null && string.IsNullOrEmpty(pair.Value.error))
                    {
                        bool isHave = false;
                        if (pair.Value.assetBundle != null)
                        {
                            pair.Value.assetBundle.Unload(true);
                            isHave = true;
                        }
                        if (pair.Key.Contains(".mp3"))
                        {
                            isHave = true;
                        }
                        if (isHave)
                        {
                            pair.Value.Dispose();
                            list.Add(pair.Key);
                        }

                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    dicResourceLoader.Remove(list[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Unload Resource Exception:" + ex.Message);
            }
        }

        public void DisposeWWW(string url, WWW w)// using to free memory
        {
            try
            {
                if (w != null)
                {
                    if (w.assetBundle != null)
                    {
                        w.assetBundle.Unload(false);
                    }
                    else if (w.texture != null)
                    {
                        GameObject.Destroy(w.texture);
                    }
                    w.Dispose();
                    w = null;
                    dicSpriteLoader.Remove(url);
                    dicTextureLoader.Remove(url);
                    dicResourceLoader.Remove(url);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Unload Resource Exception:" + url + ex.Message);
            }
        }

        public void FreeMemory()// using to free memory
        {
            try
            {
                foreach (KeyValuePair<string, WWW> pair in dicResourceLoader)
                {
                    if (pair.Value != null)
                    {
                        if (pair.Value.assetBundle != null)
                        {
                            pair.Value.assetBundle.Unload(true);
                        }
                        if (pair.Value.GetAudioClip() != null)
                        {
                            pair.Value.GetAudioClip().UnloadAudioData();
                        }
                        if (pair.Value.texture != null)
                        {
                            GameObject.Destroy(pair.Value.texture);
                        }
                        pair.Value.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Unload Resource Exception:" + ex.Message);
            }
            dicSpriteLoader.Clear();
            dicTextureLoader.Clear();
            dicResourceLoader.Clear();

        }
        public void RemoveTexture(string key)
        {
            try
            {
                Texture2D t;
                dicTextureLoader.TryGetValue(key, out t);
                if (t != null)
                {
                    Texture2D.DestroyImmediate(t, true);
                    dicTextureLoader.Remove(key);
                    dicResourceLoader.Remove(key);
                }
                if (t != null)
                {
                    GameObject.Destroy(t);

                }


            }
            catch
            {
                Debug.LogError("Unload Resource Not Find");
            }
        }
    }

    public class DownloadFileUtil : SingletonMono<DownloadFileUtil>
    {

        // Use this for initialization
        void Start()
        {
            // set global and unique in game
            this.gameObject.AddComponent<GlobalData>();
        }

        // Update is called once per frame
        //	void Update () {
        //
        //	}

        public void OnDownloadFile(string url, Action<WWW> resultCallback, Action<float> processCallback, float secondTimeout, bool isCacheWWW, int maxRetry = 3, float wait = 0)
        {
            if (string.IsNullOrEmpty(url))
            {
                resultCallback(null);
                return;
            }
            if (secondTimeout < 0)
                secondTimeout = 1;
            StartCoroutine(OnDownloadFileRoutine(url, resultCallback, processCallback, secondTimeout,isCacheWWW, maxRetry, 0, wait));
        }
        //
        private IEnumerator OnDownloadFileRoutine(string url, Action<WWW> resultCallback, Action<float> processCallback, float secondTimeout, bool isCacheWWW, int maxRetry, int retryCount = 0, float wait = 0)
        {
            if (wait > 0)
            {
                yield return new WaitForSeconds(wait);
            }
            WWW w = null;
            w = ResourceLoaderManager.Instance.GetWWW(url);
            if (w != null)
            {// kiem tra co cache hay khong
                if (resultCallback != null)
                {
                    resultCallback(w);
                }
                if (processCallback != null)
                {
                    processCallback(1);
                }
                //Debug.LogError ("Co cache ne:"+url);
                yield break;
            }
            else
            {
                //Debug.LogError (" khong Co cache ne:"+url);

                w = new WWW(url);
                float cachePercent = 0;
                int unDownLoadStep = 0;
                bool isFailDownload = false;
                int freezeTime=0;
                while (!w.isDone)
                {
                    if (Mathf.Abs(cachePercent - w.progress) > 0.0f)
                    {
                        cachePercent = w.progress;
                        unDownLoadStep = 0;
                        if (processCallback != null)
                        {
                            processCallback(cachePercent);
                        }
                    }
                    else
                    { // truong hop mang qua cham hoac download file bi stop do rot mang...
                        unDownLoadStep++;
                        if (unDownLoadStep > 10 * secondTimeout)
                        { //secondTimeout
                            isFailDownload = true;
                            // Download stop because freeze forever
                            break;
                        }
                        if (processCallback != null)
                        {
                            processCallback(cachePercent);
                        }
                        freezeTime++;
                        if(freezeTime>20)
                        {
                            freezeTime=0;
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    if (!string.IsNullOrEmpty(w.error))
                    {
                        isFailDownload = true;
                    }
                    yield return null;
                }
                if (isFailDownload)
                {
                    RetryAttempDownloadFile(url, resultCallback, processCallback, secondTimeout,isCacheWWW, maxRetry, retryCount);
                }
                else
                { // Download Succesful
                    if(isCacheWWW)
                    {
                        ResourceLoaderManager.Instance.AddWWW(url, w);
                    }
                    if (resultCallback != null)
                    {
                        resultCallback(w);
                    }


                }
            }
        }
        public void RetryAttempDownloadFile(string url, Action<WWW> resultCallback, Action<float> processCallback, float secondTimeout, bool isCacheWWW, int maxRetry, int retryCount)
        {
            if (retryCount < maxRetry)// retry lai lan nua
            {
                retryCount++;
                StartCoroutine(OnDownloadFileRoutine(url, resultCallback, processCallback, secondTimeout,isCacheWWW, maxRetry, retryCount));
            }
            else
            {
                // total fail
                if (resultCallback != null)
                {
                    resultCallback(null);
                }
            }
        }
    }
}