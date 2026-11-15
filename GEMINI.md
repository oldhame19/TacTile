\# TacTile – Project Instructions



\## 1. Project Overview



TacTile is an existing Unity-based, two-player strategy game combining elements of Othello and Stratego.



The game currently uses a 5x5 board. Players place weighted tiles with colored ticks, match edge colors for bonuses, and capture neighboring tiles based on side values. The winner is determined by the number of tiles controlled.



This is an existing university team project, not a new project. Preserve its current implementation and build upon it.



\## 2. Development Environment



\- Game engine: Unity 6.3

\- Language: C#

\- Operating system: Windows 11

\- IDEs: Visual Studio, VS Code, Antigravity

\- Version control: Git and GitHub Desktop

\- Project location: CST-410/TacTile



Never change the game engine or Unity version without explicit approval.



\## 3. Development Priorities



\### Phase 1: University Project



The immediate priority is finishing the existing class project.



\- Preserve existing gameplay and functionality.

\- Prioritize stability and completion of required features.

\- Avoid unnecessary architectural changes.

\- Keep modifications small and well documented.

\- Do not jeopardize the class deliverable for speculative improvements.



\### Phase 2: Standalone Mobile Game



After the university project is completed, the long-term goal is to expand TacTile into a polished mobile game.



Potential improvements include:

\- Mobile UI and responsive layouts.

\- Touchscreen controls.

\- Additional game modes.

\- Computer-controlled opponents.

\- Improved animations and sound.

\- Android and iOS builds.



These goals must not interfere with Phase 1.



\## 4. Existing Architecture



The following systems have been identified from previous development discussions. Their actual implementations must be inspected before making architectural assumptions.



\- GameDriver: Main gameplay coordination.

\- GameState: Tracks board spaces, tiles, selections, completed turns, and game-over state.

\- TurnManager: Manages turn progression.

\- TurnPhase: Represents gameplay phases.

\- LegalMoveGenerator: Determines valid moves.

\- CombatManager: Handles capturing and combat interactions.

\- TileView: Handles tile ownership, placement, appearance, and selection.

\- BoardSpace: Represents individual board positions.

\- TileGenerator: Generates tile strengths and colors.

\- TileSpawner: Creates player tile collections.

\- GameBalanceConfig: ScriptableObject for configurable game balance values.



Do not assume this list is complete or that these descriptions accurately reflect the current code.



Inspect the actual project before modifying or extending these systems.



\## 5. Mandatory Development Rules



1\. Inspect relevant scripts before making modifications.

2\. Understand existing dependencies and execution flow.

3\. Explain proposed implementations before significant changes.

4\. Prefer small, additive changes over rewrites.

5\. Never delete existing functions or functionality without explicit approval.

6\. Never restructure the project without approval.

7\. Never change Unity versions or game engines.

8\. Never modify project settings without approval.

9\. Never introduce third-party dependencies without approval.

10\. Preserve existing naming conventions and architecture where practical.

11\. Explain every modified file and the reason for its modification.

12\. Provide instructions for testing changes inside Unity.

13\. Identify potential regressions and dependencies.

14\. Never claim a feature works simply because code compiles.

15\. Never make assumptions about prefab assignments, Inspector references, scene objects, or ScriptableObject configuration.



\## 6. Approval and Safety Requirements



The developer must remain in control of project modifications.



\- Do not enable YOLO mode or unrestricted execution.

\- Do not automatically approve destructive operations.

\- Do not overwrite or delete files without explicit approval.

\- Do not modify scenes, prefabs, assets, project settings, or configuration files without first explaining the proposed changes.

\- Do not perform broad refactoring without approval.

\- Ask before installing packages, plugins, or dependencies.

\- Present a proposed implementation and affected files before significant changes.

\- Wait for approval before proceeding with significant modifications.



Read-only inspection and analysis are permitted when requested.



\## 7. Required Feature Development Workflow



For every feature:



1\. Identify relevant scripts and dependencies.

2\. Explain the existing implementation.

3\. Describe the proposed solution.

4\. List files that need modification.

5\. Obtain approval before implementing significant changes.

6\. Make incremental modifications.

7\. Explain exactly what changed.

8\. Provide Unity testing instructions.

9\. Identify possible regressions.

10\. Confirm what remains unverified.



Avoid unrelated changes while implementing a specific feature.



\## 8. Version Control



\- Respect existing Git branches and protected branches.

\- Never switch branches without approval.

\- Never commit, push, merge, reset, or discard changes without explicit approval.

\- Check the current working tree before making modifications.

\- Preserve existing uncommitted work.

\- Recommend a checkpoint commit before substantial changes, but let the developer perform or approve it.



\## 9. Budget Constraints



The development budget is $0.



\- Prefer free and locally available tools.

\- Do not recommend paid subscriptions as requirements.

\- Do not introduce paid APIs or services.

\- Avoid cloud services that could generate unexpected charges.

\- Ask before using any service that requires payment or an API key.



\## 10. Initial Project Inspection



Before proposing architectural changes, inspect the actual project.



The initial inspection must be read-only.



Determine:



\- Project structure and important directories.

\- Main gameplay systems and their responsibilities.

\- Game initialization sequence.

\- Board and tile generation.

\- Tile selection and placement flow.

\- Turn progression and player ownership.

\- Combat and capture mechanics.

\- Scoring and victory conditions.

\- Scene, prefab, and ScriptableObject dependencies.

\- Potential architectural risks.

\- Considerations for future mobile development.



Do not modify any project files during this inspection.



\## 11. Documentation



After inspection, help maintain project documentation covering:



\- Architecture and system responsibilities.

\- Gameplay execution flow.

\- Important script dependencies.

\- Scene and prefab relationships.

\- Configuration and balance settings.

\- Known bugs and limitations.

\- Development roadmap.

\- Significant implementation decisions.



Documentation must reflect the actual implementation rather than assumptions.



\## 12. Communication Style



\- Be direct and practical.

\- Explain technical decisions clearly.

\- Avoid unnecessary theoretical explanations.

\- Identify uncertainty instead of guessing.

\- Provide exact file paths and code locations.

\- Prefer minimal, understandable changes.

\- Explain how to verify results in Unity.



The primary objective is to safely finish the existing TacTile university project while preparing it for future AI-assisted development as a standalone mobile game.

