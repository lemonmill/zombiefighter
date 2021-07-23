using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Image image = GetComponent<Image>();
        RectTransform rectTransform = (RectTransform) transform;

        float aspectRatio = image.sprite.textureRect.width / image.sprite.textureRect.height;
        
        if (aspectRatio>1)
            rectTransform.localScale = new Vector3(aspectRatio/2, 1, 1);
        else
            rectTransform.localScale = new Vector3(1, 1 / aspectRatio/2, 1);
    }
}