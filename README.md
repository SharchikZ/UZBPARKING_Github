# UZB Parking - 3D Car Parking & Driving Simulator

A mobile car parking and driving simulation game set in a realistic environment with authentic local details, built with Unity.

---

## 🚀 Key Features

* **Realistic Vehicle Physics**: Powered by `RealisticCarControllerV3` for smooth driving, drifting, and precise parking mechanics.
* **Mobile-Optimized Controls**: Touchscreen integration via `Joystick Pack` for seamless touch navigation.
* **Customization Options**: Multiple wheel rim selections (`Rims`) and regional license plate frames (`numframe`).
* **Detailed Environments**: Custom road networks created with `RoadArchitect` featuring regional architecture (`russian_buildings`).
* **Multi-language Support**: Multi-language UI localization (`locales`).

---

## 📁 Repository Structure

```text
.
├── Audios/                 # Sound effects and background audio
├── DemoPrefabs/            # Pre-configured demo prefabs
├── Fonts/                  # UI typography and font files
├── Graphics/               # Textures, sprites, and UI elements
├── Joystick Pack/          # On-screen mobile input assets
├── Materials/              # Custom materials and shaders
├── Models/                 # Main 3D vehicle and environment models
├── OldModels/              # Legacy 3D assets archive
├── OldProject/             # Archived project files
├── PrefabPushing/          # Physics/interactive prefabs
├── RealisticCarControllerV3/ # RCC Vehicle Physics System
├── Resources/              # Dynamic asset loading folder
├── Rims/                   # Car rim 3D models and textures
├── RoadArchitect/          # Procedural road and infrastructure system
├── locales/                # Multi-language localization files
├── map/                    # Map layouts and scene assets
├── numframe/               # License plate frames and styling
└── russian_buildings/      # Building models and environment props
```

---

## 🛠️ Setup & Installation

1. **Unity Engine**: Ensure you have Unity installed (2021.3 LTS / 2022.3 LTS or newer recommended).
2. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-username/uzb-parking.git
   ```
3. **Open in Unity Hub**:
   * Add the project folder to **Unity Hub**.
   * Open the main scene located in the `map/` directory.
4. **Build**: Target platform can be switched to **Android** or **iOS** under `File > Build Settings`.

---

## 📄 License

This project is open-source or proprietary to its creators. Check individual asset license terms for third-party packages used (`RealisticCarControllerV3`, `Joystick Pack`, `RoadArchitect`).
