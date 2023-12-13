using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {
    START,
    ACTION_SELECTION,
    SKILL_SELECTION,
    RUNNING_TURN,
    BUSY,
    PARTY_SCREEN,
    BATTLE_OVER
}

public enum BattleAction {
    SKILL,
    SWITCH_POKEMON,
    USE_ITEM,
    RUN
}

public class BattleSystem : MonoBehaviour {
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] SkillAnimation playerSkillAnimation;
    [SerializeField] SkillAnimation enemySkillAnimation;

    BattleState state;
    BattleState? prevState;
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

    IEnumerator RunTurns(BattleAction playerAction) {
        state = BattleState.RUNNING_TURN;

        if(playerAction == BattleAction.SKILL) {
            playerUnit.Pkm.CurrentSkill = playerUnit.Pkm.Skills[currentSkill];
            enemyUnit.Pkm.CurrentSkill = enemyUnit.Pkm.GetRandomSkill();

            int playerMovePriority = playerUnit.Pkm.CurrentSkill.SkillBase.Priority;
            int enemyMovePriority = enemyUnit.Pkm.CurrentSkill.SkillBase.Priority;
            //check who goes first
            bool playerGoFirst = true;
            if(enemyMovePriority > playerMovePriority) {
                playerGoFirst = false;
            } else if(enemyMovePriority == playerMovePriority){
                playerGoFirst = playerUnit.Pkm.Speed >= enemyUnit.Pkm.Speed;
            }
            var firstUnit = (playerGoFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoFirst) ? enemyUnit : playerUnit;
            var secondPokemon = secondUnit.Pkm;

            // first turn
            yield return RunSkill(firstUnit, secondUnit, firstUnit.Pkm.CurrentSkill, firstUnit.IsPlayerUnit);
            yield return RunAfterTurn(firstUnit);
            if(state == BattleState.BATTLE_OVER) yield break;

            // second turn
            if(secondPokemon.HP > 0) {
                yield return RunSkill(secondUnit, firstUnit, secondUnit.Pkm.CurrentSkill, secondUnit.IsPlayerUnit);
                yield return RunAfterTurn(secondUnit);
                if(state == BattleState.BATTLE_OVER) yield break;
            }
        } else {
            if(playerAction == BattleAction.SWITCH_POKEMON) {
                var selectedMember = playerParty.Pokemons[currentMember];
                state = BattleState.BUSY;
                yield return SwitchPokemon(selectedMember);
            }

            // Enemy Turn
            var enemySkill = enemyUnit.Pkm.GetRandomSkill();
            yield return RunSkill(enemyUnit, playerUnit, enemySkill, false);
            yield return RunAfterTurn(enemyUnit);
            if(state == BattleState.BATTLE_OVER) yield break;
        }

        if(state != BattleState.BATTLE_OVER) {
            PlayerActionSelection();
        }
    }

    void PlayerActionSelection() {
        state = BattleState.ACTION_SELECTION;
        dialogBox.SetDialog("Choose an action !!!");
        dialogBox.EnabledActionSelector(true);
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
                prevState = state;
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
            var skill = playerUnit.Pkm.Skills[currentSkill];
            if(skill.timesCanUse == 0) return;
            dialogBox.EnabledSkillSelector(false);
            dialogBox.EnabledDialogText(true);
            StartCoroutine(RunTurns(BattleAction.SKILL));
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
            if(prevState == BattleState.ACTION_SELECTION) {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SWITCH_POKEMON));
            } else {
                state = BattleState.BUSY;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
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

        state = BattleState.RUNNING_TURN;
    }

    IEnumerator RunSkill(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill, bool isPlayerUnit) {
        bool isCanSkill = sourceUnit.Pkm.OnBeforeSkill();
        if(!isCanSkill) {
            yield return ShowStatusChanges(sourceUnit.Pkm);
            yield return sourceUnit.Hud.UpdateHPBar();
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit.Pkm);
        skill.timesCanUse--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pkm.PkmBase.Name} use {skill.SkillBase.Name}");

        if(CheckIfSkillHits(skill, sourceUnit.Pkm, targetUnit.Pkm)) {
            // SKill Animation
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if(isPlayerUnit) StartCoroutine(playerSkillAnimation.PlaySkillAnimation(sourceUnit.Pkm.Skills[currentSkill].SkillBase.SkillAnimationBase.SkillFrame));
            if(!isPlayerUnit) StartCoroutine(enemySkillAnimation.PlaySkillAnimation(sourceUnit.Pkm.Skills[currentSkill].SkillBase.SkillAnimationBase.SkillFrame));
            yield return new WaitForSeconds(1f);

            if(skill.SkillBase.Category == SkillCategory.STATUS) {
                yield return RunSkillEffect(skill.SkillBase.Effect, sourceUnit.Pkm, targetUnit.Pkm, skill.SkillBase.Target);
            } else {
                var damageDetails = targetUnit.Pkm.TakeDamage(skill, sourceUnit.Pkm);
                yield return targetUnit.Hud.UpdateHPBar();
                yield return ShowDamageDetails(damageDetails);
                yield return new WaitForSeconds(1f);
            }

            if(skill.SkillBase.SecondaryEffects != null && skill.SkillBase.SecondaryEffects.Count > 0 && targetUnit.Pkm.HP > 0) {
                foreach (var secondary in skill.SkillBase.SecondaryEffects) {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if(rnd <= secondary.Chance) {
                        yield return RunSkillEffect(secondary, sourceUnit.Pkm, targetUnit.Pkm, secondary.Target);
                    }
                }
            }

            if(targetUnit.Pkm.HP <= 0) {
            //if(damageDetails.isFainted) {
                yield return dialogBox.TypeDialog($"{targetUnit.Pkm.PkmBase.Name} fainted");
                targetUnit.PlayFaintedAnimation();
                yield return new WaitForSeconds(2f);
                CheckForBattleOver(targetUnit);
            }
        } else {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pkm.PkmBase.Name} missed!!!");
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit) {
        if(state == BattleState.BATTLE_OVER) yield break;
        yield return new WaitUntil(() => state == BattleState.RUNNING_TURN);

        // vì sao lại là sourceUnit -> Kiểu handle sau khi mày xài skill, nếu đang burn thì sẽ bị đốt
        sourceUnit.Pkm.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pkm);
        yield return sourceUnit.Hud.UpdateHPBar();

        if(sourceUnit.Pkm.HP <= 0) {
        //if(damageDetails.isFainted) {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pkm.PkmBase.Name} fainted");
            sourceUnit.PlayFaintedAnimation();
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunSkillEffect(SkillEffect effect, Pokemon source, Pokemon target, SkillTarget skillTarget) {
        // Stat Boosting
        if(effect.Boosts != null) {
            if(skillTarget == SkillTarget.SELF) {
                source.ApplyBoosts(effect.Boosts);
            } else {
                target.ApplyBoosts(effect.Boosts);
            }
        }

        // Status Condition
        if(effect.Status != ConditionID.none) {
            target.SetStatus(effect.Status);
        }

        // Status Condition
        if(effect.VolatileStatus != ConditionID.none) {
            target.SetVolatileStatus(effect.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon) {
        while (pokemon.StatusChanges.Count > 0) {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
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

    bool CheckIfSkillHits(Skill skill, Pokemon sourceUnit, Pokemon targetUnit) {
        if(skill.SkillBase.AlwaysHits) return true;

        float skillAccuracy = skill.SkillBase.Accuracy;
        int accuracy = sourceUnit.StatBoosts[Stat.ACCURACY];
        int evasion = targetUnit.StatBoosts[Stat.EVASION];
        var boostValues = new float[]  { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if(accuracy > 0) {
            skillAccuracy *= boostValues[accuracy];
        } else {
            skillAccuracy /= boostValues[-accuracy];
        }

         if(evasion > 0) {
            skillAccuracy /= boostValues[evasion];
        } else {
            skillAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= skillAccuracy;
    }

    void BattleOver(bool isWon) {
        state = BattleState.BATTLE_OVER;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(isWon);
    }

}
