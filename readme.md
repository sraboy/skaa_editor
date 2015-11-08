# skaa_editor
SkaaEditor seeks to provide non-technical users (e.g., non-programmers) the ability to re-skin the game to their liking. The project is currently in its infancy and battles three other projects that vie for my time.

![screenshot](https://github.com/sraboy/skaa_editor/blob/master/_other/skaa_editor.png)

## status & known issues
You can load most (all?) unit-type sprites (ballista, catapult, etc). Submit an issue via the GitHub tracker if you encounter an SPR file that does not open properly. You can currently save either the entire sprite (all frames) or just the current frame, both in the 7KAA SPR file format. Same goes for exporting to a 32-bit BMP. Note that yellow is the default color; 7KAA automatically replaces this yellow with the proper nation color while in-game.

- The sprites will not actually work in 7KAA unless you manually edit the frame's offset in std.set. This is in-progress.
- SkaaEditor must be restarted for each sprite you wish to open/edit/save. 
- There is currently no undo/redo nor copy/paste. Ensure you're making backups of your game data.
- You can select a color on the palette without being in Edit Mode.

## getting started
You may download the current ALPHA release [here](https://github.com/sraboy/skaa_editor/blob/master/other/SkaaEditor_x86_2015102138.zip). Otherwise, compile the code with Visual Studio 2015 (untested on Mono), targeting .NET4.5.

The ZIP file includes debug information to help you help me fix bugs.

## how to

The "Edit Mode" checkbox enables editing. By default, left-click is "erase" and replaces the underlying pixel with a transparent color. Right-clicking changes it to black. Select colors (only changing the left-click is currently supported) but choosing them from the palette on the left.

Use the tracking bar on the top-right to navigate frames. This is how you change the "active frame," the one which you're editing. Double-click on the mini-viewer to animate by cycing through all the frames in the sprite; double-click again to stop. Middle-clicking zooms in.

## the future
Eventually, I plan to merge some of my other work on 7KAA's data files into this, as a monolithic trainer/game-editor. I'd also like to port the map generation code and add the ability to create custom maps and custom scenarios.

## how to help
Fork the code and take a whack. Most of the code is self-explanatory and quite a bit was actually copy/pasted from the original C++ and then edited for C#; obviously, we're using managed code and .NET-style objects. You may still see a few C++isms in the code though. Feel free to clean those up and use "real .NET". I'm not inclined to change too many things that work as-is.

On my lazier coding days, I go through and properly document/comment the code.

## credits
Obviously, this project wouldn't be possible without [Enlight Software](http://www.enlight.com/)'s release of the game's source code -- not to mention Trevor Chan's brilliant ideas for this game to begin with. My personal fork, with Visual Studio project/solution files, can be found [here](https://github.com/sraboy/7kaa). 

Thanks to everyone at [7kfans.com/](http://www.7kfans.com/), the new home of Seven Kingdoms and its open source project, for keeping 7KAA awesome and up-to-date. I'd also like to give a shoutout to [Cyotek](http://www.cyotek.com/)'s awesome [ImageBox control](https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox) which saved me quite a bit of pain and could very well have prevented me from just dumping the project from day one.

## license
Everything of mine is GPLv3; content from 7KAA remains under that project's license, [http://www.7kfans.com/wiki/index.php/Seven_Kingdoms:Copyrights](mostly all GPL as well).