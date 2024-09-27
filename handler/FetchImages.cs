using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace InfiniteItems
{
    public class IconFetcherClient : MonoBehaviour
    {
        private readonly HttpClient _client;

        private string Query;

        public IconFetcherClient(string q)
        {
            _client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            Query = q;
        }

        public void ChangeQuery(string nq)
        {
            Query = nq;
        }

        public async Task<Texture2D> GetRandomTexture()
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://picsum.photos/64"))
            {
                var asyncOp = www.SendWebRequest();

                while (asyncOp.isDone == false)
                    await Task.Delay(1000 / 30);

                if( www.result!=UnityWebRequest.Result.Success )
                {
                    Debug.Log($"{www.error}, URL:{www.url}");
                    return null;
                }
                else
                {
                    return DownloadHandlerTexture.GetContent(www);
                }
            }
        }
	}
}
