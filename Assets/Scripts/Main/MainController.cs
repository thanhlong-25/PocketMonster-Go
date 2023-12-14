using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    FREE_ROAM,
    BATTLE,
    CHATTING
}

public class MainController : MonoBehaviour {
    GameState state;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    private void Awake() {
        ConditionsDB.Init();
    }

    private void Start() {
        playerController.OnEncoutered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogController.Instance.OnShowDialog += () => {
            state = GameState.CHATTING;
        };

        DialogController.Instance.OnCloseDialog += () => {
            if(state == GameState.CHATTING) {
                state = GameState.FREE_ROAM;
            }
        };
    }

    void StartBattle() {
        state = GameState.BATTLE;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    void EndBattle(bool won) {
        state = GameState.FREE_ROAM;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update() {
        if(state == GameState.FREE_ROAM) {
            playerController.HandleUpdate();
        } else if (state == GameState.BATTLE) {
            battleSystem.HandleUpdate();
        } else if (state == GameState.CHATTING) {
            DialogController.Instance.HandleUpdate();
        }
    }
}
