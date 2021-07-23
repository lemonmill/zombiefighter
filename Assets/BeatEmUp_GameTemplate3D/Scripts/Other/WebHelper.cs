using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebHelper : MonoBehaviour
{
    public void  OpenURLButton(string URL) => OpenURL(URL);
    public static void OpenURL(string URL) => Application.OpenURL(URL);
}
