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
    Pokemon _pokemon;

   public void setData(Pokemon pokemon) {
         _pokemon = pokemon;
         nameText.text = pokemon.PkmBase.Name;
         levelText.text = "Lv." + pokemon.Level;
         hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);

         type_1.sprite = pokemon.PkmBase.Type_1_Sprite;
         type_1.enabled = true;
         if(pokemon.PkmBase.Type_2 != PokemonType.NONE) {
               type_2.sprite = pokemon.PkmBase.Type_2_Sprite;
               type_2.enabled = true;
         }
   }

   public IEnumerator UpdateHPBar() {
      // int curHp = _pokemon.HP;
      // Debug.Log(curHp);
      // int maxHP = _pokemon.MaxHp;
      // Debug.Log(maxHP);

      // int tinhtien = 0;

      // while (tinhtien != maxHP - curHp) {
      //       hpText.text = $"{curHp} / {maxHP}";
      //       //yield return new WaitForSeconds(1f);
      //       curHp--;
      //       tinhtien++;
      //       yield return hpBar.SetHPSmooth((float)curHp / maxHP);
      // }

      // // Khi HP giảm xuống 0, cập nhật văn bản và thanh HP cuối cùng
      // //hpText.text = $"{hpText} / {maxHP}";
      // //yield return hpBar.SetHPSmooth((float)curHp / maxHP);

      //
      yield return hpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.MaxHp);
   }
}
