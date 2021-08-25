# ShaderGraph Key2Node
Simple shortcut system into unity shader graph without modifying package by hacking style.

Reference - [Unity Forum Thread](https://forum.unity.com/threads/keyboard-shortcuts-are-an-essential-and-missing-feature.852154/) by DigitalSalmon.

### Installation
#### way 1. unity-package
this repo can be imported as open unity package.  
add https://github.com/seonghwan-dev/shadergraph-key2node.git in package manager.
#### way 2. npmjs
in `Packages/manifest.json` ...  

```json
{
  "dependencies": {
    "kr.seonghwan.shadergraph-key2node": "1.0.0"
  }
}
```

```json
{
  "scopedRegistries": [
    {
      "name": "NPM",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "kr.seonghwan"
      ]
    }
  ]
}
```



### Uninstallation
use menuitem `Tools/ShaderGraph Key2Node/Clean Uninstall`

### Trouble shooting
run menuitem `Tools/ShaderGraph Key2Node/Force Resolve`

### Why should I use in this way?
You don't have to. this is an expedient to avoid modifying the Shader Graph packge in every project. We can mofidy the original package caches in appdata, but it's local way so its not fit. I tried to implement this feature without editing original package by hooking several functions in loaded assembly on unity editor but it requires lots of things to consider. 

### How this works?
1. script installs static delegates at the source file in `ProjectRoot`/Library/PackageCache/com.unity.shadergraph@x.x.x/editor/drawing/views/grapheditorview.cs
2. add callbacks to installed delegate when domain reloaded. (`[InitializedOnLoad]`)
3. when constructor of grapheditorview is called, installed delegates invoked.

### Warning
- takes more time after compilation because of InitializeOnLoad
- takes more time in starting unity editor because of installing delegates and callbacks every unity editor instance.
