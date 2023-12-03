using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {
    START,
    PLAYER_ACTION,
    PLAYER_SKILL,
    ENEMY_SKILL,
    BUSY,
    PARTY_SCREEN
}

public class BattleSystem : MonoBehaviour {
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
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
        playerHud.setData(playerUnit.Pkm);
        enemyHud.setData(enemyUnit.Pkm);
        dialogBox.SetSkillName(playerUnit.Pkm.Skills);

        partyScreen.Init();

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pkm.PkmBase.Name} appeared !!!");

        PlayerAction();
    }

    void PlayerAction() {
        state = BattleState.PLAYER_ACTION;
        dialogBox.SetDialog("Choose an action !!!");
        dialogBox.EnabledActionSelector(true);
    }

    IEnumerator PerformPlayerSkill() {
        state = BattleState.BUSY;

        var skill = playerUnit.Pkm.Skills[currentSkill];
        skill.timesCanUse--;
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
        skill.timesCanUse--;
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

            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon != null) {
                OpenPartyScreen();
            } else {
                OnBattleOver(false);
            }
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
            StartCoroutine(PerformPlayerSkill());
        } else if (Input.GetKeyDown(KeyCode.X)) { // Cancel
            dialogBox.EnabledSkillSelector(false);
            dialogBox.EnabledDialogText(true);
            PlayerAction();
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
            PlayerAction();
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
        playerHud.setData(newPokemon);
        dialogBox.SetSkillName(newPokemon.Skills);

        yield return dialogBox.TypeDialog($"Go {newPokemon.PkmBase.Name} !!!");

        StartCoroutine(EnemyMove());
    }

}
