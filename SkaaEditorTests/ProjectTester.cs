#region Copyright Notice
/***************************************************************************
* The MIT License (MIT)
*
* Copyright © 2015-2016 Steven Lavoie
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
* the Software, and to permit persons to whom the Software is furnished to do so,
* subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
* FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
* COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
* IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************/
#endregion
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkaaEditorUI.Forms;
using SkaaEditorUI.Presenters;
using SkaaGameDataLib;

namespace SkaaEditorUnitTester
{
    [TestClass]
    public class ProjectTester
    {
        public TestContext testContext { get; set; }
        public MDISkaaEditorMainForm mainForm = new MDISkaaEditorMainForm();
        public string ProjectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\projects\\_test\\basic\\";


        [TestMethod]
        public void OpenSpriteTest()
        {
            var sprpresenter = new SpritePresenter();
            var palpresenter = new ColorPalettePresenter();

            var pal = palpresenter.Load(ProjectPath + "pal_std.res");
            Assert.IsInstanceOfType(palpresenter.GameObject, typeof(ColorPalette));

            sprpresenter.PalettePresenter = palpresenter;

            sprpresenter.Load(ProjectPath + "ballista.spr", FileFormats.SpriteSpr);
            Assert.IsInstanceOfType(sprpresenter.GameObject, typeof(SkaaSprite));
        }
    }
}
