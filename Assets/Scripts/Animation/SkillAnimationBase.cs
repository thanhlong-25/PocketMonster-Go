using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillAnimation", menuName = "Pokemon/Create new skill animation")]
public class SkillAnimationBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] private Sprite[] skillFrame;
    private float frameRate = 0.15f;

    public string Name {
        get { return name; }
    }

    public Sprite[] SkillFrame {
        get { return skillFrame; }
    }

    public float FrameRate {
        get { return frameRate; }
    }
}
