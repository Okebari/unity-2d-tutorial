using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    public int nextMove;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        Think();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y );

        // Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.5f, rigid.position.y);

        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        // 레이히트는 관통이 안된다. 한번 맞으면 끝
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if(rayHit.collider == null){
            // Debug.Log("경고");
            Turn();
        }
    }
    
    void Think(){

        // Set Next Active
        nextMove = Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite
        if(nextMove != 0){
            spriteRenderer.flipX = nextMove == 1;
        }

        // Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn(){
        // nextMove = nextMove * -1;
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        
        // CancelInvoke("Think");
        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged(){
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y
        spriteRenderer.flipY = true;

        // Collider Disable
        capsuleCollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Destory
        Invoke("DeActive", 5);
    }

    void DeActive(){
        gameObject.SetActive(false);
    }

}
