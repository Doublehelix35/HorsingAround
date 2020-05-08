using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public void UpdateFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
