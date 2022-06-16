using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public bool CanGoLeft;
    public bool CanGoRight;
    public bool CanGoUp;
    public bool CanGoDown;
    private GameObject playerObject;
    private Transform player;
    private EnemyStats enemyStats;
    private TurnSystem turnSystem;
    private float distanceToPlayer;
    private Vector3 startPosition;
    private Vector3 movePosition;
    private LayerMask collisionLayer;
    public List<GameObject> squadList;
    public bool isAggroed;
    public bool canSeePlayer;
    
    

    void Start()
    {
        startPosition = transform.position;
        movePosition = transform.position;
        playerObject = GameObject.FindWithTag("Player");
        player = playerObject.transform;
        turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        enemyStats = GetComponent<EnemyStats>();
        collisionLayer = LayerMask.GetMask("Collision");
        isAggroed = false;
        canSeePlayer = false;
    }

    void FixedUpdate()
    {
        // Only act if it's enemy turn
        if (TurnSystem.state == TurnState.ENEMYTURN)
        {
            // If it's this enemy's turn
            if (turnSystem.CurrentEnemy == this.gameObject)
            {
                    // If we've finished moving finish turn
                if (Vector3.Distance(startPosition, transform.position) == 1)
                {
                    startPosition = transform.position;
                    movePosition = transform.position;
                    EndTurn();
                }
                // If we're currently moving keep moving
                else if (Vector3.Distance(transform.position, movePosition) > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, movePosition, MoveSpeed * Time.deltaTime);
                }
                // If we haven't moved yet
                else if (Vector3.Distance(transform.position, movePosition) == 0)
                {
                    distanceToPlayer = GetTileDistance(transform.position, player.position);
                    // Aggro if player is close enough and in line of sight
                    if (isAggroed == false)
                    {
                        // If enemy isn't aggroed and player is too far away do nothing
                        if (distanceToPlayer > enemyStats.AggroDistance)
                        {
                            EndTurn();
                        }
                        // If enemy isn't aggroed and player is within aggro distance
                        else if (distanceToPlayer <= enemyStats.AggroDistance)
                        {
                            canSeePlayer = PlayerInLineOfSight();

                            // If player is in line of sight aggro and move
                            if (canSeePlayer == true)
                            {
                                isAggroed = true;
                                AlertSquad();
                                MoveTowardsPosition(transform.position, player.position);
                            }
                            else if (canSeePlayer == false)
                            {
                                EndTurn();
                            }
                        }
                    }
                    
                    else if (isAggroed == true)
                    {
                        // Check if enemy should attack
                        if (distanceToPlayer <= enemyStats.MeleeDistance)
                        {
                            HandleCombat(player);
                            EndTurn();
                        }
                        // Check if enemy should move towards player
                        else if (distanceToPlayer <= enemyStats.DisengageDistance)
                        {
                            MoveTowardsPosition(transform.position, player.position);
                        }
                        // Check if enemy should disengage
                        else if (distanceToPlayer > enemyStats.DisengageDistance)
                        {
                            isAggroed = false;
                            canSeePlayer = false;
                            EndTurn();
                        }
                    }
                } 
            }
        }
    }

    public void StartTurn()
    {
        // Anything we want to call at start of this enemy's turn
    }

    public void EndTurn()
    {
        // Anything we want to call at end of this enemy's turn
        turnSystem.EndEnemyTurn();
    }

    bool CheckCollision(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.2f, collisionLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void HandleCombat(Transform target)
    {
        enemyStats.DealDamage(enemyStats.AttackDamage, target);
    }

    public float GetTileDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
    }

    public void AggroSelf()
    {
        if (isAggroed == false)
        {
            // Get player reference if we don't have it already
            if (playerObject == null) playerObject = GameObject.FindWithTag("Player");

            float distanceToPlayer = GetTileDistance(transform.position, playerObject.transform.position);
            
            isAggroed = true;

            // If player is outside of disengage distance increase disengage distance to current distance + 4
            // Stops enemy from immediately unaggroing on their turn
            // And if player runs away after sniping, enemy can chase them a bit
            if (distanceToPlayer > enemyStats.DisengageDistance)
            {
                enemyStats.DisengageDistance = (int)distanceToPlayer + 4;
            }
        }
    }

    public void AlertSquad()
    {
        for (int i = 0; i < squadList.Count; i++)
        {
            // Only aggro squad member if enemy has line of sight. This stops squad members being aggroed through walls.
            RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, squadList[i].transform.position - transform.position, Vector3.Distance(transform.position, squadList[i].transform.position), collisionLayer);
            if (WallTileInArray(hitArray) == false)
            {
                squadList[i].GetComponent<EnemyAI>().AggroSelf();
            }
        }
    }

    bool PlayerInLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerObject.transform.position - transform.position, collisionLayer);

        if (hit.collider.gameObject.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool WallTileInArray(RaycastHit2D[] hitArray)
    {
        for (int i = 0; i < hitArray.Length; i++)
        {
            if (hitArray[i].collider.gameObject.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }

    void MoveTowardsPosition(Vector2 a, Vector2 b)
    {
        // If we're at the position stop
        if (a == b)
        {
            return;
        }

        float xDistance = b.x - a.x;
        float yDistance = b.y - a.y;

        Vector3 newPositionX = new Vector2(0f, 0f);
        Vector3 newPositionY = new Vector2(0f, 0f);

        Vector2 left = (Vector2)transform.position + Vector2.left;
        Vector2 right = (Vector2)transform.position + Vector2.right;
        Vector2 up = (Vector2)transform.position + Vector2.up;
        Vector2 down = (Vector2)transform.position + Vector2.down;

        CanGoLeft = !CheckCollision(left);
        CanGoRight = !CheckCollision(right);
        CanGoUp = !CheckCollision(up);
        CanGoDown = !CheckCollision(down);

        // If the player is to the left/right/up/down and there's no immediate obstacle
        // If the player is directly up only newPositionY will be set.
        // If the player is directly left only newPosistionX will be set.
        // If the player is up and left both newPositionY and newPositionX will be set and we will have to pick one later.
        
        // Player is to the left
        if (xDistance < 0 && CanGoLeft)
        {
            newPositionX = left;
        }
        // Player is to the right
        else if (xDistance > 0 && CanGoRight)
        {
            newPositionX = right;
        }
        // Player is up
        if (yDistance > 0 && CanGoUp)
        {
            newPositionY = up;
        }
        // Player is down
        else if (yDistance < 0 && CanGoDown)
        {
            newPositionY = down;
        }
        
        // If neither newPositionX or newPositionY were set because the way was blocked
        // try to move in a lateral direction
        if (newPositionX.magnitude == 0 && newPositionY.magnitude == 0)
        {
            // If player is directly left/right set newPositionY to up or down
            if (Mathf.Abs(xDistance) != 0)
            {
                if (CanGoUp && !CanGoDown)
                {
                    movePosition = up;
                }
                else if (CanGoDown && !CanGoUp)
                {
                    movePosition = down;
                }
                else if (CanGoUp && CanGoDown)
                {
                    if (Random.Range(0,2) == 0)
                    {
                        movePosition = up;
                    }
                    else
                    {
                        movePosition = down;
                    }
                }
                // If enemy can't move laterally do nothing
                else
                {
                    EndTurn();
                }
            }
            // If player is directly up/down set newPositionX to left or right
            else if (Mathf.Abs(yDistance) != 0)
            {
                if (CanGoLeft && !CanGoRight)
                {
                    movePosition = left;
                }
                else if (CanGoRight && !CanGoLeft)
                {
                    movePosition = right;
                }
                else if (CanGoLeft && CanGoRight)
                {
                    if (Random.Range(0,2) == 0)
                    {
                        movePosition = left;
                    }
                    else
                    {
                        movePosition = right;
                    }
                }
                // If enemy can't move laterally do nothing
                else
                {
                    EndTurn();
                }
            }
        }
        // If target is directly above or below ignore x
        else if (newPositionX.magnitude == 0 && newPositionY.magnitude != 0)
        {
            movePosition = newPositionY;
        }
        // If target is directly left or right ignore y
        else if (newPositionY.magnitude == 0 && newPositionX.magnitude != 0)
        {
            movePosition = newPositionX;
        }
        // Choose between x and y
        else
        {
           if (Random.Range(0,2) == 0)
           {
               movePosition = newPositionY;
           }
           else
           {
               movePosition = newPositionX;
           }
        }
    }
}
