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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SkaaEditorUI.Misc;
using SkaaGameDataLib.Util;

namespace SkaaEditorUI.Presenters
{
    public abstract class PresenterBase<T> : INotifyPropertyChanged, IPresenterBase<T> where T : class
    {
        public static readonly TraceSource Logger = new TraceSource($"{typeof(PresenterBase<T>)}", SourceLevels.All);

        private static OpenFileDialog _dlg;
        private T _gameObject;
        private FileFormats _fileFormat;

        public T GameObject
        {
            get
            {
                return this._gameObject;
            }
            set
            {
                SetField(ref this._gameObject, value, () => OnPropertyChanged());//GetDesignModeValue(() => this.GameObject)));
            }
        }

        public abstract T Load(string filePath, params object[] param);     //Passed as a delegate in Open()
        public abstract bool Save(string filePath, params object[] param);  //Passed as a delegate in Save()
        protected abstract void SetupFileDialog(FileDialog dlg);            //Caled in both Open/Save

        static PresenterBase()
        {
            _dlg = new OpenFileDialog();
        }

        #region PropertyChangedEvent
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        PresenterBase<T1> IPresenterBase<T>.Open<T1>(params object[] loadParam)
        {
            using (var dlg = new OpenFileDialog())
            {
                SetupFileDialog(dlg);
                this.GameObject = dlg.CustomShowDialog(() => this.Load(dlg.FileName, loadParam));
            }
            return this as PresenterBase<T1>;
        }

        bool IPresenterBase<T>.Save<T1>(params object[] loadParam)
        {
            bool result;

            using (var dlg = new SaveFileDialog())
            {
                SetupFileDialog(dlg);
                result = dlg.CustomShowDialog(() => this.Save(dlg.FileName, loadParam));
            }

            return result;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="propertyExpression"></param>
        ///// <returns></returns>
        ///// <remarks> 
        ///// This specific method is licensed under CC-BY-SA v2.5 as it was adapted from a StackOverflow user post. 
        ///// Original: http://stackoverflow.com/questions/4364888/how-to-get-property-name-from-within-getter-setter-of-that-property
        ///// License: CC-BY-SA v2.5 (http://creativecommons.org/licenses/by-sa/2.5/)
        ///// </remarks>
        //public static string GetDesignModeValue<T1>(Expression<Func<T1>> propertyExpression)
        //{
        //    //adapted from: http://stackoverflow.com/questions/4364888/how-to-get-property-name-from-within-getter-setter-of-that-property
        //    return (propertyExpression.Body as MemberExpression).Member.Name;
        //}

        /// <summary>
        /// Uses the default comparer to compare <paramref name="field"/> and <paramref name="value"/> and,
        /// if different, sets <paramref name="field"/> to the value specified and calls the delegate
        /// specified by <paramref name="onPropertyChanged"/>.
        /// </summary>
        /// <param name="field">The calling accessor's backing field</param>
        /// <param name="value">The new value being set</param>
        /// <param name="onPropertyChanged">The <see cref="PropertyChanged"/> delegate to be called if there was a change</param>
        /// <returns>False if the two values are equal; true otherwise</returns>
        /// <remarks> 
        /// This specific method is licensed under CC-BY-SA v2.5 as it was adapted from a StackOverflow user post. 
        /// Original: http://stackoverflow.com/questions/4364888/how-to-get-property-name-from-within-getter-setter-of-that-property
        /// License: CC-BY-SA v2.5 (http://creativecommons.org/licenses/by-sa/2.5/)
        /// </remarks>
        public static bool SetField<T1>(ref T1 field, T1 value, Action onPropertyChanged)
        {
            //adapted from: http://stackoverflow.com/questions/4364888/how-to-get-property-name-from-within-getter-setter-of-that-property
            if (EqualityComparer<T1>.Default.Equals(field, value))
                return false;
            field = value;
            onPropertyChanged();
            return true;
        }
    }
}
