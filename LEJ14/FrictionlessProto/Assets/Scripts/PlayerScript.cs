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

    [SerializeField] Collider2D collider;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] float collisionDist;
    float skinWidth = 0.02f;
    [SerializeField] LayerMask groundLayer;
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
        float horizVelAdd = moveDirection * speed * Time.fixedDeltaTime;
        if(Mathf.Abs(horizVelAdd + rb.velocity.x) < minHorizVel)
        {
            if(moveDirection < 0) 
            { 
                rb.velocity = new Vector2(-minHorizVel, rb.velocity.y); 
            } else
            {
                rb.velocity = new Vector2(minHorizVel, rb.velocity.y);
            }
        }else if(Mathf.Abs(horizVelAdd + rb.velocity.x) > maxHorizVel)
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
            rb.velocity += horizVelAdd * Vector2.right;
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
            rb.AddForce(Vector2.up * jumpForce);
            return;
        }

        bottomLeftLeft = new Vector2(collider.bounds.min.x, collider.bounds.min.y + skinWidth);
        topLeft = new Vector2(collider.bounds.min.x, collider.bounds.max.y);

        RaycastHit2D leftBL = Physics2D.Raycast(bottomLeftLeft, Vector2.left, collisionDist, groundLayer);
        RaycastHit2D leftTL = Physics2D.Raycast(topLeft, Vector2.left, collisionDist, groundLayer);
        if(leftBL || leftTL)
        {
            rb.AddForce(new Vector2(1, 2).normalized * jumpForce);
            return;
        }

        bottomRightRight = new Vector2(collider.bounds.max.x, collider.bounds.min.y + skinWidth);
        topRight = new Vector2(collider.bounds.max.x, collider.bounds.max.y);

        RaycastHit2D rightBR = Physics2D.Raycast(bottomRightRight, Vector2.right, collisionDist, groundLayer);
        RaycastHit2D rightTR = Physics2D.Raycast(topRight, Vector2.right, collisionDist, groundLayer);
        if(rightBR || rightTR)
        {
            rb.AddForce(new Vector2(-1, 2).normalized * jumpForce);
        }
    }

    public void OnSwapFriction()
    {
        IsHighFriction = !IsHighFriction;
    }
}
