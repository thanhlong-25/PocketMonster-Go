using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour {
   [SerializeField] Text nameText;
   [SerializeField] Text levelText;
   [SerializeField] HpBar hpBar;
   [SerializeField] Image type_1;
   [SerializeField] Image type_2;

   public void setData(Pokemon pokemon) {
         nameText.text = pokemon.PkmBase.Name;
         levelText.text = "Lvl " + pokemon.Level;
         hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);

         type_1.sprite = pokemon.PkmBase.Type_1_Sprite;
         type_1.enabled = true;
         if(pokemon.PkmBase.Type_2 != PokemonType.NONE) {
               type_2.sprite = pokemon.PkmBase.Type_2_Sprite;
               type_2.enabled = true;
         }
   }
}
