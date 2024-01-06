# Lethal Company Ship Turrets

This is the source-code for the Ship Turrets mod for Lethal Company the game. This mode works using [Harmony](https://harmony.pardeike.net/articles/patching.html) for adding patches/plugins to existing Lethal Company .Net assembly code.

# Setup

Follow the guide at the [Lethal Company Wiki](https://lethal.wiki/) for instructions on setting this projetc up and building it.

# File Structure

```bash
├── NuGet.Config
├── ShipTurrets.csproj
├── Plugin.cs
└── Patches
    ├── RoundManagerPatch.cs
    └── TurretPatch.cs
```

# Bugs

* "CalculatePolygonPath" can only be called on an active agent that has been placed on a NavMesh.

# Future Roadmap

* Buy turrets in the ship terminal.
* More ship Upgrades.
* Balancing with features such as:
    1. Turret Ammo
    2. Turret cooldown periods.

# License

ShipTurrets is distributed under the MIT License.

# Acknowledgments

Shout out to [GoldenKitten](https://github.com/goldenkittenplays) for their work the [FairAI](https://thunderstore.io/c/lethal-company/p/TheFluff/FairAI/) mod and sharing their open-source code on their turret AI for attacking enemies in Lethal Company.