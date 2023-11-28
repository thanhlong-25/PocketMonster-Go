using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {
    START,
    PLAYER_ACTION,
    PLAYER_SKILL,
    ENEMY_SKILL,
    BUSY
}

public class BattleSystem : MonoBehaviour {
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentSkill;

    private void Start() {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle() {
        playerUnit.Setup();
        playerHud.setData(playerUnit.Pkm);
        enemyUnit.Setup();
        enemyHud.setData(enemyUnit.Pkm);
        dialogBox.SetSkillName(playerUnit.Pkm.Skills);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pkm.PkmBase.Name} appeared !!!");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction() {
        state = BattleState.PLAYER_ACTION;
        StartCoroutine(dialogBox.TypeDialog("Choose an action !!!"));
        dialogBox.EnabledActionSelector(true);
    }

    void PlayerMove() {
        state = BattleState.PLAYER_SKILL;
        dialogBox.EnabledActionSelector(false);
        dialogBox.EnabledDialogText(false);
        dialogBox.EnabledSkillSelector(true);
    }

    private void Update() {
        if (state == BattleState.PLAYER_ACTION) {
            HandleActionSelection();
        } else if (state == BattleState.PLAYER_SKILL) {
            HandleSkillSelection();
        }
    }

    void HandleActionSelection() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentAction < 1) {
                ++currentAction;
            }
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentAction > 0) {
                --currentAction;
            }
        }
        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Z)) {
            if (currentAction == 0) {
                // Fight
                PlayerMove();
            } else if (currentAction == 1) {
                // Run
                //Camera.Stop();
            }

        }
    }

    void HandleSkillSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (currentSkill < playerUnit.Pkm.Skills.Count - 1) {
                ++currentSkill;
            }
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (currentSkill > 0) {
                --currentSkill;
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentSkill > 0) {
                currentSkill += 2;
            }
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentSkill > 1) {
                currentSkill -= 2;
            }
        }

        dialogBox.UpdateSkillSelection(currentSkill, playerUnit.Pkm.Skills[currentSkill]);

    }
}
