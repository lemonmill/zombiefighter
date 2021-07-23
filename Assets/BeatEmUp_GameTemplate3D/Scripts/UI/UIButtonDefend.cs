using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonDefend : UIButton
{
    public RectTransform PunchButtonRect;
    public RectTransform KickButtonRect;

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (inputmanager == null) return;


        base.OnPointerUp(eventData);


        if (RectTransformUtility.RectangleContainsScreenPoint(PunchButtonRect, eventData.position))
        {
            inputmanager.OnTouchScreenInputEvent("DefendPunch", BUTTONSTATE.RELEASE);
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(KickButtonRect, eventData.position))
        {
            inputmanager.OnTouchScreenInputEvent("DefendKick", BUTTONSTATE.RELEASE);
        }
    }
}