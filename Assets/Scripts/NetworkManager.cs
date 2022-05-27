using UnityEngine;
using UnityEngine.Networking;

using NativeWebSocket;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{

    [SerializeField]
    private ModelManager m_ModelManager;
    Animator animator;
    public GameObject person1;



    /// <summary>
    /// websocket to WOZ 
    /// </summary>
    WebSocket websocketWOZ;

    private string accessToken;

    public NetworkManager()
    {
    }

    private async Task ConnectWSAsync()
    {
        websocketWOZ = new WebSocket(GlobalConstants.WOZ_WSSERVER_URL + accessToken);
        websocketWOZ.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocketWOZ.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocketWOZ.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocketWOZ.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Got Message: " + message);
            // Tokenize
            string[] subs = message.Split(':', 14);

            // Check command type
            int cmd = Int32.Parse(subs[0]);
            if (cmd == 1)
            {
                string uuid = subs[4];
                int x = Int32.Parse(subs[5]);
                int y = Int32.Parse(subs[6]);
                int type = Int32.Parse(subs[8]);
                bool askingQ1 = animator.GetBool("askingQ1");
                //m_ModelManager.SpawnObject(uuid, x, y, type, subs[13]);
                if (x == 1)
                {
                    animator.SetBool("talking1", true);
                }
                if (x == 2)
                {
                    animator.SetBool("askingQ1", true);
                }       
                if (x == 3)
                {
                    animator.SetBool("thumbsup1", true);
                }


                if (y == 1)
                {
                    
                }
            }
            else if (cmd == 2)
            {
                string uuid = subs[4];
                //m_ModelManager.DeleteObject(uuid);
                animator.SetBool("talking1", false);
            }
        };
        await websocketWOZ.Connect();
    }

    private IEnumerator LoginUser()
    {
        //WWWForm form = new WWWForm();
        //form.AddField("username", GlobalConstants.USERNAME);
        //form.AddField("password", GlobalConstants.PASSWORD);
        UserData data = new UserData();
        data.username = GlobalConstants.USERNAME;
        data.password = GlobalConstants.PASSWORD;

        string jsonstr = JsonUtility.ToJson(data);
        using (UnityWebRequest www = new UnityWebRequest(GlobalConstants.APISERVER_USER_LOGIN, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonstr);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
	        // Sends header: "Content-Type: custom/content-type";
            //uploader.contentType = "custom/content-type";
            yield return www.SendWebRequest();
            if (www.isDone)
            {
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    var replytext = www.downloadHandler.text;
                    LoginDataReply reply = JsonUtility.FromJson<LoginDataReply>(replytext);
                    accessToken = reply.data.access_token;

                    yield return ConnectWSAsync();
                }
            }
        }
    }

    void Start()
    {
        StartCoroutine(LoginUser());
        animator = person1.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // #if !UNITY_WEBGL || UNITY_EDITOR
        if (websocketWOZ != null)
        {
            websocketWOZ.DispatchMessageQueue();
        }
        //if (animator.GetBool("askingQ1"))
        //{
        //    animator.SetBool("askingQ1", false);
        //}
        // #endif
    }

    /// <summary>
    /// Login as the user/password for uploading to the right tenant/slot. 
    /// </summary>
    private void Login()
    {
    }

    public IEnumerator UploadFile(byte[] payload)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image.bin", payload);
        using (UnityWebRequest www = UnityWebRequest.Post(GlobalConstants.APISERVER_UPLOAD_API, form))
        {
            // Sends header: "Content-Type: custom/content-type";
            //uploader.contentType = "custom/content-type";
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            yield return www.SendWebRequest();
            if (www.isDone)
            {
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    //do something
                }
            }
        }
    }

    private async void OnApplicationQuit()
    {
        await websocketWOZ.Close();
    }
}

