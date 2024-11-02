using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // could activate a variable and a timer in OPenDoor, SO
        //that when it activates we slide the door downward (no animation needed) so it goes behind the
        //ground and then we destroy it
    }


    //also should do something with playerPrefs so that we can keep track of which doors have been opened
    public void OpenDoor()
    {
        Destroy(gameObject); // simple for now lol
    }
}
