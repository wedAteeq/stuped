using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Util;
using UnityEngine.UI;

public class ActivateAccountPage : MonoBehaviour
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
    private string activation;


    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public Text TextMessage;
    public Transform ActivateAccount;
    public Transform Login;
    public InputField Activation;



    // Use this for initialization
    void Start()
    {
        enableInterface(true);
        Activation.text = "";
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

    public void OnActivateAccountButtonClicked()
    {
        activation = Activation.text;
        if (activation == "" || activation == " ")
            TextMessage.text = "Missing to fill required value";
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

        }

    }


    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();

        sfs = null;

        // Enable interface
        enableInterface(true);
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
    }


    private void OnExtensionResponse(BaseEvent evt)
    {
        ISFSObject objIn = (SFSObject)evt.Params["params"];
        string result = objIn.GetUtfString("result");

        if (result == "Successful")
        {
            Debug.Log("Successful");
            TextMessage.text = "Your account activated successfully";
            Activation.text = "";
            TextMessage.text = "";
            ActivateAccount.gameObject.SetActive(false);
            enableInterface(true);
            Login.gameObject.SetActive(true);

        }
        else
        {
            Debug.Log("error");
            TextMessage.text = "Your account has not activated";

        }


    }



    private void OnLoginError(BaseEvent evt)
    {
        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();

        // Show error message
        TextMessage.text = "Login failed: " + (string)evt.Params["errorMessage"];
        Debug.Log("Login failed: " + (string)evt.Params["errorMessage"]);
    }

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Debug.Log("Successfully Connected!");
            sfs.Send(new LoginRequest("", "", ZoneName));

        }
        else
        {
            Debug.Log("Connection Failed!");
            // Remove SFS2X listeners and re-enable interface
            reset();

            // Show error message
            TextMessage.text = "Connection failed; is the server running at all?";
        }
    }

    void OnLogin(BaseEvent e)
    {
        Debug.Log("Logged In: " + e.Params["user"]);
        ISFSObject objOut = new SFSObject();
        string act_code = activation;
        objOut.PutUtfString("act_code", act_code);
        sfs.Send(new ExtensionRequest("ActivateAccount", objOut));


    }

    private void enableInterface(bool enable)
    {
        Activation.interactable = enable;

        TextMessage.text = "";
    }


    void OnApplicationQuit()
    {
        if (sfs != null && sfs.IsConnected)
        {
            sfs.Disconnect();
        }
    }
}
