using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] private Sprite[] frameArr;
    private int currentFrame;
    private float frameRate = 0.15f;

    private void Start()
    {
        StartCoroutine(AnimateOnce());
    }

    private System.Collections.IEnumerator AnimateOnce()
    {
        for (int i = 0; i < frameArr.Length; i++)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = frameArr[i];
            yield return new WaitForSeconds(frameRate);
        }
    }
}
