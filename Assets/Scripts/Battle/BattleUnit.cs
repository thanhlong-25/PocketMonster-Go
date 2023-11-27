using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour {
    [SerializeField] PokemonBase pkmBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pkm { get; set; }

    public void Setup() {
        Pkm = new Pokemon(pkmBase, level);

        if (isPlayerUnit) {
            GetComponent<Image>().sprite = Pkm.PkmBase.BackSprite;
        } else {
            GetComponent<Image>().sprite = Pkm.PkmBase.FrontSprite;
        }
    }
}
