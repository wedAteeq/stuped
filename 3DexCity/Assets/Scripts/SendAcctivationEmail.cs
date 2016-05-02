using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Logging;
using UnityEngine.UI;
using System;
using Sfs2X.Util;
using UnityEditor;

public class SendAcctivationEmail : MonoBehaviour
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
    private string email;
    private string message;
    private int room = 0;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField MemberUserName;
    public InputField MemberEmail;
    public Toggle MemberAccountType;
    public Toggle MemberActivateRoom;

    public InputField AdminUserName;
    public InputField AdminEmail;
    public Transform welcome;
    public Transform Login;
    public Transform AdminView;

    string CMD_ActivateEmail = "$SignUp.ResendEmail";

  
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


    public void OnOKButtonClicked()
    {
        if (AdminUserName.text != "")
        {
            username = AdminUserName.text;
            email = AdminEmail.text;
        }
        else
        {

            username = Transverser.MemberUsername;// MemberUserName.text;
            email = Transverser.MemberEmail;// MemberEmail.text;
        }

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

    }


    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface

        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
        {
            // Show error message
            Debug.Log(reason);
        }
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        ISFSObject objIn = (SFSObject)evt.Params["params"];

        if (cmd == CMD_ActivateEmail)
        {

            if (objIn.ContainsKey("success"))
            {
                message = "The activation re-email send successfyl";
                Debug.Log(message);
                //if (AdminUserName.text != "")
                //  AdminView.gameObject.SetActive(true);
                //  else
                // Login.gameObject.SetActive(true);

            }
            else
            {
                message = objIn.GetUtfString("errorMessage");
                //message = "Resend Error: " + message;
                Debug.Log(message);
                EditorUtility.DisplayDialog("Waring Message", "The activation email did not send because " + message, "ok");
                //string user = Transverser.MemberUsername;
                // if (AdminUserName.text!="")
                // int AdminIndex = user.IndexOf("n");
                // string admin = user.Substring(0, AdminIndex + 1);
                // if (admin.Equals("Admin"))
                //   AdminView.gameObject.SetActive(true);
                //  else
                //Login.gameObject.SetActive(true);

            }
            if (AdminUserName.text != "")
                AdminView.gameObject.SetActive(true);
            else
                Login.gameObject.SetActive(true);

            welcome.gameObject.SetActive(false);

        }

    }

    private void OnLoginError(BaseEvent evt)
    {
        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface

        // Show error message
        string message = "Login failed: " + (string)evt.Params["errorMessage"];
        Debug.Log(message);

    }

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            message = "Successfully Connected!";
            Debug.Log(message);

            sfs.Send(new LoginRequest("", "", ZoneName));
        }
        else
        {
            // Remove SFS2X listeners and re-enable interface
            message = "Connection failed; is the server running at all?";
            // Show error message
            Debug.Log(message);

        }
    }

    private void OnLogin(BaseEvent evt)
    {
        Debug.Log("Logged In: " + evt.Params["user"]);

        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", username);
        objOut.PutUtfString("email", email);

        sfs.Send(new ExtensionRequest(CMD_ActivateEmail, objOut));

    }


}

