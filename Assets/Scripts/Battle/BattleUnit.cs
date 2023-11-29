using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour {
    [SerializeField] PokemonBase pkmBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pkm { get; set; }
    Vector3 originPos;
    Image image;

    private void Awake() {
        image = GetComponent<Image>();
        originPos = image.transform.localPosition;
    }

    public void Setup() {
        Pkm = new Pokemon(pkmBase, level);

        if (isPlayerUnit) {
            image.sprite = Pkm.PkmBase.BackSprite;
        } else {
            image.sprite = Pkm.PkmBase.FrontSprite;
        }

        PlayerEnterAnimation();
    }

    public void PlayerEnterAnimation() {
        if(isPlayerUnit) {
            image.transform.localPosition = new Vector3(-500f, originPos.y);
        } else {
            image.transform.localPosition = new Vector3(500f, originPos.y);
        }

        image.transform.DOLocalMoveX(originPos.x, 1.5f);
    }

    public void PlayAttackAnimation() {
        var sequence = DOTween.Sequence();
        if(isPlayerUnit) {
            sequence.Append(image.transform.DOLocalMoveX(originPos.x + 50, 0.25f));
        } else {
            sequence.Append(image.transform.DOLocalMoveX(originPos.x - 50, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originPos.x, 0.25f));
    }

    public void PlayHitAnimation() {
        var sequence = DOTween.Sequence();
        if(isPlayerUnit) {
            sequence.Append(image.transform.DOLocalMoveX(originPos.x + 50, 0.25f));
        } else {
            sequence.Append(image.transform.DOLocalMoveX(originPos.x - 50, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originPos.x, 0.25f));
    }
}
