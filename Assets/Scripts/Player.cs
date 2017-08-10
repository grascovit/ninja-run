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
    [SerializeField]
    private Transform[] groundPoints;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private LayerMask whatIsGround;
    private bool isGrounded;
    private bool jumping;
    [SerializeField]
    private float jumpForce;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		StartCoroutine("Countdown", countDownTimer);
	}

	void Update() {
		HandleInput();
	}

	void FixedUpdate() {
        isGrounded = IsGrounded();
		HandleMovement();
        HandleLayers();
        ResetValues();
	}

	private void HandleMovement() {
		AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (rigidBody.velocity.y < 0) {
            animator.SetBool("landing", true);
        }

		if (animator.GetBool ("running")) {
			rigidBody.velocity = new Vector2 (movementSpeed, rigidBody.velocity.y);
		}

        if (isGrounded && jumping) {
            isGrounded = false;
            rigidBody.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumping", true);
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

        if (Input.GetKey(KeyCode.UpArrow)) {
            jumping = true;
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
        jumping = false;
	}

    private bool IsGrounded() {
        if (rigidBody.velocity.y <= 0) {
            foreach (Transform point in groundPoints) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++) {
                    if (colliders[i].gameObject != gameObject) {
                        animator.SetBool("jumping", false);
                        animator.SetBool("landing", false);
                        return true; 
                    }
                }
            }
        }

        return false;
    }

    private void HandleLayers() {
        if (!isGrounded) {
            animator.SetLayerWeight(1, 1);
        } else {
            animator.SetLayerWeight(1, 0);
        }
    }
}
