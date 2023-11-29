using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] private Sprite[] frameArr;
    private int currentFrame;
    private float timer;
    private float frameRate = 0.1f;

    private void Update() {
        timer += Time.deltaTime;

        if(timer >= frameRate) {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % frameArr.Length;
            gameObject.GetComponent<SpriteRenderer>().sprite = frameArr[currentFrame];
        }
    }
}
