# GameSetExporter

This was used as a unit test for exporting 7KAA's game set (STD.SET) to JSON from dBase III. The textbox on the main window is no longer used. To use:

1. Click the Load SET button and navigate to the STD.SET file.
2. Create a folder named "dbf" in this same directory.
3. Click the Save SET button and navigate to the dbf directory. Type any filename; it's not used.

All the dBase databases are dumped to individual DBF files and then they're all parsed by [Json.NET](http://www.newtonsoft.com/json), parsed for pretty-printing (indentation) and dumped to DBName.dbf.json.txt files in the dbf directory.

Note that the SkaaGameDataLib project in this repo may not be updated. See the [SkaaEditor](https://www.github.com/sraboy/skaa_editor) project.

This code is released under the [GPLv3](http://www.gnu.org/licenses/gpl-3.0.html).
