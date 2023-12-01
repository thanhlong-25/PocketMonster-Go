using System;
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

    [SerializeField] SkillAnimation playerSkillAnimation;
    [SerializeField] SkillAnimation enemySkillAnimation;

    BattleState state;
    int currentAction;
    int currentSkill;

    public event Action<bool> OnBattleOver;

    public void StartBattle() {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle() {
        playerUnit.Setup();
        playerHud.setData(playerUnit.Pkm);
        enemyUnit.Setup();
        enemyHud.setData(enemyUnit.Pkm);
        dialogBox.SetSkillName(playerUnit.Pkm.Skills);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pkm.PkmBase.Name} appeared !!!");

        PlayerAction();
    }

    void PlayerAction() {
        state = BattleState.PLAYER_ACTION;
        StartCoroutine(dialogBox.TypeDialog("Choose an action !!!"));
        dialogBox.EnabledActionSelector(true);
    }

    IEnumerator PerformPlayerSkill() {
        state = BattleState.BUSY;

        var skill = playerUnit.Pkm.Skills[currentSkill];
        yield return dialogBox.TypeDialog($"{playerUnit.Pkm.PkmBase.Name} use {skill.SkillBase.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        enemyUnit.PlayHitAnimation();

        StartCoroutine(playerSkillAnimation.PlaySkillAnimation(playerUnit.Pkm.Skills[currentSkill].SkillBase.SkillAnimationBase.SkillFrame));
        yield return new WaitForSeconds(1f);

        var damageDetails = enemyUnit.Pkm.TakeDamage(skill, playerUnit.Pkm);
        yield return enemyHud.UpdateHPBar();
        yield return ShowDamageDetails(damageDetails);

        yield return new WaitForSeconds(1f);
        if(damageDetails.isFainted) {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pkm.PkmBase.Name} fainted");
            enemyUnit.PlayFaintedAnimation();
            
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        } else {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove(){
        state = BattleState.ENEMY_SKILL;

        var skill = enemyUnit.Pkm.GetRandomSkill();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pkm.PkmBase.Name} use {skill.SkillBase.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerUnit.PlayHitAnimation();

        StartCoroutine(enemySkillAnimation.PlaySkillAnimation(enemyUnit.Pkm.Skills[currentSkill].SkillBase.SkillAnimationBase.SkillFrame));
        yield return new WaitForSeconds(0.5f);

        var damageDetails = playerUnit.Pkm.TakeDamage(skill, enemyUnit.Pkm);
        yield return playerHud.UpdateHPBar();
        yield return ShowDamageDetails(damageDetails);

        yield return new WaitForSeconds(1f);
        if(damageDetails.isFainted) {
            yield return dialogBox.TypeDialog($"{playerUnit.Pkm.PkmBase.Name} fainted");
            playerUnit.PlayFaintedAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        } else {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails) {
        if(damageDetails.Critical > 1f) {
            yield return dialogBox.TypeDialog("A critical hit !!!");
        }

        if(damageDetails.TypeEffectiveness > 1f) {
            yield return dialogBox.TypeDialog("It's super effective !!!");
        } else if (damageDetails.TypeEffectiveness < 1f) {
             yield return dialogBox.TypeDialog("It's not effective !!!");
        }
    }

    void PlayerMove() {
        state = BattleState.PLAYER_SKILL;
        dialogBox.EnabledActionSelector(false);
        dialogBox.EnabledDialogText(false);
        dialogBox.EnabledSkillSelector(true);
    }

    public void HandleUpdate() {
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
            if (currentSkill < playerUnit.Pkm.Skills.Count - 2) {
                currentSkill += 2;
            }
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentSkill > 1) {
                currentSkill -= 2;
            }
        }

        dialogBox.UpdateSkillSelection(currentSkill, playerUnit.Pkm.Skills[currentSkill]);

         if(Input.GetKeyDown(KeyCode.Z)) {
            dialogBox.EnabledSkillSelector(false);
            dialogBox.EnabledDialogText(true);
            StartCoroutine(PerformPlayerSkill());
        }
    }
}
