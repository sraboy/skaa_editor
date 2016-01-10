In keeping with the 7KAA project's goals of not changing working code, the game's file formats must remain constant. This requires several workarounds which may make SkaaEditor seem a bit unintuitive.

A basic understanding of a couple of the file formats and how they interact is required to successfully edit the resources in any meaningful way.

7KAA keeps a lot of essential information in the Standard Set file (std.set). This file contains a header listing each of its records and their respective offsets in the file. Its records are, in fact, dBaseIII DBF files just stitched together end-to-end. The header listing is in the "ResIdx" format, which consists of nine characters for the record's name and four bytes for its corresponding offset.

However, RES files are not in any single format. Many RES files have their own internal tracking of the information that std.set tracks for others. These files are not marked in any obvious way nor do they have a reliable file header (just a 16-bit value indicating how many records are in the file). Many of the files in this format have bitmap data in their records. Within SkaaEditor, these are known as ResIdxMultiBmp files. A few RES files are simply DBF files while others are color palettes, audio or other data.

When a file is open, its type will be displayed in the status bar on the bottom right of the window. Use this to guide you.

When editing Sprites, or SPR files, each frame's offset is maintained in the SFRAME database in std.set. Due to the encoding of the frames' bitmaps, any change that changes a transparent pixel (either to another color or changing its position) will affect the offset of every frame in the file following that one. It also changes the file's size, which is also tracked in SFRAME. Therefore, to make changes to SPR files, you must also open the game set.

Remember, changes are cumulative. This means that, if you have an edited Sprite and have updated it in the std.set file, you must open that same std.set file to make further changes to the same Sprite or others.

--- Current File Support ---
If a file is not a ResIdx or DBF file, you'll likely need to open the std.set file along with it. Currently, only the SFRAME database is supported, meaning you can only edit Sprites in SPR files when it comes to std.set.

ResIdx files can be edited on their own, without std.set.

ICN files are just like single-frame sprites: essentially bitmap data. However, they use their own palette. You must load the COL file in the same directory with the corresponding filename.



