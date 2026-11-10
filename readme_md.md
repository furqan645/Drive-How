# Vehicle Dynamics and Constraint Validation Simulation

A modular C# vehicle simulation and dynamic constraint validation system developed in Unity, demonstrating real-time vector transformations, kinematic damping, decoupled state management, and spatial chunk activation.

### Applied Engineering and Mathematics
* Kinematics and Metric Telemetry: Implemented instantaneous Euclidean norm vector magnitude calculations to convert Rigidbody linear velocity into real-time metric speed ($km/h$) for dynamic constraint validation.
* Damped Interpolation and Orientation: Decoupled camera tracking using `Vector3.SmoothDamp` critically damped harmonic chasing and 4D `Quaternion.Slerp` spherical linear interpolation to eliminate visual frame jitter.
* Signal Processing and Audio Modulation: Applied first-order linear interpolation (`Mathf.Lerp`) modulated against frame delta time to dynamically simulate engine pitch transitions across acceleration, idle, and reverse states.
* Rigid-Body Coordinate Synchronization: Deconstructed physics-tick simulation states using `WheelCollider.GetWorldPose` to synchronize spatial positions and rotations directly with visual wheel meshes.

### Architecture and Systems Design
* Centralized Lifecycle Singletons: Structured `LevelManager` and `UIManager` with duplicate-destruction guards to ensure clean session lifecycles.
* Memory-Safe Event Handling: Managed scene transition delegates via `SceneManager.sceneLoaded` with explicit single-run unsubscription to prevent event-handler memory leaks.
* Asynchronous Task Management: Protected UI messaging routines against race conditions by enforcing single-flight execution and terminating overlapping coroutines.
* Spatial Geometry Chunking: Partitioned procedural roadway assets into deterministic array-indexed chunks to minimize draw-call overhead and avoid runtime instantiation spikes.
* State Persistence: Maintained player metrics, progress thresholds, and level states across sessions using structured key-value serialization via `PlayerPrefs`.

### Project Architecture
* Controllers: `CarController.cs` (physics, kinematics, audio DSP, constraint checks), `CameraFollow.cs` (SmoothDamp tracking, Quaternion Slerp)
* Management: `LevelManager.cs` (spatial array chunking, scale contraction), `UIManager.cs` (single-flight coroutines, UI updates), `BtnControlls.cs` (lifecycle navigation, scene loading)
* Telemetry Display: `GameOver.cs` (metric hydration and score presentation)

### Setup Instructions
1. Open the project root in Unity 2022.3 LTS or newer.
2. Ensure scenes are registered in build order: `Starter.unity`, `GamePlay.unity`, `Ender.unity`.
3. Load `Assets/Scenes/Starter.unity` and run the simulation.