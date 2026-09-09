# EmergeLens: A Virtual Reality (VR) Game

This repository contains a Unity-based mobile VR box game developed as a capstone project for disaster awareness and preparedness. The simulation utilizes a Google Cardboard-style VR headset paired with a Bluetooth controller to immerse players in high-stress emergency scenarios, building crucial survival memory while keeping them physically safe.

## How to Import Unity Project from Github to Unity Hub

### Step 1: Download the GitHub Repository
Open your terminal or Git Bash and run:
```
git clone https://github.com/username/repository.git
```

### Step 2: Identify the Root Project Directory
Locate the exact folder containing the Unity project files before opening Unity Hub:
- Navigate inside your cloned or unzipped folder.
- Ensure you find the directory that directly contains the Assets, Packages, and ProjectSettings folders.

### Step 3: Add the Project into Unity Hub
1. Open Unity Hub.
2. Click on the Projects tab on the left sidebar.
3. Click the Add button in the top-right corner (or click the arrow next to it and choose Add project from disk).
4. Browse to and select the project folder identified in Step 2 (the one containing Assets).
5. Click Add Project (or Select Folder).

### Step 4: Resolve the Editor Version (Optional)
- Match Version: Look at the Editor Version column in Unity Hub. If the required version is installed, click the project to open it.
- Install Missing Version: If a warning icon appears stating the version is missing, click the dropdown under Editor Version to select an installed version, or click the warning prompt to let Unity Hub automatically download and install the exact matching Unity Editor version.
- First-Time Import: The first launch will take several minutes as Unity automatically rebuilds the Library folder and re-indexes all assets and packages.
