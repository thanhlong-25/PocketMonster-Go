using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB {
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>{
        {
            ConditionID.psn, new Condition() { 
                Name = "Poison", 
                StartMessage = "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) => {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name} hurt it self due to poison");
                }
            }
        },
        {
            ConditionID.brn, new Condition() { 
                Name = "Burn", 
                StartMessage = "has been burned",
                OnAfterTurn = (Pokemon pokemon) => {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name} hurt it self due to burn");
                }
            }
        },
        {
            ConditionID.par, new Condition() { 
                Name = "Paralyze", 
                StartMessage = "has been paralyzed",
                OnBeforeSkill = (Pokemon pokemon) => {
                    if(Random.Range(1, 5) == 1) {
                        pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name}'s paralyzed and can't do anything");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.slp, new Condition() { 
                Name = "Sleep", 
                StartMessage = "has fallen asleep",
                OnStart = (Pokemon pokemon) => {
                    //Sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1, 4);
                },
                OnBeforeSkill = (Pokemon pokemon) => {
                    if(pokemon.StatusTime <= 0) {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name} woke up!!!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name} is sleeping!!!");
                    return false;
                },
            }
        },
        {
            ConditionID.frz, new Condition() { 
                Name = "Freeze", 
                StartMessage = "has been frozen",
                OnBeforeSkill = (Pokemon pokemon) => {
                    if(Random.Range(1, 5) == 1) {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name}'s is not frozen anymore");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.PkmBase.Name} was frozen!!!");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID {
    none,
    psn, // Poison
    brn, // Burn
    slp, // Sleep
    par, // paralyze tê liệt
    frz // Freeze
}
