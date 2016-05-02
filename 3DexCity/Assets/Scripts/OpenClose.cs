using UnityEngine;
using System.Collections;

public class OpenClose : MonoBehaviour
{

    Animator anim;
    bool doorOpen;
    // Use this for initialization
    void Start()
    {
        doorOpen = false;
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
		if (col.gameObject.tag == "Player" && NetworkManager.Instance.EnterRoom( this.transform.parent.name))
        {
            doorOpen = true;
            Doors("open");
        }
    }
    // Update is called once per frame
    void OnTriggerExit()
    {
        if (doorOpen)
        {
            doorOpen = false;
            Doors("close");
        }
    }
    void Doors(string dir)
    {
        anim.SetTrigger(dir);
    }
}