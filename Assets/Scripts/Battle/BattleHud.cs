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
      [SerializeField] Text hpText;
      Pokemon _pokemon;

      public void setData(Pokemon pokemon) {
            _pokemon = pokemon;
            nameText.text = pokemon.PkmBase.Name;
            levelText.text = "Lv." + pokemon.Level;
            hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
            hpText.text = $"{pokemon.HP} / {pokemon.MaxHp}";

            type_1.sprite = pokemon.PkmBase.Type_1_Sprite;
            type_1.enabled = true;
            if(pokemon.PkmBase.Type_2 != PokemonType.NONE) {
                  type_2.sprite = pokemon.PkmBase.Type_2_Sprite;
                  type_2.enabled = true;
            }
      }

      public IEnumerator UpdateHPBar() {
            //yield return hpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.MaxHp);
            //yield return UpdateHpText();
            IEnumerator smoothHP = hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            StartCoroutine(smoothHP);

            yield return StartCoroutine(UpdateHpText());
      }

      public IEnumerator UpdateHpText() {
            int curHp = _pokemon.HP;
            int maxHP = _pokemon.MaxHp;
            int tempHp = int.Parse(hpText.text.Split('/')[0]);

            while (tempHp >= curHp) {
                  hpText.text = $"{tempHp} / {maxHP}";
                  yield return new WaitForSeconds(0.2f);
                  tempHp--;
            }
      }
}
