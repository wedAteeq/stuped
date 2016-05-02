using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using System;
using Sfs2X.Entities.Data;
using UnityEditor;

public class DeleteAccount : MonoBehaviour
{

    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
    private string ServerIP = "127.0.0.1";// Default host
    private int defaultTcpPort = 9933;// Default TCP port
    private int defaultWsPort = 8888;			// Default WebSocket port
    private string ZoneName = "3DexCityZone";
    private int ServerPort = 0;

    private SmartFox sfs;
    private string username;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField UserName;
    public Text TextMessage;
    public Transform Delete;
    public Transform ConfirmDelete;

    void Start()
    {
        TextMessage.text = "";

    }

    // Update is called once per frame
    void Update()
    {
        // As Unity is not thread safe, we process the queued up callbacks on every frame
        if (sfs != null)
            sfs.ProcessEvents();
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

    public void OnDeleteButtonClocked()
    {
        int index;
        string admin;
        username = UserName.text;
        index = username.IndexOf("n");
        admin = username.Substring(0, index + 1);
        Debug.Log("delete account");
        if (username == "" || username == " ")
            TextMessage.text = "Missing to fill required value";
        else
        if (admin.Equals("Admin"))
            TextMessage.text = "You are not allow to delete admin";
        else
        {  // Enable interface
            enableInterface(false);

#if UNITY_WEBGL
            {
             sfs = new SmartFox(UseWebSocket.WS);
             ServerPort = defaultWsPort;
            }
#else
            {
                sfs = new SmartFox();
                ServerPort = defaultTcpPort;
            }
#endif

            sfs.ThreadSafeMode = true;

            sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
            sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

            sfs.Connect(ServerIP, ServerPort);
            UserName.text = "";
            enableInterface(true);

        }

    }//end

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            // Login
            Debug.Log("Successfully Connected!");

            sfs.Send(new LoginRequest("", "", ZoneName));
        }
        else
        {
            Debug.Log("Connection Failed!");
            // Remove SFS2X listeners and re-enable interface
            reset();

            // Show error message
            TextMessage.text = "Connection failed!";
        }
    }

    private void OnLogin(BaseEvent evt)
    {
        Debug.Log("Logged In: " + evt.Params["user"]);

        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", username);
        sfs.Send(new ExtensionRequest("DeleteAccount", objOut));

    }

    private void enableInterface(bool enable)
    {
        TextMessage.text = "";

    }

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface
        reset();

        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
        {
            // Show error message
            TextMessage.text = "Connection was lost; reason is: " + reason;
        }
    }//end

    private void OnLoginError(BaseEvent evt)
    {    // Show error message
        string message = (string)evt.Params["errorMessage"];
        //TextMessage.text = "Login failed: " + message;
        Debug.Log("Login failed: " + message);

        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        Debug.Log("2: ");

        ISFSObject objIn = (SFSObject)evt.Params["params"];

        string result = objIn.GetUtfString("DeleteResult");

        if (result == "Successful")
        {
            Debug.Log("Successful");
            //TextMessage.text = "the account deleted successfully";
            //EditorUtility.DisplayDialog("Waring Message", "         The account deleted successfully", "ok");

            ConfirmDelete.gameObject.SetActive(true);
            Delete.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("error");
            //TextMessage.text = "Your account has not been deleted";
            //EditorUtility.DisplayDialog("Waring Message", "         Your account has not been deleted", "ok");

        }


    }//end extension

    public void OnOKButonClicked()
    {
        ConfirmDelete.gameObject.SetActive(false);
    }

    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();

        sfs = null;

        // Enable interface
        enableInterface(true);
    }

}