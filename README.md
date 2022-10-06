# RealityMerge
Extra Muros 2022 – Venedig

## About
This AR App has two main components/scenes. The first scene allows you to place a [concept building](https://mimo-hsd.de/project/concept/) in real scale and small scale in AR. The second scene demonstrates the energyflow of household devices, also within AR.

## Getting Started
This App is build and tested for Windows platforms.

### Setup development environment
The project is developed with Unity, using the [SimpleFileBrowser](https://github.com/yasirkula/UnitySimpleFileBrowser) Asset by [yasirkula](https://github.com/yasirkula). Based on the preferred platform the appropriate Build Support is needed. The Build Support can be found through Unity Hub under the Installs panel ("*Installs > Unity Version > Add Modules*").

## How to build the project
### Windows
1. Open "Build Settings"
2. Under "Scene In Build" the following scene should be added: "MainScene"
3. Switch the build target to Windows
4. Press "Build and Run"

## Usage
First, the images to be used must be imported - by clicking on the "Import Image Sequence" button. The images must be in the same directory, multiple images can be selected using the Ctrl key. Now the position of the randomly generated seam between the images can be specified. All functions are explained in this table:

| **Function name** | **Effect** |
| ------ | ------ |
| Warp Image  | Recalculate the randomly generated seam |
| Row Size | Number of pixels that a row has in height |
| Seam Size | Maximum width of the seam relative to the image size, value range from 0.01 - 1.0 | 
| Horizontal  | Checkbox with which it is possible to switch between a horizontal or a vertical seam |
| Flip | Checkbox with which the order of the used images can be reversed |
| Offset | Slider with which the position of the seam can be moved |
| Save Image | Saves the current image in the same directory where the original images were located |

Contributors: [Lukas Scheurer](lukas.scheurer@study.hs-duesseldorf.de)

