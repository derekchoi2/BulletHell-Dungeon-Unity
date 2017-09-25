using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

	public Image bgImg;
	public Image joystickImg;

	private Vector3 InputVec = Vector3.zero;

	void start(){
		InputVec = Vector3.zero;
	}

	public virtual void OnDrag(PointerEventData ped){
		MoveJoystick (ped);

	}

	public virtual void OnPointerDown(PointerEventData ped){
		MoveJoystick (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped){
		InputVec = Vector3.zero;
		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
	}

	void MoveJoystick(PointerEventData ped){
		Vector2 pos = Vector2.zero;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
			bgImg.rectTransform, 
			ped.position, 
			ped.pressEventCamera, 
			out pos)){

			pos.x = pos.x / bgImg.rectTransform.sizeDelta.x;
			pos.y = pos.y / bgImg.rectTransform.sizeDelta.y;
			float x;
			if (bgImg.rectTransform.pivot.x == 1)
				x = pos.x * 2 + 1;
			else
				x = pos.x * 2 - 1;

			float y;
			if (bgImg.rectTransform.pivot.y == 1)
				y = pos.y * 2 + 1;
			else
				y = pos.y * 2 - 1;

			InputVec = new Vector3 (x, 0, y);
			if (InputVec.magnitude > 1)
				InputVec = InputVec.normalized;

			joystickImg.rectTransform.anchoredPosition = new Vector3 (InputVec.x * (bgImg.rectTransform.sizeDelta.x / 3),
				InputVec.z * (bgImg.rectTransform.sizeDelta.y / 3));

		}
	}

	public Vector3 GetDiscrete(){
		float x = InputVec.x;
		float z = InputVec.z;

		if (x >= 0.5)
			x = 1;
		else if (x <= -0.5)
			x = -1;
		else
			x = 0;

		if (z >= 0.5)
			z = 1;
		else if (z <= -0.5)
			z = -1;
		else
			z = 0;
		
		return new Vector3 (x, 0, z);
	}
}
