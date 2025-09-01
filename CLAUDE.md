# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2D Asteroids game project using Unity 6000.0.54f1. It implements classic Asteroids gameplay with a ship that can thrust, rotate, and shoot bullets at asteroids in a screen-wrapping environment.

## Development Environment

- **Unity Version**: 6000.0.54f1
- **Rendering Pipeline**: Universal Render Pipeline (URP)
- **Input System**: Unity's new Input System package (1.14.1)
- **2D Features**: Uses Unity's 2D feature package suite

## Core Architecture

### Game Systems

The project follows a component-based architecture with several key systems:

1. **Singleton Pattern**: Core managers inherit from `Singleton<T>` for global access
2. **Object Pooling**: `BestObjectPool<T>` manages bullet lifecycle efficiently
3. **Screen Wrapping**: `WarpManager` handles viewport boundary wrapping for all game objects
4. **Player Control**: `PlayerController` manages ship movement, rotation, and shooting

### Key Components

- **Ship** (`Assets/Scripts/Ship.cs:4`): Core ship component with physics and audio references
- **Asteroid** (`Assets/Scripts/Asteroid.cs:6`): Asteroid behavior with collision detection placeholders
- **Bullet** (`Assets/Scripts/Bullet.cs:5`): Projectile with timeout and pooling support
- **PlayerController** (`Assets/Scripts/PlayerController.cs:3`): Main player input and ship control
- **WarpManager** (`Assets/Scripts/WarpManager.cs:4`): Handles screen wrapping for all registered transforms

### Controls

- **W**: Thrust forward
- **A/D**: Rotate left/right
- **Space**: Fire bullets

### Object Management

The game uses Unity's built-in object pooling system (`UnityEngine.Pool`) wrapped in `BestObjectPool<T>` for efficient bullet management. All game objects that need screen wrapping register with the `WarpManager` singleton.

## Asset Structure

- **Scripts**: Core game logic in `Assets/Scripts/`
- **Sprites**: Kenney Simple Space asset pack in `Assets/Sprites/kenney_simple-space/`
- **Prefabs**: Game object prefabs in `Assets/Prefab/`

## Development Commands

This Unity project uses Unity Editor for development. Common operations:

- **Open Project**: Launch Unity Hub and open the project folder
- **Build**: File > Build Settings in Unity Editor
- **Play Testing**: Use Unity Editor Play Mode
- **Package Management**: Window > Package Manager in Unity Editor

## Project State

The project appears to be in active development with placeholder collision detection logic in the `Asteroid` class (`Assets/Scripts/Asteroid.cs:31-38`). The basic movement, shooting, and screen wrapping systems are implemented and functional.