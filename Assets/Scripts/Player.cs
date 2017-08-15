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
    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPosition = Vector2.zero;
    private bool isSwipe = false;
    private float minSwipeDistance = 50.0f;
    private float maxSwipeTime = 0.5f;

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
			rigidBody.velocity = new Vector2(movementSpeed, rigidBody.velocity.y);
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
        HandleKeyPress();
        HandleSwipe();
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

    private void HandleKeyPress() {
        if (Input.GetKey(KeyCode.DownArrow)) {
            sliding = true;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            jumping = true;
        }
    }

    private void HandleSwipe() {
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                switch (touch.phase) {
                    case TouchPhase.Began:
                        isSwipe = true;
                        fingerStartTime = Time.time;
                        fingerStartPosition = touch.position;
                        break;
                    case TouchPhase.Canceled:
                        isSwipe = false;
                        break;
                    case TouchPhase.Ended:
                        float gestureTime = Time.time - fingerStartTime;
                        float gestureDistance = (touch.position - fingerStartPosition).magnitude;
                        if (isSwipe && gestureTime < maxSwipeTime && gestureDistance > minSwipeDistance) {
                            Vector2 direction = touch.position - fingerStartPosition;
                            Vector2 swipeType = Vector2.zero;

                            if (Mathf.Abs(direction.x) <= Mathf.Abs(direction.y)) {
                                swipeType = Vector2.up * Mathf.Sign(direction.y);
                            }

                            if (swipeType.y != 0.0f) {
                                if (swipeType.y > 0.0f) {
                                    jumping = true;
                                } else {
                                    sliding = true;
                                }
                            }
                        }
                    break;
                }
            }
        }
    }
}
