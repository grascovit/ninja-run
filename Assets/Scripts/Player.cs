using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	private Rigidbody2D rigidBody;
	private Animator animator;

	[SerializeField]
	private float movementSpeed;
	private float countDownTimer = 3.0f;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		StartCoroutine("Countdown", countDownTimer);
	}

	void FixedUpdate() {
		HandleMovement();
	}

	private void HandleMovement() {
		rigidBody.velocity = new Vector2(movementSpeed, rigidBody.velocity.y);
	}

	private IEnumerator Countdown(int time) {
		while (countDownTimer > 0) {
			yield return new WaitForSeconds(1);
			countDownTimer -= 1;
		}

		animator.SetBool("running", true);
	}
}
