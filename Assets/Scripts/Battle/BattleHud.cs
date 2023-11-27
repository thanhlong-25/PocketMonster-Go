using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour {
   [SerializeField] Text nameText;
   [SerializeField] Text levelText;
   [SerializeField] HpBar hpBar;

   public void setData(Pokemon pokemon) {
        nameText.text = pokemon.PkmBase.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
   }
}
