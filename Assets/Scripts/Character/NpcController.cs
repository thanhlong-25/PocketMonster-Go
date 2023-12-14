using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable {
    [SerializeField] Dialog dialogBox;
    [SerializeField] List<Sprite> sprites;

    SpriteAnimator spriteAnimator;

    private void Start() {
        spriteAnimator = new SpriteAnimator(sprites, GetComponent<SpriteRenderer>());
        spriteAnimator.Start();
    }

    private void HandleUpdate() {
        spriteAnimator.HandleUpdate();
    }

    public void Interact() {
        StartCoroutine(DialogController.Instance.ShowDialog(dialogBox));
    }
}
