# Lethal Company Ship Turrets

This is the source-code for the Ship Turrets mod for Lethal Company the game. This mode works using Harmony for adding patches/plugins to existing Lethal Company .Net assembly code.

# File Structure

├── NuGet.Config
├── ShipTurrets.csproj
├── Plugin.cs
└── Patches
    ├── RoundManagerPatch.cs
    └── TurretPatch.cs

# Bugs

* "CalculatePolygonPath" can only be called on an active agent that has been placed on a NavMesh.

# Future Roadmap

* Buy turrets in the ship terminal.
* More ship Upgrades.
* Balancing with features such as:
    1. Turret Ammo
    2. Turret cooldown periods.