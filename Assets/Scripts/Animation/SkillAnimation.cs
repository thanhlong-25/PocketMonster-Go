using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimation  : MonoBehaviour {
    SpriteRenderer spriteRender;

    private void Awake() {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    public IEnumerator PlaySkillAnimation(Sprite[] skillSprite) {
        for (int i = 0; i < skillSprite.Length; i++) {
            spriteRender.sprite = skillSprite[i];
            yield return new WaitForSeconds(0.1f);
        }
    }
}
