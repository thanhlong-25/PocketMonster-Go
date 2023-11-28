using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {
    public SkillBase SkillBase { get; set; }
    public int timesCanUse { get; set; }

    public Skill(SkillBase pBase) {
        SkillBase = pBase;
        timesCanUse = pBase.TimesCanUse;
    }
}
