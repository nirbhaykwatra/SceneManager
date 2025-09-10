# Scene Manager
[![Unity 6000.0+](https://img.shields.io/badge/unity-6000.0%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE.md)

A convenient tool for managing scene assets in a Unity project.

### System Requirements
Unity 6000.0+. Will likely work on earlier versions but this is the version I tested with.

Requires Odin Inspector (UI Toolkit version coming in the future).

### Installation
Use the Package Manager and use Add package from git URL, using the following: 
```
https://github.com/nirbhaykwatra/SceneManager.git
```

## Usage

### Opening Scene Manager

- Go to `File -> Scene Manager` or press `CTRL+SHIFT+N` (you can remap the shortcut as you wish)

## Basic Functionality

![swm](https://github.com/nirbhaykwatra/blog.nirbhaykwatra.com/blob/main/static/storage/SceneManagerREADME/smw.png?raw=true)

### 1. Main Toolbar

- **Clean Project Metadata** - Remove all unused scene metadata files from the project. This function is also executed every time the Scene Manager window is opened.  
Use this after moving scene files to a different location.

- **Delete Selected Scene** - Delete the scene selected on the sidebar menu.

- **Open Selected Scene** - Open the scene selected on the sidebar menu.

- **Create New Scene** - Opens the Create New Scene window.

### 2. Sidebar Menu

- This menu lists all the scenes found in the assets directory. Scenes are grouped by scene type by default but they can also be listed under their asset paths.

### 3. Inspector

- **Type** - The scene type.

- **Path** - The scene asset path.

- **Edit Scene Metadata** - Can be used to enable editing on some metadata fields. Currently only supports changing scene type.

- **Refresh Scene Metadata** - Used to synchronise the inspector with the scene's serialized metadata files. Ideally this shouldn't need to be used.

### 4. Utility Toolbar

- **Refresh** - Refresh the sidebar menu. May need to be used sometimes when editing scene metadata or deleting scenes.

- **Show In Project** - Pings the selected scene in the project window. Useful for quickly locating scene asset files.

- **Show By Path** - Toggles between type-based and path-based menu sorting.

## Create New Scene

![alt text](https://github.com/nirbhaykwatra/blog.nirbhaykwatra.com/blob/main/static/storage/SceneManagerREADME/demo.gif?raw=true)

Creating new scenes is very simple:
- Click "Create New Scene"
- Specify the name, path, type and an optional scene template for your scene
- Hit "Create Scene".

### Custom Scene Types

Scene Types are designed to help organize your scenes. They do not affect the scene itself in any way, they are simply an extra piece of metadata.

As seen in the previous step, scenes can be assigned types using a dropdown at creation or in the Scene Manager window at a later point. 

In addition, custom scene types can also be created and destroyed at any time, helping to organize scenes to your preference.

![alt text](https://github.com/nirbhaykwatra/blog.nirbhaykwatra.com/blob/main/static/storage/SceneManagerREADME/demo2.gif?raw=true)