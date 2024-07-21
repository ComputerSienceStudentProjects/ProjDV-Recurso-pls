using UnityEngine;

public class MenuReveal : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Animator>().SetTrigger("Reveal");
    }
}
