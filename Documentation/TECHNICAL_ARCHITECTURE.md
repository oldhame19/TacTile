# TacTile — Technical Architecture

## 1. Engine and Environment Specifications

- **Unity Editor Version:** `6000.3.11f1` (Unity 6.3 preview/release), verified in [ProjectSettings/ProjectVersion.txt](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/ProjectSettings/ProjectVersion.txt).
- **Target OS:** Windows 11 (Development); standalone PC build target with mobile expansion planned for Phase 2.
- **Rendering Pipeline:** Universal Render Pipeline (URP 17.3.0), utilizing [PC_RPAsset.asset](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Settings/PC_RPAsset.asset) and [PC_Renderer.asset](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Settings/PC_Renderer.asset).
- **Input System:** Both legacy Input Manager and the New Input System are enabled (`activeInputHandler: 2` in [ProjectSettings/ProjectSettings.asset](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/ProjectSettings/ProjectSettings.asset)), package `com.unity.inputsystem` v1.19.0.
- **UI Framework:** Unity UI (uGUI v2.0.0) combined with TextMeshPro.
- **Active Git Branch:** `ai-development`.

---

## 2. Project Directory Structure

```
Assets/
├── Materials/            # Surface materials (BoardLight, TileColorOne, TickDark, BackGround)
├── Prefabs/              # Core runtime prefabs (BoardSquare, Tile, Tick)
├── Scenes/
│   ├── MainScene.unity   # Primary gameplay scene (Build Index 0, active)
│   └── SampleScene.unity # Inactive template scene
├── Settings/             # URP configuration assets and volume profiles
├── TextMesh Pro/         # TMP fonts and default resources
├── Scripts/
│   ├── Board/
│   │   ├── BoardGenerator.cs  # 5x5 board instantiation and boost assignment
│   │   └── BoardSpace.cs      # Board cell logic, position tracking, placement receiver
│   ├── Config/
│   │   ├── GameBalanceConfig.cs     # ScriptableObject definition for game constants
│   │   └── GameBalanceConfig.asset  # Serialized asset holding active balance values
│   ├── Game/
│   │   ├── GameDriver.cs         # Main orchestrator, singleton, combat and turn loop
│   │   ├── GameState.cs          # Domain data model for board, tiles, and player state
│   │   ├── TurnStateMachine.cs   # Static phase transition state machine
│   │   ├── LegalMoveGenerator.cs # Static query engine determining legal actions
│   │   ├── ScoreboardUI.cs       # UI script tracking scores and highlighting active player
│   │   └── ClickTester.cs        # Debug raycasting script attached to Main Camera
│   ├── Tile/
│   │   ├── TileSpawner.cs        # Player rack tile instantiation and color balancing
│   │   ├── TileGenerator.cs      # Procedural edge strength generator and tick instantiator
│   │   ├── TileView.cs           # Tile presentation, selection wobble, placement state
│   │   └── TickView.cs           # Individual tick renderer and color application
│   └── Unused/                   # Deprecated prototypes (Board.cs, Space.cs, TileData.cs)
└── _Recovery/            # Auto-saved recovery scene (0.unity) from prior editor session
```

---

## 3. Important Scripts, Classes, and Responsibilities

| Class / File | Type | Architectural Pattern | Primary Responsibilities |
| :--- | :--- | :--- | :--- |
| [GameDriver.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Game/GameDriver.cs) | `MonoBehaviour` | Singleton Controller | Central coordinator with `[DefaultExecutionOrder(1000)]`. Populates `GameState`, handles tile selection, coordinates placement, executes combat calculations, and drives turn phases. |
| [GameState.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Game/GameState.cs) | Pure C# Class | Domain Model | Holds runtime game state: `CurrentPlayer`, `CurrentPhase`, `SelectedTile`, `BoardSpaces`, `AllTiles`, and `CompletedTurns`. Provides helper queries (`GetPlayerTiles`, `GetEmptySpaces`, `IsGameOver`). |
| [TurnStateMachine.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Game/TurnStateMachine.cs) | `static class` | State Machine Logic | Controls valid transitions: `BeginTurn` $\to$ `SelectTile` $\to$ `PlaceTile` $\to$ `EndTurn` $\to$ `BeginTurn`. Toggles active player and increments completed turn counter. |
| [LegalMoveGenerator.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Game/LegalMoveGenerator.cs) | `static class` | Rule Engine | Computes legal moves for a given player based on turn phase, returning a `List<LegalAction>`. |
| [BoardGenerator.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Board/BoardGenerator.cs) | `MonoBehaviour` | World Builder | Instantiates the $5 \times 5$ board grid at `Start()`, calculates centered world coordinates, and distributes the 1 neutral and 6 colored boost spaces. |
| [BoardSpace.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Board/BoardSpace.cs) | `MonoBehaviour` | View / Component | Sits on each cell prefab. Stores `Row`, `Column`, `OccupyingTile`, `BoostType`. Listens for clicks via `OnMouseDown()`, notifies `GameDriver`, and handles tile reparenting upon placement. |
| [TileSpawner.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Tile/TileSpawner.cs) | `MonoBehaviour` | Spawner | Spawns 13 tiles for each player at `Start()`. Ensures balanced tick colors across each player's rack. |
| [TileGenerator.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Tile/TileGenerator.cs) | `MonoBehaviour` | Procedural Generator | Generates random edge strengths ($0-4$) summing to $7-9$ total ticks. Instantiates `Tick.prefab` objects around tile edges. |
| [TileView.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Tile/TileView.cs) | `MonoBehaviour` | View / Component | Manages visual representation (white for P1, black for P2), tracks placement status, runs selection wobble coroutine (`Jiggle()`), and listens for clicks via `OnMouseDown()`. |
| [TickView.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Tile/TickView.cs) | `MonoBehaviour` | View / Component | Colors individual tick meshes using `MaterialPropertyBlock` for `Red`, `LightBlue`, or `Yellow`. |
| [ScoreboardUI.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Game/ScoreboardUI.cs) | `MonoBehaviour` | View / UI Controller | Polls placed tile ownership each frame in `Update()`, updates TMP score text, and highlights the active player's name. |
| [GameBalanceConfig.cs](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Config/GameBalanceConfig.cs) | `ScriptableObject` | Data Container | Holds configurable balance values (`boardSize = 5`, `boostsPerColor = 2`, `tilesPerPlayer = 13`, tick constraints, boost bonuses). |

---

## 4. Game Initialization Sequence

Because multiple systems instantiate objects at runtime, initialization order is strictly governed by Unity's execution lifecycle:

```mermaid
sequenceDiagram
    autonumber
    participant Unity as Unity Lifecycle
    participant BG as BoardGenerator (Start)
    participant TS as TileSpawner (Start)
    participant GD as GameDriver (Start, Order 1000)
    participant GS as GameState
    participant SM as TurnStateMachine

    Note over Unity,GD: Frame 0 Initialization
    Unity->>BG: Start()
    BG->>BG: GenerateBoard(): Instantiates 25 BoardSquare prefabs
    BG->>BG: AssignBoostSpaces(): 1 center Neutral, 6 Colored boosts

    Unity->>TS: Start()
    TS->>TS: SpawnPlayerTiles(): Instantiates 13 tiles per player
    TS->>TS: Procedural Strengths & Tick Generation on each tile

    Note over GD: GameDriver has [DefaultExecutionOrder(1000)]
    Unity->>GD: Start() -> InitializeGame()
    GD->>GS: new GameState()
    GD->>GD: FindObjectsOfType<BoardSpace>() -> state.BoardSpaces (25)
    GD->>GD: FindObjectsOfType<TileView>() -> state.AllTiles (26)
    GD->>GD: tile.Initialize(owner, this) on all tiles
    GD->>GD: AdvanceAutomaticPhases()
    GD->>SM: Advance(state): BeginTurn -> SelectTile
```

1. **`BoardGenerator.Start()` (Execution Order 0):**
   - Reads `GameBalanceConfig.asset` (`boardSize = 5`, `squareSpacing = 2.0`).
   - Calculates board center offset: `boardOffset = (5 - 1) * 2 / 2f = 4.0f`.
   - Loops through 25 cells, instantiates [BoardSquare.prefab](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Prefabs/BoardSquare.prefab) at `(x, 0.3f, z)`, and initializes coordinates via [BoardSpace.Initialize(row, col)](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Board/BoardSpace.cs#L58-L62).
   - Identifies center cell `[2, 2]` (a fully playable space) and sets `BoostType.Neutral` (purple).
   - Shuffles remaining 24 spaces and assigns `BoostType.Red` (2), `BoostType.LightBlue` (2), and `BoostType.Yellow` (2).
2. **`TileSpawner.Start()` (Execution Order 0):**
   - Generates balanced colors for 13 tiles per player via `CreateBalancedColors()`.
   - Spawns Player 1 row at $Z = -7.3$ and Player 2 row at $Z = 7.3$.
   - Calls [TileGenerator.InitializeTickColor()](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scripts/Tile/TileGenerator.cs#L25-L39), which generates side strengths totaling 7–9 ticks and instantiates [Tick.prefab](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Prefabs/Tick.prefab) instances along tile edges.
3. **`GameDriver.Start()` (Execution Order 1000):**
   - Instantiates a new `GameState`.
   - Calls `FindObjectsOfType<BoardSpace>()` to populate `state.BoardSpaces`.
   - Calls `FindObjectsOfType<TileView>()` to populate `state.AllTiles` and assigns itself (`this`) to each tile via `tile.Initialize(tile.Owner, this)`.
   - Calls `AdvanceAutomaticPhases()`, which invokes `TurnStateMachine.Advance(state)`, transitioning the starting state from `BeginTurn` to `SelectTile`.

---

## 5. Scene Objects and Inspector References

Inspected from [Assets/Scenes/MainScene.unity](file:///C:/Users/oldha/OneDrive/Documents/CST-410/TacTile/Assets/Scenes/MainScene.unity):

```
Hierarchy
├── Main Camera (Transform, Camera, AudioListener, URP Additional Camera Data, ClickTester)
├── Directional Light (Transform, Light, URP Additional Light Data)
├── Global Volume (Transform, Volume with DefaultVolumeProfile)
├── BoardBase (Transform, BoxCollider)
├── Board (Transform, BoardGenerator)
│   ├── boardSquarePrefab -> Assets/Prefabs/BoardSquare.prefab
│   ├── balanceConfig     -> Assets/Scripts/Config/GameBalanceConfig.asset
│   └── squareSpacing     -> 2.0
├── TileManager (Transform, TileSpawner)
│   ├── tilePrefab        -> Assets/Prefabs/Tile.prefab
│   ├── balanceConfig     -> Assets/Scripts/Config/GameBalanceConfig.asset
│   ├── tileSpacing       -> 2.25
│   ├── playerOneZ        -> -7.3
│   └── playerTwoZ        -> 7.3
├── GameManager (Transform, GameDriver)
│   └── balanceConfig     -> Assets/Scripts/Config/GameBalanceConfig.asset
└── Canvas (RectTransform, Canvas, CanvasScaler, GraphicRaycaster)
    └── ScoreboardPanel (RectTransform, Image, ScoreboardUI)
        ├── player1NameText   -> Player1Name (TMP_Text)
        ├── player1ScoreText  -> Player1Score (TMP_Text)
        ├── player2NameText   -> Player2Name (TMP_Text)
        ├── player2ScoreText  -> Player2Score (TMP_Text)
        ├── activePlayerColor -> Yellow/Gold (1.0, 0.8, 0.2, 1.0)
        └── inactivePlayerColor -> White (1.0, 1.0, 1.0, 1.0)
```

---

## 6. Execution Flows

### 1. Tile Selection Flow
```
User clicks Tile (3D BoxCollider)
  │
  ▼
TileView.OnMouseDown()
  │
  ▼
GameDriver.TrySelectTile(TileView)
  ├── IF CurrentPhase == PlaceTile AND tile is unplaced AND belongs to CurrentPlayer:
  │     - Deselect previous tile: state.SelectedTile.SetSelected(false)
  │     - Select new tile: state.SelectedTile = tile; tile.SetSelected(true) [Jiggle animation]
  │     - Remain in PlaceTile phase
  │
  └── ELSE IF CurrentPhase == SelectTile:
        - Query LegalMoveGenerator.GetLegalActions(state, CurrentPlayer)
        - IF LegalAction(SelectTile, tile) exists:
            - state.SelectedTile = tile; tile.SetSelected(true)
            - TurnStateMachine.Advance(state) -> transitions to PlaceTile
```

### 2. Tile Placement and Reparenting Flow
```
User clicks BoardSpace (3D BoxCollider)
  │
  ▼
BoardSpace.OnMouseDown()
  │
  ▼
GameDriver.TryPlaceTile(BoardSpace)
  │
  ▼
Query LegalMoveGenerator.GetLegalActions(state, CurrentPlayer)
  │
  ▼
IF LegalAction(PlaceTile, space) exists:
  │
  ├── BoardSpace.TryPlaceTile(placedTile):
  │     - OccupyingTile = placedTile
  │     - placedTile.MarkAsPlaced(space) [IsPlaced = true, SetSelected(false) stops Jiggle]
  │     - placedTile.transform.SetParent(space.transform)
  │     - placedTile.transform.position = space.position + (Vector3.up * 0.25f)
  │     - placedTile.transform.rotation = space.rotation
  │
  ├── GameDriver.ResolveCaptures(placedTile, space) [Executes combat against neighbors]
  │
  └── TurnStateMachine.Advance(state) [Transitions PlaceTile -> EndTurn]
        │
        ▼
      GameDriver.AdvanceAutomaticPhases()
        - Advances EndTurn: CompletedTurns++, toggles CurrentPlayer, clears SelectedTile
        - Advances BeginTurn -> transitions into SelectTile for next player
```

### 3. Combat and Capture Flow
```
GameDriver.ResolveCaptures(placedTile, placedSpace)
  │
  ├── Check Top neighbor:    [Row + 1, Col] -> Attacker Top    vs Defender Bottom
  ├── Check Right neighbor:  [Row, Col + 1] -> Attacker Right  vs Defender Left
  ├── Check Bottom neighbor: [Row - 1, Col] -> Attacker Bottom vs Defender Top
  └── Check Left neighbor:   [Row, Col - 1] -> Attacker Left   vs Defender Right
        │
        ▼
  For each neighbor that exists, is occupied, and belongs to OPPONENT:
    - AttackingStrength = GetEffectiveStrength(attacker, attackingSide)
    - DefendingStrength = GetEffectiveStrength(defender, defendingSide)
        │
        ├─ Base strength from TileGenerator (0..4 ticks)
        ├─ If on Neutral Boost Space: +1 bonus (neutralBoostBonus)
        └─ If on Matching Color Boost Space: +1 bonus (matchingColorBoostBonus)
        │
        ▼
    IF AttackingStrength > DefendingStrength:
      - Defender captured!
      - defenderTile.SetOwner(attacker.Owner)
      - MaterialPropertyBlock updates defender mesh to attacker's color
```

### 4. Live Scoring Update Flow
```
ScoreboardUI.Update() [Runs every frame]
  │
  ▼
Queries GameDriver.Instance.State
  │
  ▼
Loops through state.AllTiles:
  - If tile is placed AND Owner == Player1 -> player1Score++
  - If tile is placed AND Owner == Player2 -> player2Score++
  │
  ▼
If scores changed: update TMP_Text components
If active player changed: update player name text colors
```

---

## 7. Verified Facts vs. Assumptions

### Verified Implementation Facts
1. **No Standalone CombatManager or TurnManager:** Combat logic resides entirely in `GameDriver.cs`. Turn logic is split between static `TurnStateMachine.cs` and `GameDriver.cs`.
2. **Missing Victory Loop:** `GameState.cs` contains an `IsGameOver` property (lines 57–65), but it is never invoked or referenced anywhere else in the project. The game has no game-over screen or restart mechanic.
3. **Intentional Tile Count:** Each player receives 13 tiles. The board contains 25 playable spaces, including the center neutral purple boost space. Therefore, when the board is full, exactly one tile remains unused overall. Which player retains that tile depends on placement order.
4. **Input Architecture:** Uses legacy `OnMouseDown()` on 3D physics BoxColliders. It does not use the New Input System actions despite the package being installed.
5. **Scoreboard Polling:** `ScoreboardUI.Update()` counts placed tiles every frame via a `foreach` loop over `state.AllTiles`.

### Assumptions (Identified for Verification)
1. **Input Touch Behavior on Mobile:** Unity emulates mouse clicks from single touches on mobile devices, which allows basic `OnMouseDown()` taps to work in standard scenarios. However, this has not been validated on a physical device build and lacks multi-touch or touch-cancellation handling.
2. **Color Palette Mismatch in Prefabs:** In [BoardSquare.prefab](file:///C:/Users/oldha\OneDrive\Documents\CST-410\TacTile\Assets\Prefabs\BoardSquare.prefab#L130-L131) and [Tick.prefab](file:///C:/Users/oldha\OneDrive\Documents\CST-410\TacTile\Assets\Prefabs\Tick.prefab#L130-L131), `yellowColor` is serialized as lime green (`0.196, 0.804, 0.196`) and `lightBlueColor` as pure blue (`0, 0, 1`). It is assumed this was an intentional visual choice in the Unity Inspector that overrides the C# default constants.
