using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;


public class ContactUs : MonoBehaviour
{

    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
    private string ServerIP = "127.0.0.1";// Default host
    private int defaultTcpPort = 9933;// Default TCP port
    private int defaultWsPort = 8888;           // Default WebSocket port
    private string ZoneName = "3DexCityZone";
    private int ServerPort = 0;

    private SmartFox sfs;
    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField email, subject, msg;
    public Text textMsg;
    public Transform Thanks, Contact;
	public GameObject home;
    private string Email, Subj, Msg;


    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------


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

    public void OnSendButtonclicked()
    {
        Email=email.text;
        Subj = subject.text;
        Msg = msg.text;
        if (requredFilled())
            if (Email.IndexOf("@") != -1)
            {

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
            else
                textMsg.text = "Invalid email address";
        else
            textMsg.text = "Missing to fill required value(s)";

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
            Debug.Log("Connection failed; is the server running at all?");
        }
    }

    private void OnLogin(BaseEvent evt)
    {
        Debug.Log("Logged In: " + evt.Params["user"]);

        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Email", Email);
        objOut.PutUtfString("Subject", Subj);
        objOut.PutUtfString("Message", Msg);
        sfs.Send(new ExtensionRequest("ContactUs", objOut));
    }



    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface
        reset();

        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
        {
            // Show error message
            Debug.Log("Connection was lost; reason is: " + reason);
        }
    }//end

    private void OnLoginError(BaseEvent evt)
    {    // Show error message
        string message = (string)evt.Params["errorMessage"];
        Debug.Log("Login failed: " + message);

        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        Debug.Log("extension");

        ISFSObject objIn = (SFSObject)evt.Params["params"];

        string result = objIn.GetUtfString("Result");

        if (result == "Successful")
        {
            Debug.Log("Successful");
            Contact.gameObject.SetActive(false);
			home.SetActive (true);
			Thanks.gameObject.SetActive(true);
        }
        else
            Debug.Log("error");

    }//end extension

    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();
        sfs = null;
    }
    private bool requredFilled()
    {
        if (Email == "" || Email == " " || Subj == "" || Subj == " " || Msg == "" || Msg == " " )
            return false;
        else
            return true;
    }
}