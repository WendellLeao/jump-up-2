using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
    //Structers
    [Serializable] public struct PlayerMovement
    {
        [Header("Movement")]
        [Range(1f, 10f)] public float speed;
        [HideInInspector] public float input;
    }
    [Serializable] public struct PlayerJump
    {
        [Header("Jump")]
        [Range(1f, 10f)] public float force;
        [HideInInspector] public bool isGrounded;
        public bool isJumping;

        [Header("Checking Ground")]
        public Transform checkGround;
        public LayerMask whatIsGround;
        public float checkRadius;

        [Header("Fixing Gravity")]
        [HideInInspector] public float fallMultiplier;
        [HideInInspector] public float lowJumpMultiplier;
    }

    //Variables
    [SerializeField] private PlayerMovement move;
    [Space(5)]
    [SerializeField] private PlayerJump jump;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera cam;

    [Header("Core")]
    SpriteRenderer sprite;
    BoxCollider2D box;
    Rigidbody2D body;
    Animator anim;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem fallingDustPS;
    [SerializeField] private ParticleSystem dustPS;
    //Colliders
    Vector2 originalBoxSize;

    [Header("Player's Life")]
    public static int lifes = 3;
    private bool isVulnerable = true, isTakingDamage = false, isInLava = false;
    private Physics2D physics;

    //Unity Functions
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start() 
    {
        jump.isJumping = false;

        //Getting box collider size values
        originalBoxSize = new Vector2(box.size.x, box.size.y);
    }
    private void Update()
    {
        if (GameStateManager.gameState == GameState.Playing)
        {
            //Getting axis input values
            move.input = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump") && jump.isGrounded) 
            {
                CreateDust();
                jump.isJumping = true;
                anim.SetTrigger("isJumping");
                anim.SetBool("isFalling", false);
                AudioManager.instance.Play("JumpSound");
                body.velocity = Vector2.up * jump.force;
            }
        }
    }
    private void FixedUpdate()
    {
        if(GameStateManager.gameState == GameState.Playing)
        {
            //Calling functions
            LifeSystem();
            Move();
            Jump();

            if(isInLava)
            {
                Swimming();
            }
        }

        else if(GameStateManager.gameState == GameState.PlayerIsDead)
        {
            body.velocity = new Vector2(0f, body.velocity.y);
        }

        else if(GameStateManager.gameState == GameState.LevelCompleted)
        {
            body.velocity = new Vector2(move.speed, body.velocity.y);
            Destroy(gameObject, 1.5f);
        }
    }

    //Player's Functions
    void Move()
    {
        body.velocity = new Vector2(move.input * move.speed, body.velocity.y);

        if (move.input > 0 && sprite.flipX || move.input < 0 && !sprite.flipX)
        {
            Flip();
        }

        else if (move.input != 0 && jump.isGrounded)
        {
            CreateDust();
        }
    }
    void Jump()
    {
        //checking if the player is on the ground
        jump.isGrounded = Physics2D.OverlapCircle(jump.checkGround.position, jump.checkRadius, jump.whatIsGround);

        //Fixing Gravity
        jump.fallMultiplier = 2.5f;
        jump.lowJumpMultiplier = 2f;

        if(!isInLava)
        {
            if (body.velocity.y < 0)
            {
                body.velocity += Vector2.up * Physics2D.gravity.y * (jump.fallMultiplier - 1) * Time.fixedDeltaTime;

                //Reset isJumping
                jump.isJumping = false;
                anim.SetBool("isFalling", true);
            }

            else if (body.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                body.velocity += Vector2.up * Physics2D.gravity.y * (jump.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        //Shorten collider when player is jumping
        else if(jump.isJumping) box.size = new Vector2(originalBoxSize.x, originalBoxSize.y - 0.8f);
        else  box.size = originalBoxSize;
    }
    void Swimming()
    {
        if(Input.GetButton("Jump"))
        {
            body.AddForce(new Vector2(0, 20f * Time.fixedDeltaTime), ForceMode2D.Impulse);
        }
        else
        {
            //decrease the gravity force
        }
    }
    void Flip()
    {
        sprite.flipX = !sprite.flipX;
    }
    void CreateDust()
    {
        dustPS.Play();
    }
    void CreateFallingDust()
    {
        fallingDustPS.Play();
    }
    void LifeSystem()
    {
        if(isTakingDamage)
        {
            if(isVulnerable) StartCoroutine(PlayerDamage());
        }

        if (lifes <= 0)
        {
            AudioManager.instance.Play("DeathSound");
            GameStateManager.gameState = GameState.PlayerIsDead;
        } 
    }

    //Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(GameStateManager.gameState == GameState.Playing)
        {
            if (collision.gameObject.tag == "Enemy" && isVulnerable)
            {
                //if(lifes > 0) StartCoroutine(PlayerDamage());
            }

            else if(collision.gameObject.tag == "DeadLine")
            {
                /*float currentY = cinemachinePos.position.y;
                cinemachinePos.position = new Vector2(transform.position.x, currentY);*/
                anim.SetTrigger("wasHited");
                lifes = 0;
            }

            else if(collision.gameObject.tag == "Ground")
            {
                CreateFallingDust();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(GameStateManager.gameState == GameState.Playing)
        {
            if (collision.gameObject.tag == "Damager") 
            {
                isTakingDamage = true;
            }

            else if(collision.gameObject.tag == "DeadLine")
            {
                cam.Follow = null;
                StartCoroutine(TimeToDie(0.5f));
            }
            
            else if (collision.gameObject.tag == "Lava")
            {
                isInLava = true;
                //isTakingDamage = true;
            }

            else if(collision.gameObject.tag == "EndLine")
            {
                GameStateManager.gameState = GameState.LevelCompleted;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) 
    {
       if(GameStateManager.gameState == GameState.Playing)
        {
            if (collision.gameObject.tag == "Damager") 
            {
                isTakingDamage = false;
            }
            
            else if (collision.gameObject.tag == "Lava") 
            {
                isInLava = false;
                isTakingDamage = false;
            }
        }
    }

    //IEnumerators
    IEnumerator PlayerDamage()
    {
        lifes -= 1;
        isVulnerable = false;
        sprite.color = Color.red;
        anim.SetTrigger("wasHited");
        AudioManager.instance.Play("HitSound");

        for (float i = 0f; i < 0.6f; i += 0.1f)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        sprite.color = Color.white;
        isVulnerable = true;
    }
    IEnumerator TimeToDie(float time)
    {
        cam.Follow = null;
        yield return new WaitForSeconds(time);
        lifes = 0;
    }

}
