using UnityEngine;
using UnityEngine.UIElements;

public class DemoEnd : MonoBehaviour
{

    [SerializeField] private UIDocument _uiDocument;
    // Start is called before the first frame update
    void Start()
    {
        _uiDocument.rootVisualElement.Q<Button>().clicked += Exit;
    }

    // Update is called once per frame
    public void Exit()
    {
        Application.Quit(0);
    }
    
}
