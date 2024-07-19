using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuReveal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetTrigger("Reveal");
    }
}
