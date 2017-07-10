using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	private Rigidbody2D rigidBody;
	private Animator animator;

	[SerializeField]
	private float movementSpeed;
	private float countDownTimer = 3.0f;
	private bool sliding;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		StartCoroutine("Countdown", countDownTimer);
	}

	void Update() {
		HandleInput();
	}

	void FixedUpdate() {
		HandleMovement();
		ResetValues();
	}

	private void HandleMovement() {
		AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

		if (animator.GetBool ("running")) {
			rigidBody.velocity = new Vector2 (movementSpeed, rigidBody.velocity.y);
		}

		if (sliding && !animatorStateInfo.IsName("Slide")) {
			animator.SetBool("sliding", true);
		} else if (!animatorStateInfo.IsName("Slide")) {
			animator.SetBool("sliding", false);
		}
	}

	private void HandleInput() {
		if (Input.GetKey(KeyCode.DownArrow)) {
			sliding = true;
		}
	}

	private IEnumerator Countdown(int time) {
		while (countDownTimer > 0) {
			yield return new WaitForSeconds(1);
			countDownTimer -= 1;
		}

		animator.SetBool("running", true);
	}

	private void ResetValues() {
		sliding = false;
	}
}
