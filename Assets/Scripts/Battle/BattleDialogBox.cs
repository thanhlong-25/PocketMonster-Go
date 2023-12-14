using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour {
    [SerializeField] Color highlightedColor;
    [SerializeField] Text dialogText;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject skillDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> skillsText;

    [SerializeField] Text skillPPText;
    [SerializeField] Text skillTypeText;

    public void SetDialog(string dialog) {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog) {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray()) {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / Constants.LETTER_PER_SECOND);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnabledDialogText(bool enabled) {
        dialogText.enabled = enabled;
    }

    public void EnabledActionSelector(bool enabled) {
        actionSelector.SetActive(enabled);
    }

    public void EnabledSkillSelector(bool enabled) {
        skillSelector.SetActive(enabled);
        skillDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction) {
        for (int i = 0; i < actionTexts.Count; ++i) {
            if (i == selectedAction) {
                actionTexts[i].color = highlightedColor;
            } else {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateSkillSelection(int selectedSkill, Skill skill) {
        for (int i = 0; i < skillsText.Count; ++i) {
            if (i == selectedSkill) {
                skillsText[i].color = highlightedColor;
            } else {
                skillsText[i].color = Color.black;
            }
        }

        skillPPText.text = $"PP {skill.timesCanUse}/{skill.SkillBase.TimesCanUse}";
        skillTypeText.text = skill.SkillBase.Type.ToString();

        if(skill.timesCanUse == 0) {
            skillPPText.color = Color.red;
        } else if(skill.timesCanUse <= skill.SkillBase.TimesCanUse / 2) {
            skillPPText.color = new Color(1f, 0.647f, 0f);
        } else {
            skillPPText.color = Color.black;
        }
    }

    public void SetSkillName(List<Skill> skills) {
        for (int i = 0; i < skillsText.Count; ++i) {
            if(i < skills.Count) {
                skillsText[i].text = skills[i].SkillBase.Name;
            } else {
                skillsText[i].text = "-";
            }
        }
    }
}
