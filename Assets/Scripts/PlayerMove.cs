﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;

    // Audio
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action){
        switch(action){
            case "JUMP" :
                audioSource.clip = audioJump;
                break;
            case "ATTACK" :
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED" :
                audioSource.clip = audioDamaged;
                break;
            case "ITEM" :
                audioSource.clip = audioItem;
                break;
            case "DIE" :
                audioSource.clip = audioDie;
                break;
            case "FINISH" :
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    private void Update(){

        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")){
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        // Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.0f, rigid.velocity.y);
        }

        // Direction Sprite
        if(Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if(Mathf.Abs(rigid.velocity.x) < 0.3){
            anim.SetBool("isWalking", false);
        }else{
            anim.SetBool("isWalking", true);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // 캐릭터 좌우 설정
        if(rigid.velocity.x > maxSpeed){
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }else if(rigid.velocity.x < maxSpeed*(-1)){ 
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

        // Landing Platform
        if(rigid.velocity.y < 0){
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0));
            // 레이히트는 관통이 안된다. 한번 맞으면 끝
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if(rayHit.collider != null){
                if(rayHit.distance < 0.5f){
                    // Debug.Log(rayHit.collider.name);
                    anim.SetBool("isJumping", false);
                }
            }
        }

        // Debug.Log(rigid.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Enemy"){

            // Attack
            // 몬스터보다 위에있음 + 아래로 낙하 중 = 밟음
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y){
                OnAttack(collision.transform);
            }else{
            // Damaged
                OnDamaged(collision.transform.position);
            }

        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Item"){
            // Debug.Log("Item");
            // Point
            bool isBronze = col.gameObject.name.Contains("Bronze");
            bool isSilver = col.gameObject.name.Contains("Silver");
            bool isGold = col.gameObject.name.Contains("Gold");

            if(isBronze){
                gameManager.stagePoint += 50;
            }else if(isSilver){
                gameManager.stagePoint += 100;
            }else if(isGold){
                gameManager.stagePoint += 150;
            }

            PlaySound("ITEM");

            // Deactive Item
            col.gameObject.SetActive(false);
        }else if(col.gameObject.tag == "Finish"){
            PlaySound("FINISH");

            // Next Stage
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy){
        // Point
        gameManager.stagePoint += 100;
        
        // Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos){

        // HP Down
        gameManager.HpDown();

        // Change Layer (Immortal Active)
        gameObject.layer = 11;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x-targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");

        PlaySound("DAMAGED");

        
        Invoke("OffDmg", 1);
    }

    void OffDmg(){
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1f);
    }

    public void OnDie(){
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y
        spriteRenderer.flipY = true;

        // Collider Disable
        capsuleCollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlaySound("DIE");
    }

    public void VelocityZero(){
        rigid.velocity = Vector2.zero;
    }

}
