using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour {
    [SerializeField] BattleHud hud;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pkm { get; set; }

    public bool IsPlayerUnit { 
        get { return isPlayerUnit; }
    }

    public BattleHud Hud { 
        get { return hud; }
    }

    Vector3 originPos;
    Color originColor;
    Image image;

    private void Awake() {
        image = GetComponent<Image>();
        originPos = image.transform.localPosition;
        originColor = image.color;
    }

    public void Setup(Pokemon pokemon) {
        Pkm = pokemon;

        if (isPlayerUnit) {
            image.sprite = Pkm.PkmBase.BackSprite;
        } else {
            image.sprite = Pkm.PkmBase.FrontSprite;
        }

        hud.setData(pokemon);
        image.color = originColor;
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
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originColor, 0.1f));
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originColor, 0.1f));
    }

    public void PlayFaintedAnimation() {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
