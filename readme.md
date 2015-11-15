# skaa_editor
SkaaEditor seeks to provide non-technical users (e.g., non-programmers) the ability to re-skin the game to their liking. The project is currently in its infancy and battles three other projects that vie for my time.

![screenshot](other/screenshot.png)

## getting started
You may download the current alphaV3 release [here](https://github.com/sraboy/skaa_editor/blob/master/other/SkaaEditor_x86_alphav3.zip). The release includes debug information (PDB files) included to help you help us with more helpful error messages if the application throws an exception. Please report bugs here by submitting an issue or by emailing me at steven.lavoiejr@gmail.com.

You can load most (all?) unit-type sprites (SPR files: ballista.spr, caravan.spr, etc). Please submit an issue here on Github if you encounter an SPR file that does not open properly. 

You can currently save the sprite, with any edits, in the 7KAA SPR file format. You can also export to a 32-bit BMP via the Edit menu. You cannot simply save an edited sprite without saving a new game set file, std.set. This contains offsets for the bitmap data in the SPR file.

SPR files are sprites. The SET file, std.set, is a database containing information about the individual sprites, among many other things.

## status & known issues
- Note that various shades of yellow are the default colors; 7KAA automatically replaces this yellow with the proper nation color while in-game.
- Use the built-in "project" functionality for best results. Otherwise, know that the game set's data is loaded based on the current sprite's SpriteId, which is based on the SPR's filename. Therefore, if you have "myballista.spr" open for editing and then load std.set, your saves
- There is currently no undo/redo nor copy/paste.
- Editing with the pencil tool is a bit laggy.

## how to

Launch SkaaEditor. You can create a new project by clicking the "New Project" button on the toolbar. 

Choose a color from the palette. Right-click will erase pixels by making them transparent. Select colors (only changing the left-click is currently supported) but choosing them from the palette on the left.

Use the tracking bar on the "timeline" in the top-right to navigate frames. This is how you change the "active frame," the one you're editing. Double-click on the timeline's image to cause it to animate by cycing through all the frames in the sprite; double-click again to stop. Middle-clicking the timeline's image zooms in/out. A single left/right click on the image will navigate forward/backward exactly one frame.

## the future
Eventually, more of the sprite's data will be exposed to the user to allow for editing specific actions, as well as adding new frames for new actions. 

I would like this project to be a suite of tools for developers/enthusiasts to mod and customize the game.

## how to help
Fork the code and take a whack. You can compile the code with Visual Studio 2015 (untested on Mono). Each project within the solution has its own .NET framework target since some are third-party libraries. Changing these targets may introduce undefined behavior and/or new bugs.

Most of the code is commented and/or self-explanatory. You'll likely see a few C++isms where I was following the original game code closely, especially in the SkaaGameDataLib project.

Pull requests should come from a clean branch tracking master. Minimize the number/extent of changes per pull request. Ensure you are really following the source code, and testing your changes, before submitting pull requests. There is a "unit testing" library included in the project; it's really a catch-all project for testing so feel free to create new classes in there to test your objects. Creating tests for SkaaGameDataLib would be greatly appreciated but would require some significant work on your part to refactor the code... it's a mess.

## credits
Obviously, this project wouldn't be possible without [Enlight Software](http://www.enlight.com/)'s release of the game's source code -- not to mention Trevor Chan's brilliant ideas for this game to begin with. My personal fork of the game, with Visual Studio project/solution files, can be found [here](https://github.com/sraboy/7kaa). 

Thanks to everyone at [7kfans.com/](http://www.7kfans.com/), the new home of Seven Kingdoms and its open source project, for keeping 7KAA awesome and now up-to-date. I'd also like to give a shoutout to [Cyotek](http://www.cyotek.com/) for their awesome [ImageBox control](https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox) and [ColorPicker](https://github.com/cyotek/Cyotek.Windows.Forms.ColorPicker), both of which have been extended for this project's use.

## license
This project code is GPLv3, unless otherwise noted in the code or documentation. Game content from 7KAA remains under that project's license, [http://www.7kfans.com/wiki/index.php/Seven_Kingdoms:Copyrights](mostly all GPL as well). Again, see the source for specifics but other licenses include: 

- CC-by-NC-ND 2.5: Some icons/graphics from Paint.NET 
- FDL-1.2: The current paint bucket fill implementation
- MIT: Cyotek's controls
- VCKicks (proprietary license): BitmapProcessing library