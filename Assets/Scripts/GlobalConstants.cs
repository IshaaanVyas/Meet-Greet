public static class GlobalConstants {

    /// <summary>
    /// Hardcoded username 
    /// </summary>
    public const string USERNAME = "wizard4";

    /// <summary>
    /// Hardcoded password 
    /// </summary>
    public const string PASSWORD = "iamwizABF98";

    public const string APISERVER_URL = "https://api.woz.nusssi.com";
    //public const string APISERVER_URL = "http://localhost:5010";
    //private const string DATASERVER_URL = "http://192.168.86.250:5010";
    //private const string DATASERVER_URL = "http://192.168.147.165:5010";

    /// <summary>
    /// Upload latest frame API URL.
    /// </summary>
    public const string APISERVER_UPLOAD_API = APISERVER_URL + "/upload";

    public const string APISERVER_USER_LOGIN = APISERVER_URL + "/user/login";
    public const string APISERVER_USER_LOGOUT = APISERVER_URL + "/user/logout";


    /// <summary>
    /// Wizard of Oz websocket server IP. 
    /// </summary>
    //public const string WOZ_WSSERVER_URL = "ws://localhost:5011/ws?type=1&token=";
    public const string WOZ_WSSERVER_URL = "wss://messenger.woz.nusssi.com/?type=1&token=";

    /// <summary> 
    /// ASR Websocket server IP. 
    /// </summary>
    public const string ASRSERVER_IP = "wss://asr1.woz.nusssi.com";
    // private static string ASRSERVER_IP = "ws://192.168.234.165:2700";
    //private static string ASRSERVER_IP = "ws://192.168.86.250:2700";

}