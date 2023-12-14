using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogController : MonoBehaviour {
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogController Instance { get; private set; }
    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    private void Awake() {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog) {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate() {
        if(Input.GetKeyDown(KeyCode.Z) && isTyping == false) {
            ++currentLine;
            if(currentLine < dialog.Lines.Count) {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            } else {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line) {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray()) {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / Constants.LETTER_PER_SECOND);
        }
        isTyping = false;
    }
}
