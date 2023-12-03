using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour {
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Image type_1;
    [SerializeField] Image type_2;
    [SerializeField] Text hpText;
    [SerializeField] Image pokemon_3D;

    [SerializeField] Image background;
    [SerializeField] Sprite defaultbackground;
    [SerializeField] Sprite avaiableBackground;
    [SerializeField] Sprite unavailableBackground;
    Pokemon _pokemon;

    public void setData(Pokemon pokemon) {
            _pokemon = pokemon;
            nameText.text = pokemon.PkmBase.Name;
            levelText.text = "Lv." + pokemon.Level;
            hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
            hpText.text = $"{pokemon.HP} / {pokemon.MaxHp}";

            pokemon_3D.sprite = pokemon.PkmBase.Sprite_3D;
            type_1.sprite = pokemon.PkmBase.Type_1_Sprite;
            type_1.enabled = true;
            if(pokemon.PkmBase.Type_2 != PokemonType.NONE) {
                  type_2.sprite = pokemon.PkmBase.Type_2_Sprite;
                  type_2.enabled = true;
            }
      }

      public void SetSelected(bool selected) {
            if(selected) {
                  if(_pokemon.HP == 0) background.sprite = unavailableBackground;
                  else background.sprite = avaiableBackground;
            } else {
                  background.sprite = defaultbackground;
            }
      }
}
