using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimation  : MonoBehaviour {
    SpriteRenderer spriteRender;

    private void Awake() {
        spriteRender = GetComponent<SpriteRenderer>();
        spriteRender.enabled = false;
    }

    public IEnumerator PlaySkillAnimation(Sprite[] skillSprite) {
        spriteRender.enabled = true;
        for (int i = 0; i < skillSprite.Length; i++) {
            spriteRender.sprite = skillSprite[i];
            if(i == skillSprite.Length - 1) {
                spriteRender.enabled = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
