# skaa_editor
SkaaEditor seeks to provide non-technical users (e.g., non-programmers) the ability to re-skin the game to their liking. The project is currently in its infancy and battles three other projects that vie for my time.

![screenshot](https://github.com/sraboy/skaa_editor/blob/master/_other/skaa_editor.png)

## status & known issues
You can load most (all?) unit-type sprites (ballista, catapult, etc). Submit an issue via the GitHub tracker if you encounter an SPR file that does not open properly. You can currently save the sprite, with any edits, in the 7KAA SPR file format. You can also exporting to a 32-bit BMP. 

Note that yellow is the default color; 7KAA automatically replaces this yellow with the proper nation color while in-game.

- You must save the Game Set after saving the sprite.
- The sprite's filename must match the original in order for you to make any changes to the game set later. This is due to the SET file's use of the filename as the sprite's ID in the database.
- There is currently no undo/redo nor copy/paste. Ensure you're making backups of your game data.
- Editing is laggy. You'll have to move the mouse slower than you're used to.

## getting started
You may download the current alphaV2 release [here](https://github.com/sraboy/skaa_editor/blob/master/other/SkaaEditor_x86_alphav2.zip). Otherwise, compile the code with Visual Studio 2015 (untested on Mono), targeting .NET4.5.

There is debug information included to give you more helpful error messages. Please report bugs here by submitting an issue or by emailing me at steven.lavoiejr@gmail.com.

## how to

The "Edit Mode" checkbox enables editing. Choose a color from the palette. Right-click will erase pixels by making them transparent. Select colors (only changing the left-click is currently supported) but choosing them from the palette on the left.

Use the tracking bar on the top-right to navigate frames. This is how you change the "active frame," the one you're editing. Double-click on the mini-viewer to animate by cycing through all the frames in the sprite; double-click again to stop. Middle-clicking zooms in.

## the future
Eventually, I plan to merge some of my other work on 7KAA's data files into this, as a monolithic trainer/game-editor. I'd also like to port the map generation code and add the ability to create custom maps and custom scenarios.

## how to help
Fork the code and take a whack. Most of the code is commented and/or self-explanatory. You may see a few C++isms where I was following the original game code closely. Feel free to clean up and use "real .NET". I'm not inclined to change too many things that work as-is.

## credits
Obviously, this project wouldn't be possible without [Enlight Software](http://www.enlight.com/)'s release of the game's source code -- not to mention Trevor Chan's brilliant ideas for this game to begin with. My personal fork, with Visual Studio project/solution files, can be found [here](https://github.com/sraboy/7kaa). 

Thanks to everyone at [7kfans.com/](http://www.7kfans.com/), the new home of Seven Kingdoms and its open source project, for keeping 7KAA awesome and up-to-date. I'd also like to give a shoutout to [Cyotek](http://www.cyotek.com/)'s awesome [ImageBox control](https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox) which saved me quite a bit of pain and could very well have prevented me from just dumping the project from day one.

## license
Everything of mine is GPLv3; content from 7KAA remains under that project's license, [http://www.7kfans.com/wiki/index.php/Seven_Kingdoms:Copyrights](mostly all GPL as well). See the source for any individual notes on licensing as some content/code falls under other (open source) licenses.