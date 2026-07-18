# 🕺 DanceMaster — AR App (Unity)

**DanceMaster AR** is an augmented reality application that lets users learn dance choreographies in an immersive, spatial way. The app projects footprint guides directly onto the floor via AR, allowing dancers to follow along step by step in their own environment.

Built with **Unity 6**, the app supports both **iOS (ARKit)** and **Android (ARCore)** and features a clean, modern UI powered by the **Unity UI Toolkit**.

> 🌐 Live: [DanceMaster.at](https://DanceMaster.at) · 📦 Latest Release: **v1.4.0** (stable)

---

## 📱 Features

### AR-Based Dance Training
- Footprints are projected onto the real-world floor via AR tracking
- A step controller lets users progress through a dance at their own pace
- Integrated music & metronome plugin keeps timing consistent

### Two Dance Modes

| Mode | Description |
|---|---|
| **Online Dances** | Curated choreographies created by the DanceMaster team |
| **My Dances** | Users can create, edit, and save their own steps and sequences |

### Modern UI (Unity UI Toolkit)
- Structured panels and smooth transitions
- Consistent UX via StyleSheets and UI Toolkit components
- Lightweight and responsive across screen sizes

---

## 🛠️ Tech Stack

| Component | Technology |
|---|---|
| Engine | Unity 6 |
| AR Framework | AR Foundation |
| iOS Support | ARKit XR Plugin |
| Android Support | ARCore XR Plugin |
| UI System | Unity UI Toolkit |
| Shaders | ShaderLab / HLSL |
| Logic | C# |
| Backend | [DanceMaster Symfony API](https://github.com/WeidenauerErik/DanceMaster-AR-Symfony) |

---

## 📦 Project Structure

```
DanceMaster-AR-Unity/
├── Assets/                # Scenes, Scripts, UI, Shaders, Prefabs
├── Packages/              # Unity Package Manager dependencies
├── ProjectSettings/       # Unity project configuration
├── UIElementsSchema/      # UI Toolkit schema definitions
└── .utmp/                 # Unity tooling
```

---

## 🚀 Getting Started

### Prerequisites
- Unity 6 (or later)
- AR Foundation package
- ARKit XR Plugin (for iOS builds)
- ARCore XR Plugin (for Android builds)
- A compatible physical device with AR support

### Setup

```bash
git clone https://github.com/WeidenauerErik/DanceMaster-AR-Unity.git
```

1. Open the project in **Unity Hub**
2. Import AR Foundation, ARKit, and ARCore packages via the **Package Manager**
3. Configure your backend API URL in the project settings
4. Build and deploy to your target platform (iOS / Android)

### Permissions Required
- **Camera** — for AR world tracking
- **Motion Tracking** — for ARKit/ARCore spatial anchors

---

## 📦 Releases

| Version | Status |
|---|---|
| v1.4.0 | ✅ Latest (stable) |
| v1.3.0 | Previous |

---

## 🔗 Related Repositories

| Repository | Description |
|---|---|
| [DanceMaster-AR-Symfony](https://github.com/WeidenauerErik/DanceMaster-AR-Symfony) | Backend REST API (Symfony/PHP) |
| [DanceMaster-WEB-Vue](https://github.com/WeidenauerErik/DanceMaster-WEB-Vue) | Web frontend (Vue 3) |

---

## ✨ Author

**Erik Weidenauer**
