using UnityEngine;
using UnityEditor;
using Sfs2X;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class AAvatar : MonoBehaviour {
    private SmartFox sfs;

    private string userName;
  
    // Update is called once per frame
    public void getAvatarTye(SmartFox sfs2x, string user)
    {
        sfs = sfs2x;

        Debug.Log("in method ");
        userName = user;
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", userName);
        sfs.Send(new ExtensionRequest("RtrvAvatar", objOut));

    }

}
