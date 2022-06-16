# Princess Tower Escape

## Gameplay Video:
https://www.youtube.com/watch?v=P22JCZOapfM

## Overview
Princess Tower Escape is a turn-based hack-and-slash game about a princess escaping a tower. Navigate your way through three levels, evading and fighting guards with the spells and resources you pick up along the way.

## Aim of the game
To get to the exit of the final level and escape the tower. Enemy guards will try to stop the player. The player can find crossbow bolts, potions and spell scrolls to help them. Defeated guards have a chance to drop a health potion.

## Attacks and spells
### Melee
The player can damage guards by bashing into them (2 damage).

### Crossbow
A ranged single-target attack (4 damage).

### Fireball
A ranged attack that hits everything in a 3x3 grid (4 damage).

### Lightning
A ranged attack that hits everything in a line (4 damage).

### Shield
A temporary shield that prevents damage to the player (20% of maximum health).

### Health Potion
Heals the player for 20% of maximum health.

## Scenes

### StartMenu
A start screen containing a play button to start the game and a quit button to end it.

### Levels 1, 2 and 3
#### TurnSystem GameObject
Handles the game states and player and enemy turn-taking.

#### InventoryManager
Keeps track of player inventory and includes methods to add, remove and use items.

#### SoundManager
Holds references to audio clips and contains a method for other scripts to play them.

#### Grid
Contains the tiles of each level. There is a tilemap with a collider that contains walls and torches. There is a second tilemap without a collider that contains doorways.

#### Camera
A static camera with an audio listener.

#### UI
* Player Resources
    * Player health
    * Player temporary health (added by shield scrolls)

* Pause button -- pauses the game, which opens the pause menu. From here the player can resume the game, return to the start menu or quit the game.

* Action Bar -- tracks items in player's inventory. Buttons can be clicked to use each item.

* Messages -- message boxes are displayed at the beginning of each level. When the player loses and when the player wins the game.

* PlayerSpawnPoint -- the location where the player is spawned.

* DoorCollider -- when the player occupies this tile the level is completed.

* EnemySpawners -- spawns a variable number of enemies in a 3x3 grid.

* ItemSpawners -- spawns an item in this location. There is a variable chance to spawn one of a range of items and a chance to spawn nothing at all.

## Game States
* START -- initial state at the beginning of each level. Ends when message is dismissed.

* PLAYERTURN -- the player can move one square or use an action.

* ENEMYTURN -- enemies can move one square or use an action.

* PAUSE -- the game is paused. A message is displayed letting the player resume the game, return to the menu or quit. This can only occur during the player's turn or the enemies' turn.

* WIN -- the game is won. A message is displayed letting the player return to the menu or quit.

* LOSE -- the game is lost. A message is displayed letting the player return to the menu or quit.


## Design Decisions

### Inspiration
My main inspiration for this project was old school roguelike games. I'm a huge fan of pixel art and I'd already seen the Kenney 1 Bit Pack tileset and wanted to make something with it. The simple style meant that it was easy to customize the tiles and add new ones here and there without it being obvious.

### Randomness
One of the things I would have liked to implement was procedurally-generated levels, but it ended up being too time-consuming. I decided instead to have a fixed layout for each level but to randomise the number of enemies in each location and the items that spawned. I hoped that this would encourage the player to take different routes through the levels and weigh up the risks and rewards of going after certain resources. I hoped that allowing the player to see the entire level at once instead of implementing a fog of war (which is a popular feature of many traditional roguelikes) would add to this strategic puzzle-like gameplay.

### Enemies
I spent a lot of time tinkering with the enemy AI and there are still things I would like to improve. The pathfinding is quite simple and if I had the time I would definitely want to implement A* pathfinding. The guards do have the ability to manoeuvre around each other and will alert the other members of their squad if the player attacks them. I wanted the guards to give a definite sense of threat, and watching a large group of them swarm into action when the player hits one with a ranged attack is quite unnerving.

---
## Script Breakdown
---
### TurnSystem
(TurnSystem GameObject)

#### Start
Sets the game state to start, displays the level beginning message and spawns the player at the player spawn location.

#### RegisterEnemy(GameObject enemy)
Adds enemy to a list of enemies in play. Called by the enemy spawners on each enemy.

#### StartPlayerTurn()
Calls the player manager StartTurn method. Called by the start button at the beginning of each level.

#### EndPlayerTurn()
Calls StartEnemyTurn. Called by the player manager.

#### StartEnemyTurn()
If there are enemies in play it changes the game state to ENEMYTURN and sets the current enemy to the first one in the EnemiesInPlay list.

If there aren't any enemies in play it jumps back to the player's turn (StartPlayerTurn()).

#### EndEnemyTurn()
If this is the last enemy in the EnemiesInPlay list it changes to the player's turn by calling StartPlayerTurn()

Otherwise it moves to the next enemy by calling DoNextEnemyTurn()

#### DoNextEnemyTurn()
Changes the current enemy to the next enemy in the EnemiesInPlay list.

#### PlayerDefeated()
Changes the game state to LOSE and invokes a callback.

#### PlayerWin()
Changes the game state to WIN and invokes a callback.

#### PauseGame()
Changes the game state to PAUSE and invokes a callback.

---
### SoundManager
(SoundManager GameObject)

Loads sound clips in the Resources folder and provides a function for other scripts to play them.

---
### EnemyAI
(Enemy and BossEnemy GameObjects)

#### FixedUpdate()
The enemy will only act if the game state is ENEMYTURN and it is the TurnSystem's current enemy.

If it has moved one tile it will call EndTurn().

If it's currently animating between two tiles it will keep moving using Vector3.MoveTowards()

If it hasn't started moving it gets the distance between it and the player.

If it isn't currently aggroed it checks to see whether it should.

If the player is too far away it ends its turn.

If the player is close enough and the player is in line of sight it is aggroed, it alerts its squad and it moves one tile towards the player with MoveTowardsPosition(Vector2 a, Vector2 b)

If the player is not in line of sight it ends its turn.

If it is currently aggroed it checks to see whether it's close enough to attack.

If it is it calls HandleCombat(Transform target) and ends its turn.

If it isn't it checks whether it should keep chasing the player.

If the player is closer or at the disengage distance it keeps chasing by calling MoveTowardsPosition(Vector2 a, Vector2 b)

If the player is further than the disengage distance the enemy will de-aggro and end its turn.

#### StartTurn()
A placeholder method in case we want to perform any logic at the beginning of the enemy's turn.

#### EndTurn()
Calls the TurnSystem EndEnemyTurn().

#### bool CheckCollision(Vector3 target)
Checks whether there would be a collision if the enemy moved to that square.

#### HandleCombat(Transform target)
Calls the enemy's EnemyStats DealDamage(int amount, Transform target) method.

#### GetTileDistance(Vector2 a, Vector2 b)
Calculates the distance in tiles between two positions. This is how the enemy calculates how far away the player is.

#### AggroSelf()
This makes the enemy chase the player. If the player is further away than the enemy's disengage distance it updates it to the player's distance + 4. This is so that if a player hits an enemy with a ranged attack from a long way away the enemy doesn't immediately disengage.

#### AlertSquad()
This loops through the enemy's squad and aggros them by calling AggroSelf() on each one.

#### bool PlayerInLineOfSight()
This casts a ray between the enemy and the player's position and returns true if it doesn't collide with anything other than the player.

#### MoveTowardsPosition(Vector2 a, Vector2 b)
Logic to choose which tile the enemy will move to next when it is chasing the player. It does this by setting the Vector3 movePosition variable.

xDistance and yDistance are floats that store the exact x and y distances between the enemy and the player.

newPositionX and newPositionY hold x and y positions that the enemy will move to. Because the player and enemies don't move diagonally only one of these can be set to a non-zero value.

left, right, up and down are Vector2 convenience variables set to the tile positions immediately to the left, right, above and below the enemy's current position.

The enemy tracks whether it can currently move left, right, up or down by calling CheckCollision(Vector3 target) on each one.

If the player is to the left, right, up or down of the enemy and the enemy can go that way newPositionX and newPositionY are set to left, right, up or down.

If the player is directly up or down only newPositionY will be set.

If the player is directly left or right only newPosistionX will be set.

If the player is up and left both newPositionY and newPositionX will be set.

If both newPositionX and newPositionY are set to a non-zero value we'll have to pick one of them later so the enemy doesn't move diagonally.

If neither newPositionX or newPositionY were set because the way was blocked the enemy will try to move in a lateral direction (by setting movePosition to that direction).
If it can't move laterally it will end its turn.

If only one of newPositionX or newPositionY were set the enemy will move in that direction (by setting movePosition to that direction).

If both newPositionX and newPositionY are set it picks one at random and will move in that direction (by setting movePosition to that direction).

---
### EnemyStats
(Enemy and BossEnemy GameObjects)

Tracks the enemy's name, attack damage, maximum health and current health. Also its aggro distance (how close the player must get before the enemy chases them) and its disengage distance (how far away the player must be before the enemy stops chasing them).

#### UpdateHP(int amount)
Adds the amount to the enemy's current HP. Stops the enemy's HP from going below 0 or above its maximum.

If the enemy drops to 0 it alerts its squad, is removed from the TurnSystem EnemiesInPlay list and its squad list. It calls its LootTable DropItem() method to see if it drops an item. Then the enemy game object is disabled.

If the enemy survives, it aggros by calling its EnemyAI AggroSelf() method and alerts its squad.

#### DealDamage(int damage, Transform target)
Deals damage to the player by calling the PlayerStats method TakeDamage(int amount). The method expects a negative number.

#### LootTable
(Enemy and BossEnemy GameObjects)

When an enemy dies this determines whether an item will drop. One item and a percentage chance to drop can be specified.

#### Inventory
(InventoryManager GameObject)

Keeps track of which items the player currently has and contains methods for using them.

#### AddItem(string type)
Adds the item to the player's inventory and creates a floating message (+1 potion/+3 bolts/+1 scroll). When the player picks up crossbow bolts a number between 1 and 3 is chosen.

#### AimCrossbow() / AimFireball() / AimLightning
(Activated by action bar button)

If there are bolts or scrolls left and the player has actions left it calls the PlayerManager StartAiming(string ability) method.

#### FireCrossbow(Transform target)
Called by PlayerManager.UseAbilityAtLocation(Vector3 location). Deals damage to a single target with the PlayerStats DealDamage(int damage) method and uses a player action with PlayerStats.UseAction(). Reduces the number of crossbow bolts by one.

#### CastFireball(List<Transform> targets)
Called by PlayerManager.UseAbilityAtLocation(Vector3 location). Loops through the targets and deals damage to each one with the PlayerStats.DealDamage(int damage) and uses a player action with PlayerStats.UseAction(). Reduces the number of fireball scrolls by one.

#### CastLightning(List<Transform> targets)
Called by PlayerManager.UseAbilityAtLocation(Vector3 location). Loops through the targets and deals damage to each one with the PlayerStats.DealDamage(int damage) and uses a player action with PlayerStats.UseAction(). Reduces the number of lightning scrolls by one.

#### UseShieldScroll()
(Activated by action bar button)

If there are shield scrolls left and the player has actions left it calls the PlayerStats UpdateTempHP(float percentage) method to increase the player's temporary hit points. It also uses an action with PlayerStats.UseAction() and reduces the number of shield scrolls by one.

#### UseHealthPotion()
(Activated by action bar button)

If there are health potions left and the player has actions left it calls the PlayerStats Heal(float percentage) method to increase the player's current hit points. It also uses an action with PlayerStats.UseAction() and reduces the number of health potions by one.

---

### PlayerManager
(Player GameObject)

#### Update()
Handles player ranged abilities.

If the game state is PLAYERTURN and the player is currently aiming it checks for player input.

If the left mouse button is pressed it calls UseAbilityAtLocation(Vector3 location) at the target square's location.

If the right mouse button or the direction keys are pressed aiming is cancelled with CancelAiming().

#### FixedUpdate()
Handles player grid-based movement.

Only functions while the game state is PLAYERTURN.

If the player has moved one tile it ends the player's turn with EndTurn().

If the player has started moving it continues animating the player sprite towards the movePosition with Vector3.MoveTowards.

If the player hasn't started moving it checks to see whether the horizontal or vertical direction keys have been pressed. The player can't move diagonally so if both horizontal and vertical keys are pressed horizontal will be preferred. If the direction keys have been pressed it calls HandleMovement(Vector3 newPosition) passing in the relevant location.

If the direction keys are pressed it uses up a player action by calling PlayerStats.UseAction(). This is so that the player can't use the action bar while the player sprite is animating.

#### HandleMovement(Vector3 newPosition)
Takes a Vector3 and checks it for colliders with GetCollision(Vector3 target, float radius, LayerMask collisionLayer).

If there is no collider it sets the player's movePosition to newPosition. FixedUpdate() will then animate the player sprite moving to this tile.

If there is an enemy in that location it calls HandleCombat(Transform target) and ends the player's turn.

If the player collides with anything else (e.g. a wall) the player doesn't move and their turn isn't ended.

#### HandleCombat(Transform target)
Called by HandleMovement(Vector3 newPosition) when a player collides with an enemy.

Passes target and player's attack damage to PlayerStat.DealDamage(int damage, Transform target).

#### Collider2D GetCollision(Vector3 target, float radius, LayerMask collisionLayer)
Helper function for HandleMovement() and GetEnemiesInLocationList(List<Vector3> locationList).

Checks a location for any tile on the collision layer (walls and enemies).

#### StartTurn()
Called by TurnSystem.StartPlayerTurn().

Resets player startPosition to the player's current position. Makes sure player isn't aiming. Calls PlayerStats.ReplenishActions() to set the player's remaining actions to their maximum. Updates the game state to PLAYERTURN.

#### EndTurn()
Called by FixedUpdate() if the player has moved one tile.

Called by HandleMovement(Vector3 newPosition) if melee combat occurs.

Cancels player aiming. Calls PlayerStats UseAllActions() method and TurnSystem EndPlayerTurn() method.

#### StartAiming(string ability)
Called by Inventory methods AimCrossbow(), AimFireball and AimLightning().

Calls the HUD methods ActivateAimingReticule() and DisableActionBarButtons(). Creates a floating message "aiming".

#### CancelAiming()
Called by Update() when player cancels an active ability by right-clicking or moving.

Called by UseAbilityAtLocation(Vector3 location) once an ability is used.

Calls the HUD methods DeactivateAimingReticule() and EnableActionBarButtons().

#### bool AbilityTargetIsValid(Vector3 location)
An overload method for AbilityTargetIsValid(Vector3 location, out RaycastHit2D[] hitArray).

Called by TargetSquare script so it can get just the bool without the array.

#### bool AbilityTargetIsValid(Vector3 location, out RaycastHit2D[] hitArray)
Called by UseAbilityAtLocation(Vector3 location).

Uses Physics2D.RaycastAll() to get all colliders between the player location and the target square.

Returns false if the player targets themselves or if it hits a wall tile (using WallTileInArray(RaycastHit2D[] hitArray)).

For fireball these are the only conditions.

For crossbow and lightning it also has to hit at least one enemy (collider with Enemy tag).

#### bool WallTileInArray(RaycastHit2D[] hitArray)
A helper function for AbilityTargetIsValid(Vector3 location, out RaycastHit2D[] hitArray).

Takes an array of RaycastHit2D. Returns true if a collider with the tag Wall is in the array.

#### UseAbilityAtLocation(Vector3 location)
Called by Update() when player uses a ranged ability.

Calls AbilityTargetIsValid(Vector3 location, out RaycastHit2D[] hitArray) to check whether to proceed.

If the target is valid it checks the current active ability (crossbow, fireball or lightning).

For crossbow the first collider in the RaycastHit2D array is passed to Inventory.FireCrossbow(Transform target).

For fireball all locations in a 3x3 grid surrounding the location parameter are passed to GetEnemiesInLocationList(List<Vector3> locationList). If more than zero targets are returned they are passed to Inventory.CastFireball(List<Transform> targets).
For lightning all colliders in the RaycastHit2D array with the tag Enemy are passed to Inventory.CastLightning(List<Transform> targets).

If the target is invalid a floating message with "no target" is displayed.

#### List<Transform> GetEnemiesInLocationList(List<Vector3> locationList)
Takes a list of Vector3 and returns a list of any enemies occupying those locations. Called by UseAbilityAtLocation(Vector3 location).

#### bool PlayerCanUseItem()
Returns true if the game state is PLAYERTURN and the player has actions remaining this turn. Called by the Inventory AimCrossbow(), AimFireball, AimLightning, UseShieldScroll() and UseHealthPotion() methods.

---

### PlayerStats
(Player GameObject)

Tracks the player's name, attack damage, maximum health, current health and temporary health. Also the number of actions they have per turn.

At start the player's health is set to max if this is the first level.

#### TakeDamage(int damage)
The damage is a negative number. The damage is reduced by any temporary hit points provided by a shield scroll. The temporary hit points are also reduced by any damage they prevent.

Then UpdateHP(int amount) is called passing in the remaining damage (this may be zero). A floating damage number is created and if the player's health falls to zero the TurnSystem PlayerDefeated() method is called.

#### UpdateTempHP(float percentage)
Sets the player's temporary hit points to a percentage of their max health (currently 20%). This is called when the player activates a shield scroll.

#### Heal(float percentage)
Heals the player for a percentage of their maximum health (currently 20%). This is called when the player uses a potion. The amount healed is clamped to the player's maximum health so overhealing isn't possible. Calls UpdateHP(amount) to actually update the player's current hit points.

#### DealDamage(int damage, Transform target)
Used to deal damage to enemies. It creates a floating damage number over the target and calls its EnemyStats UpdateHP(int amount) method.

#### UseAction()
Reduces the player actions remaining this turn by one and if there are none left it calls the HUD method DisableActionBarButtons() to disable the action bar at the bottom of the screen. This method is used by the PlayerManager when the player moves one tile and by the Inventory when the player uses an item.

#### ReplenishActions()
Sets the player actions remaining this turn to their maximum value (currently 1) and calls the HUD method EnableActionBarButtons() to enable the action bar at the bottom of the screen. This method is called by the PlayerManager at the start of the player's turn.

#### UseAllActions()
This sets the player actions remaining this turn to zero. This is called by PlayerManager at the end of the player's turn.

#### UpdateHP(int amount)
Updates the player's health by adding the amount. The amount may be positive for healing or negative for damage. The player's hit points can't go above their max value or below zero. If they are reduced to zero the player game object is set to inactive to hide it. TakeDamage(int damage) handles the logic for player death.

---
### EnemySpawner
(EnemySpawner GameObjects)

Spawns a number of enemies between the minimum and maximum range. Places them within a 3x3 grid centred on the EnemySpawner game object.

Calls the TurnSystem RegisterEnemy(GameObject enemy) method to add each enemy to the EnemiesInPlay list.
Adds each enemy to a squad list and gives each enemy's EnemyAI script a reference to that list so it can call AlertSquad() when it engages with the player.

---
### Exit
(DoorCollider GameObject)

If the player occupies the same square it turns off the collider and loads the next level. If this is the last level the player wins.

---
### ItemPickup
(Item GameObjects)

If the player occupies the same square it turns off the item sprite and collider and adds the item to the player's inventory.

---
### ItemSpawner
(ItemSpawner GameObjects)

Spawns one of a list of items according to their probability weights. Also has a chance to spawn nothing.

---
### HUD
(UI GameObject)

Handles the logic for the UI.

On start subscribes to the TurnSystem OnWin, OnLose and OnPause events and will show a message box according to the current game state.

#### DismissMessage()
Called on the Start button at the start of the level. It dismisses the message and calls the TurnSystem method StartPlayerTurn() to begin the player's turn.

Called on the Resume button in the pause menu. It dismisses the menu.

#### ShowMessage()
Shows a message box with menu buttons depending on the game state.

#### ActivateAimingReticule(string ability)
Called by the PlayerManager StartAiming(string ability) method. It hides the cursor and creates either a TargetSquare or TargetGrid game object depending on the ability.

#### DeactivateAimingReticule()
Destroys the TargetSquare or TargetGrid and makes the cursor visible.

#### Vector3 GetAbilityTargetPosition()
Called by PlayerManager when player uses a ranged ability. Returns the current location of the TargetSquare or TargetGrid.

#### UpdateHPText()
Updates the UI to show the player's current and maximum health. Called by PlayerStats at the beginning of the level and when the TakeDamage(int damage) and Heal(float percentage) methods are called.

#### UpdateTempHPText()
Updates the UI to show the player's current temporary hit points. If the number is greater than zero the temporary hit point icon is displayed (blue shield). This method is called by PlayerStats.TakeDamage(int damage) and PlayerStats.UpdateTempHP(float percentage).

#### DisableActionBarButtons()
Loops through the action bar buttons at the bottom of the screens and makes them non-interactable. This is called by the PlayerManager StartAiming(string ability) method and the PlayerStats UseAction() and UseAllActions() methods.

#### DisableActionButtonIfItemsZero(Button button, int inventoryItem)
Sets the button to not interactable but only if there are none of that item left. Called by UpdateCrossbowBoltNumberText(), UpdateFireballScrollNumberText(), UpdateLightningScrollNumberText(), UpdateShieldScrollNumberText() and UpdateHealthPotionNumberText().

#### EnableActionButtonIfItemsLeft(Button button, int inventoryItem)
Helper method for EnableActionBarButtons(). Enables an action bar button if the number of items is greater than zero. This ensures that a button is only re-enabled if it's actually usable. Called by UpdateCrossbowBoltNumberText(), UpdateFireballScrollNumberText(), UpdateLightningScrollNumberText(), UpdateShieldScrollNumberText() and UpdateHealthPotionNumberText().

#### EnableActionBarButtons()
Sets the action bar buttons to interactable by calling EnableActionButtonIfItemsLeft(Button button, int inventoryItem) on each one. This means that each button is only re-enabled if it's usable. Called by PlayerManager.CancelAiming() and PlayerStats.ReplenishActions().

#### UpdateCrossbowBoltNumberText() / UpdateFireballScrollNumberText() / UpdateLightningScrollNumberText() / UpdateShieldScrollNumberText() / UpdateHealthPotionNumberText()
Updates the UI to show the current number of items in the player's inventory. Calls both EnableActionButtonIfItemsLeft(Button button, int inventoryItem) and DisableActionButtonIfItemsZero(Button button, int inventoryItem) so that the button is made non-interactable if it is reduced to zero and is made interactable if it increases from zero.

#### UpdateActionBarText()
Called by Start()

Calls:
* UpdateCrossbowBoltNumberText()
* UpdateFireballScrollNumberText()
* UpdateLightningScrollNumberText()
* UpdateShieldScrollNumberText()
* UpdateHealthPotionNumberText()

#### Floating Text Methods
* CreateFloatingDamageTakenNumber(int amount, Transform parent)
* CreateFloatingDamageDealtNumber(int amount, Transform target)
* CreateFloatingHealNumber(int amount, Transform parent)
* CreateFloatingMessage(string message, Transform parent)

All four functions instantiate the relevant floating number/message prefab, set its number or text and destroy it after a short amount of time has elapsed.

---
### TargetSquare
(TargetSquare and TargetGrid GameObjects)

Controls the behaviour of the ranged ability target square and target grid.
While the TargetSquare or TargetGrid game objects are active they follow the mouse and snap to the grid.

There is a line renderer component attached to both game objects. On start both positions of the line are set to the player's location. On update the second position is updated to the position of the target square.

On update it calls the PlayerManager AbilityTargetIsValid(Vector3 location) method to check if the target is valid for the ability the player is aiming. If the location is valid the square and the line change to the valid colours. If the location isn't valid they change to the invalid colours.

---
### Floating Text
#### FloatingDamageDealt, FloatingDamageTaken, FloatingHealingReceived, FloatingMessage
(FloatingText GameObjects)

Moves the FloatingText GameObject slowly up or down depending on the script.

---
### PauseMenu
(PauseButton GameObject)

When the button is pressed and the Game State is either PLAYERTURN or ENEMYTURN it pauses the game (PAUSE state).

---
### QuitGame
(QuitButton GameObject)

Quits the game when the button is pressed.

---
### ReturnToMenu
(MenuButton GameObject)

Returns to the Start Menu when the button is pressed.

---
### StartGame
(PlayButton GameObject)

Loads the first level when the button is pressed.

---
### Tooltip
(QuickBar Buttons)

Generates a tooltip when the player hovers over the button.

---
### TooltipName and TooltipText
(Tooltip GameObject)

Populates the Tooltip prefab with the name and text specified in the script.

---
### HotKey
(QuickBar Buttons and PauseButton)

Assigns a keyboard shortcut to the button.