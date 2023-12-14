using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable {
    [SerializeField] Dialog dialogBox;

    public void Interact() {
        StartCoroutine(DialogController.Instance.ShowDialog(dialogBox));
    }
}
