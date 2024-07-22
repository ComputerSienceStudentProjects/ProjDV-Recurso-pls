using System.Runtime.InteropServices;
using UnityEngine;

public class MouseEmulator : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool SetCursorPos(int X, int Y);
    
    private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    private const uint MOUSEEVENTF_LEFTUP = 0x04;
    private const uint MOUSEEVENTF_MOVE = 0x01;
    private const uint MOUSEEVENTF_WHEEL = 0x0800;

    private GameObject crosshair;
    private float clickCooldown = 0.5f;
    private float lastClickTime;

    [SerializeField] private float sens = 2f; 
    
    private void Awake()
    {
        crosshair = GameObject.Find("Crosshair");
        Cursor.visible = false;
    }

    void Update()
    {
        crosshair.transform.position = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.Space)  && Time.time - lastClickTime > clickCooldown)
        {
            lastClickTime = Time.time;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 120, 0);
        }else if (Input.GetKey(KeyCode.X))
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, unchecked((uint)-120), 0);
        }else
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 0, 0);
        }
        
        float emulatedMouseX = Input.GetAxisRaw("Emulated Mouse X") * sens;
        float emulatedMouseY = Input.GetAxisRaw("Emulated Mouse Y") * sens;
        
        if (crosshair != null)
        {
            if (emulatedMouseX != 0 || emulatedMouseY != 0)
            {
                mouse_event(MOUSEEVENTF_MOVE, (int)emulatedMouseX, (int)emulatedMouseY, 0, 0);
            }
        }
    }
}
