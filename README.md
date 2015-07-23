# skaa_editor
SkaaEditor seeks to provide non-technical users (e.g., non-programmers) the ability to re-skin the game to their liking. The project is currently in its infancy and battles three other projects that vie for my time.

![screenshot](https://github.com/sraboy/skaa_editor/blob/native_sav/_other/skaa_editor.png)

## status
Currently, you can load most (all?) unit-type sprites (ballista, catapult, etc). You have to first load a pallet (Edit>>Load Pallet, choose pal_std.res or pal_win.res) and then load an SPR file via the button on the right-hand side. 

You can export to 32-bit BMP but saving in the native SPR format is still buggy.

<<<<<<< HEAD
---
=======
## getting started
I am not yet providing binaries. You'll have to compile the code with Visual Studio (untested on Mono), targeting .NET4.5.

For now, the only thing that is loaded in the edit area is the first frame. That's not a back-end limit and is easy to change in the code if you like; I just haven't decided on the UX for that yet.

## the future
Eventually, I plan to merge some of my other work on 7KAA's data files into this, as a monolithic trainer/game-editor. I'd also like to port the map generation code and add the ability to create custom maps and custom scenarios.

## how to help
Fork the code and take a whack. Most of the code is self-explanatory and quite a bit was actually copy/pasted from the original C++ and then edited for C#; obviously, we're using managed code and .NET-style objects. You may still see a few C++isms in the code though. Feel free to clean those up and use "real .NET". I'm not inclined to change too many things that work as-is.

## credits
Obviously, this project wouldn't be possible without Enlight Software's release of the game's source code -- not to mention Trevor Chan's brilliant ideas for this game to begin with.

Shoutouts to http://http://7kfans.com/ and Cyotek's awesome [ImageBox control](https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox). The latter saved me a lot of pain early on.

## license
>>>>>>> c3548cd... updated README
Everything of mine is GPLv3; content from 7KAA remains under that license, [http://www.7kfans.com/wiki/index.php/Seven_Kingdoms:Copyrights](mostly all GPL as well).
