using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] PhysicsMaterial2D lowFriction;
    [SerializeField] PhysicsMaterial2D highFriction;
    bool isHighFriction = true;
    bool IsHighFriction { get => isHighFriction; set
        {
            if (isHighFriction) 
            { 
                collider.sharedMaterial = lowFriction; renderer.color = Color.blue; 
            } else
            {
                collider.sharedMaterial = highFriction;
                renderer.color = Color.red;
            }
            isHighFriction = !isHighFriction;
        } }

    [SerializeField] float speed;
    [SerializeField] float maxHorizVel;
    [SerializeField] float minHorizVel;
    [SerializeField] float LFAccelMult;
    [SerializeField] float LFMaxVelMult;

    float MaxHorizVel { get => maxHorizVel * ((isHighFriction) ? 1 : LFMaxVelMult); }
    float Speed { get => speed * ((isHighFriction) ? 1 : LFAccelMult); }


    [SerializeField] Collider2D collider;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] float collisionDist;
    float skinWidth = 0.02f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallJumpLayer;
    Vector2 bottomLeftDown, bottomRightDown, topLeft, topRight, bottomLeftLeft, bottomRightRight;
    [SerializeField] float jumpForce;

    float moveDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizVelAdd = moveDirection * Speed * Time.fixedDeltaTime;
        if(Mathf.Abs(horizVelAdd + rb.velocity.x) < minHorizVel && horizVelAdd > 0)
        {
            if(moveDirection < 0) 
            { 
                rb.velocity = new Vector2(-minHorizVel, rb.velocity.y); 
            } else
            {
                rb.velocity = new Vector2(minHorizVel, rb.velocity.y);
            }
        }else if(Mathf.Abs(horizVelAdd + rb.velocity.x) > MaxHorizVel)
        {
            if (moveDirection < 0)
            {
                rb.velocity = new Vector2(-maxHorizVel, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(maxHorizVel, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity += new Vector2(horizVelAdd, 0);
        }

    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<float>();
    }

    public void OnJump()
    {
        bottomLeftDown = new Vector2(collider.bounds.min.x + skinWidth, collider.bounds.min.y);
        bottomRightDown = new Vector2(collider.bounds.max.x - skinWidth, collider.bounds.min.y);

        RaycastHit2D downBottomLeft = Physics2D.Raycast(bottomLeftDown, Vector2.down, collisionDist, groundLayer);
        RaycastHit2D downBottomRight = Physics2D.Raycast(bottomRightDown, Vector2.down, collisionDist, groundLayer);
        if(downBottomLeft || downBottomRight)
        {
            rb.AddForce(Vector2.up * jumpForce * 1.25f);
            return;
        }

        if (!isHighFriction) { return; }

        bottomLeftLeft = new Vector2(collider.bounds.min.x, collider.bounds.min.y + skinWidth);
        topLeft = new Vector2(collider.bounds.min.x, collider.bounds.max.y);

        RaycastHit2D leftBL = Physics2D.Raycast(bottomLeftLeft, Vector2.left, collisionDist, wallJumpLayer);
        RaycastHit2D leftTL = Physics2D.Raycast(topLeft, Vector2.left, collisionDist, wallJumpLayer);
        if(leftBL || leftTL)
        {
            rb.AddForce(new Vector2(0.75f, 1.5f) * jumpForce);
            return;
        }

        bottomRightRight = new Vector2(collider.bounds.max.x, collider.bounds.min.y + skinWidth);
        topRight = new Vector2(collider.bounds.max.x, collider.bounds.max.y);

        RaycastHit2D rightBR = Physics2D.Raycast(bottomRightRight, Vector2.right, collisionDist, wallJumpLayer);
        RaycastHit2D rightTR = Physics2D.Raycast(topRight, Vector2.right, collisionDist, wallJumpLayer);
        if(rightBR || rightTR)
        {
            rb.AddForce(new Vector2(-0.75f, 1.5f) * jumpForce);
        }
    }

    public void OnSwapFriction()
    {
        IsHighFriction = !IsHighFriction;
    }
}
