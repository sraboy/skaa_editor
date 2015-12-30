#### Disclaimer: I am not a lawyer. I am simply providing my best understanding of these licenses but this is not legal advice. You should consult an attorney for any legal advice.

This project mixes and matches many licenses. Obviously, everything is "open source"; however, many items are under the GPLv3 while others are under the MIT license, or something else entirely. See each license for specific information but here's the simple take (see details for asterisks):

| Component			|	License			|
| ------------------|-------------------|
| Capslock			|	[MIT](https://opensource.org/licenses/MIT)								|
| Cyotek			|	[MIT](https://opensource.org/licenses/MIT)								|
| 7KAA (game)		|	[GPLv3](http://www.gnu.org/licenses/gpl-3.0.en.html)					|
| FastBitmap		|	[VCSKicks](http://www.vcskicks.com/license.php)							|
| Paint.NET*		|	[CC-by-NC-ND 2.5](http://creativecommons.org/licenses/by-nc-nd/2.5/)	|
| FloodFill			|	[GNU FDL](http://www.gnu.org/licenses/fdl-1.3.en.html)					|
| SkaaEditor Icon*	|	GPL-compatible															|
| DockPanelSuite	|	[MIT](https://opensource.org/licenses/MIT)								|
| StackOverflow*	|	[CC-by-SA 2.5](http://creativecommons.org/licenses/by-sa/2.5/)			|
| SkaaEditor		|	[MIT](https://opensource.org/licenses/MIT)								|
| CodeReview*		|   [CC-by-SA 3.0](http://creativecommons.org/licenses/by-sa/3.0/)			|

## Details
Below is an explanation of what the above "Components" refer to and their use in this project. Again, I am not a lawyer; consult one if you have questions and *please, please* contact me if you notice anything wrong with the explanations below.

My email is provided in multiple locations below; it is obscured to keep from getting caught up by spambots scraping the page. If you do not trust the link, feel free to post an issue on the GitHub page instead: https://github.com/sraboy/skaa_editor/issues.


---

- **Capslock** is my (Steven Lavoie) personal "organizational tag". It is *not* a legally-incorporated entity. It refers to an encompassing umbrella "organization" to refer to some projects I, or sometimes "we", release as open source. You will notice it by the namespace, project, assembly and/or library names. It is not to be confused with the registered trademarks "Caps Lock" or "Caps-Lock".

---

- **Cyotek** is "Cyotek, Ltd." (http://www.cyotek.com). They've released many open source components, including a ColorPicker and ImageBox, both of which have been adapted for SkaaEditor. ImageBox has been altered to allow for editing and is the Capslock.WinForms.ImageEditor library.

---

- **7KAA (game)** refers to the Seven Kingdoms: Ancient Adversaries game (www.7kfans.com). The entire game, except for the music, has been released by its creator, Enlight Software (http://www.enlight.com), under the GPLv3.

---

- **FastBitmap** is a simple library found on VCSKicks (http://www.vcskicks.com). The library can be found at http://www.vcskicks.com/fast-image-processing2.php. Its license is similar to that of the MIT license in its permissive re-use provisions but you should review the license for specifics.

---

- **Paint.NET** (http://www.getpaint.net) refers to the fantastic image editor. In SkaaEditor, we've used some of the open source project's graphics for buttons/icons. The code itself is MIT but the content is CC-by-NC-ND 2.5. The Paint.NET open source project can be found here: https://code.google.com/p/openpdn/. Note that the present version of Paint.NET is not open source so you must refer to the open source version, openpdn, if looking for the originals. The files below are known to fall under this license. It's possible I've forgotten some so, if you identify files not listed, or known to fall under a different license or copyright, please notify the author: Steven Lavoie (<a href='&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#115;&#116;&#101;&#118;&#101;&#110;&#46;&#108;&#97;&#118;&#111;&#105;&#101;&#106;&#114;&#64;&#103;&#109;&#97;&#105;&#108;&#46;&#99;&#111;&#109;'>&lt;&#111;&#98;&#115;&#99;&#117;&#114;&#101;&#100;&#32;&#039;&#109;&#97;&#105;&#108;&#116;&#111;&#039;&#32;&#108;&#105;&#110;&#107;&gt;</a>) or post an issue on the Github page: https://www.github.com/sraboy/skaa_editor.


|Icons 				  |
|---------------------|
|MenuFileCloseIcon.png|
|MenuFileNewIcon.png  | 
|MenuFileOpenIcon.png |
|MenuFileSaveIcon.png |

--

|Cursors			 |
|--------------------|
|PanToolCursor.cur	 |
|PencilToolCursor.cur|
|PencilToolCursor.cur|

---

- **FloodFill** refers to the Fill() method in Capslock.ImageEditorBox. I've made some modifications which remain under the GNU FDL. 

---

- **SkaaEditor Icon** refers to Kazuya-Hartless.png and Kazuya-Hartless.ico (and any other derivations). Credit for this icon goes to 7kfans.com forums user *Kazuya-Hartless*. The author offered it up for use in the 7KAA project under no specific license; however, it is based on game content under the GPLv3 so it must be used in a manner consistent with that. The author's original forum post can be found here: https://www.7kfans.com/forums/viewtopic.php?f=6&t=344&p=5627#p5566.

---

- **DockPanelSuite** refers to the MDI/docking library that can be found at http://dockpanelsuite.com.

---

- **SkaaEditor** refers to the project overall. All code should be marked appropriately; if not, please email me or post an issue to the GitHub page. I would also greatly appreciate a quick note if you're using any of this code; it's just nice to know that it's useful to someone other than me. I can be reached at (<a href='&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#115;&#116;&#101;&#118;&#101;&#110;&#46;&#108;&#97;&#118;&#111;&#105;&#101;&#106;&#114;&#64;&#103;&#109;&#97;&#105;&#108;&#46;&#99;&#111;&#109;'>&lt;&#111;&#98;&#115;&#99;&#117;&#114;&#101;&#100;&#32;&#039;&#109;&#97;&#105;&#108;&#116;&#111;&#039;&#32;&#108;&#105;&#110;&#107;&gt;</a>). It is my intent that non-GPL'd code be reusable under the more-permissive MIT license. Note, however, that when SkaaEditor is used with content from the 7KAA open source project, SkaaEditor must be released and used in a manner consistent with that project's license, the GPLv3 (i.e., you can't release it commercially and must release the source). This means that you can only use components from SkaaEditor in a non-GPLv3-friendly project if you *completely remove* anything from 7KAA; I've done my best to keep the various components of the project well-separated but it is *your responsibility* to ensure you are complying with the law applicable to you and your project. "Skaa" is not part of the 7KAA project nor was any code directly re-used. Whether or not, what is essentially, the re-implementation of 7KAA's C-code in C# mandates the use of the GPLv3 is an open question; if you've got the answer, let me know. Otherwise, just be aware that the SkaaGameDataLib is the project in question here and it is currently under the MIT license.

---

- **StackOverflow** refers to code adapted from posts on StackOverflow.com. These are marked throughout the code.

---

- **CodeReview** refers to code adapted from posts on codereview.stackexchange.com. These are marked throughout the code.

---