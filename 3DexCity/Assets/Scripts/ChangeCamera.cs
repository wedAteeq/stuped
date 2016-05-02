using UnityEngine;
using System.Collections;

public class ChangeCamera : MonoBehaviour
{

    public GameObject player;
    public GameObject FPCamera;
    public GameObject TPCamera;
    int cheack;
    // Use this for initialization
    void Start()
    {

        FPCamera.gameObject.SetActive(false);
        TPCamera.gameObject.SetActive(true);
        cheack = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))

        {
            if (cheack == 0)


            {
                FPCamera.gameObject.SetActive(true);
                TPCamera.gameObject.SetActive(false);
                cheack = 1;
            }

            else

            {
                FPCamera.gameObject.SetActive(false);
                TPCamera.gameObject.SetActive(true);
                cheack = 0;
            }
        }
    }
}
