using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed;
    public LayerMask foregroundLayer;
    public LayerMask grassLayer;

    public event Action OnEncoutered;

    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate() {
        if (!isMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0; // remove diagonal moving
            if (input != Vector2.zero) {

                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos)) {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos) {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        checkForEncounters(); // moving on grass to detect monster
    }

    private bool IsWalkable(Vector3 targetPos) {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, foregroundLayer) != null) {
            return false;
        }
        return true;
    }

    private void checkForEncounters() {
        if (Physics2D.OverlapCircle(transform.position, 0.1f, grassLayer) != null) {
            if (UnityEngine.Random.Range(1, 100) <= 10 ) {
                animator.SetBool("isMoving", false);
                OnEncoutered();
            }
        }
    }
}