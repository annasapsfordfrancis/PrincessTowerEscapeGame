using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public LayerMask CollisionLayer;
    private Vector3 startPosition;
    private Vector3 movePosition;
    private bool playerIsAiming;
    public string activeAbility;
    private PlayerStats playerStats;
    private TurnSystem turnSystem;
    private Inventory inventory;
    private HUD HUD;
    private SoundManager soundManager;
    void Start()
    {
        movePosition = transform.position;
        playerIsAiming = false;
        playerStats = GetComponent<PlayerStats>();
        turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        inventory = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        HUD = GameObject.Find("UI").GetComponent<HUD>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    void Update()
    {
        // Only update if it's player's turn
        if (TurnSystem.state == TurnState.PLAYERTURN)
        {
            if (playerIsAiming)
            {
                // Render target shape
                if(Input.GetButtonDown("Fire1"))
                {
                    UseAbilityAtLocation(HUD.GetAbilityTargetPosition());
                }
                // Cancel aiming on right click
                else if(Input.GetButtonDown("Fire2") || Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                {
                    CancelAiming();
                }
            }
        }
    }

    void FixedUpdate()
    {
        // If player has moved or used all their actions end turn
        if (TurnSystem.state == TurnState.PLAYERTURN && (Vector3.Distance(startPosition, transform.position) == 1 || playerStats.ActionsRemainingThisTurn == 0))
            {
                EndTurn();
            }
        // Only move if it's player's turn
        if (TurnSystem.state == TurnState.PLAYERTURN)
        {
            if (Vector3.Distance(transform.position, movePosition) > 0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePosition, MoveSpeed * Time.deltaTime);
            }
            
            else if (Vector3.Distance(transform.position, movePosition) == 0f)
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
                {
                    HandleMovement(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f));
                }

                else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
                {
                    HandleMovement(new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f));
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > float.Epsilon || Mathf.Abs(Input.GetAxisRaw("Vertical")) > float.Epsilon)
            {
                playerStats.UseAction();
            }
        }
    }

    void HandleMovement(Vector3 newPosition)
    {
        Collider2D collider = GetCollision(movePosition + newPosition, .2f, CollisionLayer);
            if (collider == null)
            {
                movePosition += newPosition;
            }
            else
            {
                if (collider.CompareTag("Enemy"))
                {   
                    HandleCombat(collider.transform);
                    // Reset key input so player doesn't attack unintentionally next turn
                    Input.ResetInputAxes();

                    EndTurn();
                }
            }
    }

    // Deals damage to target
    void HandleCombat(Transform target)
    {
        playerStats.DealDamage(playerStats.AttackDamage, target.transform);
        soundManager.PlaySound("melee");
    }

    // Checks for collisions at target location
    Collider2D GetCollision(Vector3 target, float radius, LayerMask collisionLayer)
    {
        Collider2D collider = Physics2D.OverlapCircle(target, radius, collisionLayer);
        return collider;
    }

    public void StartTurn()
    {
        startPosition = transform.position;
        playerIsAiming = false;
        playerStats.ReplenishActions();
        TurnSystem.state = TurnState.PLAYERTURN;
    }

    public void EndTurn()
    {
        playerIsAiming = false;
        playerStats.UseAllActions();
        turnSystem.EndPlayerTurn();
    }

    public void StartAiming(string ability)
    {
        // Change cursor to reticule
        playerIsAiming = true;
        activeAbility = ability;
        HUD.ActivateAimingReticule(ability);
        HUD.DisableActionBarButtons();
        HUD.CreateFloatingMessage("aiming", transform);
    }

    public void CancelAiming()
    {
        // Change cursor to default
        playerIsAiming = false;
        HUD.DeactivateAimingReticule();
        HUD.EnableActionBarButtons();
        activeAbility = null;
    }

    // Overload so target square can just get the bool without the array
    public bool AbilityTargetIsValid(Vector3 location)
    {
        return AbilityTargetIsValid(location, out RaycastHit2D[] hitArray);
    }

    // Full function for UseAbilityAtLocation
    public bool AbilityTargetIsValid(Vector3 location, out RaycastHit2D[] hitArray)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, location - transform.position, Vector3.Distance(transform.position, location), CollisionLayer);
        hitArray = hit;
        // Return out if fail conditions are met (target is player, wall is hit)
        if (transform.position == location || (hit.Length > 0 && WallTileInArray(hit) == true))
        {
            return false;
        }
        // Fireball doesn't need an enemy to target
        if (activeAbility == "fireball")
        {
            return true;
        }
        // Crossbow and lighting require an enemy to target
        else if (activeAbility == "crossbow" || activeAbility == "lightning")
        {
            if (hit.Length > 0 && hit[0].collider.gameObject.tag == "Enemy")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        
    }

    public bool WallTileInArray(RaycastHit2D[] hitArray)
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


    public void UseAbilityAtLocation(Vector3 location)
    {
        RaycastHit2D[] hit;
        if (AbilityTargetIsValid(location, out hit))
        {
            if (activeAbility == "crossbow")
            {
                inventory.FireCrossbow(hit[0].collider.transform);
                soundManager.PlaySound("crossbow");
            }
            else if (activeAbility == "fireball")
            {
                List<Vector3> locationList = new List<Vector3>() {location, location + new Vector3(-1,-1,0), location + new Vector3(-1,1,0), location + new Vector3(1,-1,0), location + new Vector3(1,1,0), location + new Vector3(1,0,0), location + new Vector3(-1,0,0), location + new Vector3(0,1,0), location + new Vector3(0,-1,0)};
                List<Transform> targets = GetEnemiesInLocationList(locationList);
                if (targets.Count > 0)
                {
                    inventory.CastFireball(targets);
                    soundManager.PlaySound("castSpell");
                }
                
            }
            else if (activeAbility == "lightning")
            {
                List<Transform> targets = new List<Transform>();

                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider.gameObject.tag == "Enemy")
                    {
                        targets.Add(hit[i].collider.transform);
                    }
                }

                if (targets.Count > 0)
                {
                    inventory.CastLightning(targets);
                    soundManager.PlaySound("castSpell");
                }
                
            }
            
            CancelAiming();
        }
        else
        {
            HUD.CreateFloatingMessage("no target", transform);
            soundManager.PlaySound("noTarget");
        }
        
    }

    public List<Transform> GetEnemiesInLocationList(List<Vector3> locationList)
    {
        List<Transform> enemies = new List<Transform>();
        for (int i = 0; i < locationList.Count; i++)
        {
            // Shoot a ray at location
            Collider2D collider = GetCollision(locationList[i], .2f, CollisionLayer);

            // If collision is with enemy
            if (collider != null && collider.gameObject.tag == "Enemy")
            {
                // Add them to the list
                enemies.Add(collider.transform);
            }  
        }
        return enemies;
    }

    // Returns true if it's player's turn and they have an action left
    public bool PlayerCanUseItem()
    {
        if (TurnSystem.state == TurnState.PLAYERTURN && playerStats.ActionsRemainingThisTurn > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}