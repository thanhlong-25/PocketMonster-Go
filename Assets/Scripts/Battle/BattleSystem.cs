using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {
    START,
    ACTION_SELECTION,
    SKILL_SELECTION,
    PERFORM_SKILL,
    BUSY,
    PARTY_SCREEN,
    BATTLE_OVER
}

public class BattleSystem : MonoBehaviour {
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] SkillAnimation playerSkillAnimation;
    [SerializeField] SkillAnimation enemySkillAnimation;

    BattleState state;
    int currentAction;
    int currentSkill;
    int currentMember;

    public event Action<bool> OnBattleOver;
    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon) {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle() {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        dialogBox.SetSkillName(playerUnit.Pkm.Skills);
        partyScreen.Init();

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pkm.PkmBase.Name} appeared !!!");
        PlayerActionSelection();
    }

    void PlayerActionSelection() {
        state = BattleState.ACTION_SELECTION;
        dialogBox.SetDialog("Choose an action !!!");
        dialogBox.EnabledActionSelector(true);
    }

    IEnumerator PlayerSkill() {
        state = BattleState.PERFORM_SKILL;
        var skill = playerUnit.Pkm.Skills[currentSkill];
        yield return RunSkill(playerUnit, enemyUnit, skill, playerUnit.IsPlayerUnit);

        // If the battle state was not changed by RunMove, then go to next step
        if(state == BattleState.PERFORM_SKILL) {
            StartCoroutine(EnemySkill());
        }

    }

    IEnumerator EnemySkill(){
        state = BattleState.PERFORM_SKILL;
        var skill = enemyUnit.Pkm.GetRandomSkill();
        yield return RunSkill(enemyUnit, playerUnit, skill, enemyUnit.IsPlayerUnit);
        
        // If the battle state was not changed by RunMove, then go to next step
        if(state == BattleState.PERFORM_SKILL) {
            PlayerActionSelection();
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
        state = BattleState.SKILL_SELECTION;
        dialogBox.EnabledActionSelector(false);
        dialogBox.EnabledDialogText(false);
        dialogBox.EnabledSkillSelector(true);
    }

    public void HandleUpdate() {
        if (state == BattleState.ACTION_SELECTION) {
            HandleActionSelection();
        } else if (state == BattleState.SKILL_SELECTION) {
            HandleSkillSelection();
        } else if (state == BattleState.PARTY_SCREEN) {
            HandlePartyMemberSelection();
        }
    }

    void HandleActionSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++currentAction;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --currentAction;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentAction += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Z)) {
            if (currentAction == 0) {
                // Pokemon
                OpenPartyScreen();
            } else if (currentAction == 1) {
                PlayerMove();
                // Fight
            } else if (currentAction == 2) {
                // Run
            } else if (currentAction == 3) {
                // Bag
            }
        }
    }

    void HandleSkillSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++currentSkill;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --currentSkill;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentSkill += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentSkill -= 2;
        }

        currentSkill = Mathf.Clamp(currentSkill, 0, playerUnit.Pkm.Skills.Count - 1);

        dialogBox.UpdateSkillSelection(currentSkill, playerUnit.Pkm.Skills[currentSkill]);

        if(Input.GetKeyDown(KeyCode.Z)) {
            dialogBox.EnabledSkillSelector(false);
            dialogBox.EnabledDialogText(true);
            StartCoroutine(PlayerSkill());
        } else if (Input.GetKeyDown(KeyCode.X)) { // Cancel
            dialogBox.EnabledSkillSelector(false);
            dialogBox.EnabledDialogText(true);
            PlayerActionSelection();
        }
    }

    void OpenPartyScreen() {
        state = BattleState.PARTY_SCREEN;
        partyScreen.SetPartyMemberData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void HandlePartyMemberSelection() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ++currentMember;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            --currentMember;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentMember += 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);
        partyScreen.UpdateMemberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.Z)) {
            var selectedMember = playerParty.Pokemons[currentMember];

            if(selectedMember.HP <= 0) {
                partyScreen.SetMessageText(selectedMember.PkmBase.Name + " fainted !!!");
                return;
            } else if(selectedMember == playerUnit.Pkm) {
                partyScreen.SetMessageText(selectedMember.PkmBase.Name + " is already on battle !!!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.BUSY;
            StartCoroutine(SwitchPokemon(selectedMember));

        } else if (Input.GetKeyDown(KeyCode.X)) {
            partyScreen.gameObject.SetActive(false);
            PlayerActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon) {
        dialogBox.EnabledActionSelector(false);
        if(playerUnit.Pkm.HP > 0) {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pkm.PkmBase.Name} !!!");
            playerUnit.PlayFaintedAnimation();

            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetSkillName(newPokemon.Skills);

        yield return dialogBox.TypeDialog($"Go {newPokemon.PkmBase.Name} !!!");

        StartCoroutine(EnemySkill());
    }

    IEnumerator RunSkill(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill, bool isPlayerUnit) {
        skill.timesCanUse--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pkm.PkmBase.Name} use {skill.SkillBase.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();

        if(isPlayerUnit) StartCoroutine(playerSkillAnimation.PlaySkillAnimation(sourceUnit.Pkm.Skills[currentSkill].SkillBase.SkillAnimationBase.SkillFrame));
        if(!isPlayerUnit) StartCoroutine(enemySkillAnimation.PlaySkillAnimation(sourceUnit.Pkm.Skills[currentSkill].SkillBase.SkillAnimationBase.SkillFrame));
        yield return new WaitForSeconds(1f);

        var damageDetails = targetUnit.Pkm.TakeDamage(skill, sourceUnit.Pkm);
        yield return targetUnit.Hud.UpdateHPBar();
        yield return ShowDamageDetails(damageDetails);

        yield return new WaitForSeconds(1f);
        if(damageDetails.isFainted) {
            yield return dialogBox.TypeDialog($"{targetUnit.Pkm.PkmBase.Name} fainted");
            targetUnit.PlayFaintedAnimation();

            yield return new WaitForSeconds(2f);
            CheckForBattleOver(targetUnit);
        }
    }

    void CheckForBattleOver(bool isPlayerUnit) {
        if(isPlayerUnit) {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon != null) {
                OpenPartyScreen();
            } else {
                BattleOver(false);
            }
        } else  {
            BattleOver(true);
        }
    }

    void BattleOver(bool isWon) {
        state = BattleState.BATTLE_OVER;
        OnBattleOver(isWon);
    }

}
