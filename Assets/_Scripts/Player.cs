using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Stats")]
    public float maxSpeed = 15f;

    public int playerNumber = 0;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;
    AudioSource source;

    private static string MOVESPEED = "movespeed";
    private static string SLIDE = "startSlide";
    private static string SLIDING = "sliding";
    private bool sliding = false;

    List<GameObject> touching = new List<GameObject>();

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        source = GetComponent<AudioSource>();
    }

    void FixedUpdate(){
        Movement();
    }

    void Movement(){
        float moveHorizontal = playerNumber == 0 ? Input.GetAxisRaw("Horizontal") : Input.GetAxisRaw("Horizontal2");
        float moveVertical = playerNumber == 0 ? Input.GetAxisRaw("Vertical") : Input.GetAxisRaw("Vertical2");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        if(movement.magnitude > 0.1f && !source.isPlaying){
            source.pitch = Random.Range(0.5f, 0.8f);
            source.Play();
        }
        else if(movement.magnitude <= 0.1f && source.isPlaying){
            source.Stop();
        }

        rb.velocity = movement.normalized * maxSpeed * Time.fixedDeltaTime;

        sr.flipX = moveHorizontal < 0 ? true : false;
        
        animator.SetFloat(MOVESPEED, movement.magnitude);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(!sliding){
            animator.SetTrigger(SLIDE);
        }

        sliding = true;
        touching.Add(other.gameObject);
        animator.SetBool(SLIDING, sliding);
    }

    private void OnCollisionExit2D(Collision2D other) {
        touching.Remove(other.gameObject);
        if(touching.Count <= 0) {
            sliding = false;
            animator.SetBool(SLIDING, sliding);
        }
    }
}