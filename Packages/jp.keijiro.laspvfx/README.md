LaspVfx
-------

![gif](https://i.imgur.com/KIwkpcI.gif)
![gif](https://i.imgur.com/Nrb1XGw.gif)

**LaspVfx** is a Unity package providing property binders for connecting [LASP]
(audio input plugin) and [Visual Effect Graph].

[LASP]: https://github.com/keijiro/Lasp
[Visual Effect Graph]: https://unity.com/visual-effect-graph

System Requirements
-------------------

- Unity 2019.4 or later

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Please add the following lines to the manifest file
(`Packages/manifest.json`).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

<details>
<summary>.NET Standard 2.0 (Unity 2021.1 or earlier)</summary>

To the `scopedRegistries` section:

```
{
  "name": "Unity NuGet",
  "url": "https://unitynuget-registry.azurewebsites.net",
  "scopes": [ "org.nuget" ]
},
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"org.nuget.system.memory": "4.5.3",
"jp.keijiro.laspvfx": "1.0.2"
```

After the changes, the manifest file should look like:

```
{
  "scopedRegistries": [
    {
      "name": "Unity NuGet",
      "url": "https://unitynuget-registry.azurewebsites.net",
      "scopes": [ "org.nuget" ]
    },
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "org.nuget.system.memory": "4.5.3",
    "jp.keijiro.laspvfx": "1.0.2",
    ...
```
</details>

<details>
<summary>.NET Standard 2.1 (Unity 2021.2 or later)</summary>

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.laspvfx": "1.0.2"
```

After the changes, the manifest file should look like:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.laspvfx": "1.0.2",
    ...
```
</details>
