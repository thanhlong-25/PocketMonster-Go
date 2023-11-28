using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Đã chạm vào đối tượng văn bản: " + gameObject.name);
        // Thực hiện hành động khi chạm vào văn bản
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Ngừng chạm vào đối tượng văn bản: " + gameObject.name);
        // Thực hiện hành động khi ngừng chạm vào văn bản
    }
}
