using UnityEngine;
using UnityEngine.EventSystems;
  
public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	
	public string actionDown = "";
	public string actionUp = "";
	protected InputManager inputmanager;

	private void Update(){
		if(inputmanager == null) inputmanager = GameObject.FindObjectOfType<InputManager>();
	}

	public void OnPointerDown(PointerEventData eventData){
		if(inputmanager != null && actionDown != "") inputmanager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
	}

	public virtual void OnPointerUp(PointerEventData eventData){
		if(inputmanager != null && actionUp != "") inputmanager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
	}
}