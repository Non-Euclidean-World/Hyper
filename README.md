# Hyper
## About
Procedural terrain generation in non-Euclidean spaces.

### Used Technologies
[Rider](https://www.jetbrains.com/rider/) \
[Visual Studio](https://visualstudio.microsoft.com/) \
[Blender](https://www.blender.org/) \
[InkScape](https://inkscape.org/) \
[Gimp](https://www.gimp.org/)

### Used Libraries
[OpenTK](https://opentk.net/) \
[BEPU physics 2](https://www.bepuentertainment.com/) \
[Assimp Net](https://github.com/assimp/assimp-net) \
[Skia Sharp](https://github.com/mono/SkiaSharp)

### References
Szirmay-Kalos, L., Magdics, M. Adapting Game Engines to Curved Spaces. Vis Comput 38, 4383-4395 (2022). https://doi.org/10.1007/s00371-021-02303-2 \
https://polycoding.net/marching-cubes/part-1/ \
https://www.youtube.com/watch?v=f3Cr8Yx3GGA \
https://www.youtube.com/watch?v=DiIoWrOlIRw

### Assets Used
<a href="https://www.flaticon.com/free-icons/idea" title="idea icons">Idea icons created by Good Ware - Flaticon</a> \
<a href="https://www.flaticon.com/free-icons/pistol" title="pistol icons">Pistol icons created by Vector Stall - Flaticon</a> \
<a href="https://www.flaticon.com/free-icons/bullet" title="bullet icons">Bullet icons created by Smashicons - Flaticon</a>
<a href="https://www.flaticon.com/free-icons/letter-h" title="letter h icons">Letter h icons created by Alphabets Number - Flaticon</a>

## Uninstalling _Hyper_

### Removing game saves
The game saves are stored in the `C:\Users\<your user name>\AppData\Roaming\Hyper` directory.

This location can be accessed by typing
```
PS> cd "$env:APPDATA\Hyper"
```
in Powershell.

### Removing the game installation (doesn't apply if the program was built from source)
Simply remove the installation folder, i.e. `Hyper-<version>`.
