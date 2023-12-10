using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour {
      [SerializeField] Text nameText;
      [SerializeField] Text levelText;
      [SerializeField] Text statusText;
      [SerializeField] HpBar hpBar;
      [SerializeField] Text hpText;
      [SerializeField] Color psnColor;
      [SerializeField] Color parColor;
      [SerializeField] Color slpColor;
      [SerializeField] Color brnColor;
      [SerializeField] Color frzColor;
      [SerializeField] Color cfsColor;

      Dictionary<ConditionID, Color> statusColors;

      Pokemon _pokemon;

      public void setData(Pokemon pokemon) {
            _pokemon = pokemon;
            nameText.text = pokemon.PkmBase.Name;
            levelText.text = "Lv." + pokemon.Level;
            hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
            hpText.text = $"{pokemon.HP} / {pokemon.MaxHp}";

            statusColors = new Dictionary<ConditionID, Color>() {
                  {ConditionID.psn, psnColor},
                  {ConditionID.par, parColor},
                  {ConditionID.slp, slpColor},
                  {ConditionID.frz, frzColor},
                  {ConditionID.brn, brnColor},
                  {ConditionID.cfs, cfsColor},
            };
            SetStatusText();
            _pokemon.OnStatusChanged += SetStatusText;
      }

      public IEnumerator UpdateHPBar() {
            if(_pokemon.HpChanged) {
                  IEnumerator smoothHP = hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
                  StartCoroutine(smoothHP);
                  yield return StartCoroutine(UpdateHpText());
            }
      }

      public IEnumerator UpdateHpText() {
            int curHp = _pokemon.HP;
            int maxHP = _pokemon.MaxHp;
            int damage = maxHP - curHp;
            int tempHp = int.Parse(hpText.text.Split('/')[0]);

            while (tempHp >= curHp) {
                  hpText.text = $"{tempHp} / {maxHP}";
                  if(damage <= 5) yield return new WaitForSeconds(0.2f);
                  else if(damage <= 10) yield return new WaitForSeconds(0.1f);
                  else if(damage <= 20) yield return new WaitForSeconds(0.05f);
                  else if(damage <= 30) yield return new WaitForSeconds(0.025f);
                  else if(damage <= 40) yield return new WaitForSeconds(0.0175f);
                  else yield return new WaitForSeconds(0.00875f);
                  tempHp--;
            }
      }

      void SetStatusText() {
            if(_pokemon.Status == null) {
                  statusText.text = "";
            } else {
                  statusText.text = _pokemon.Status.Id.ToString().ToUpper();
                  statusText.color = statusColors[_pokemon.Status.Id];
            }
      }
}
