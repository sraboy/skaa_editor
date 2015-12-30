using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace TrulyObservableCollection
{
    /// <summary>
    /// A custom implementation of <see cref="ObservableCollection{T}"/> that raises events for collection member changes by enforcing
    /// the implementation of <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <typeparam name="T">Any type that implements <see cref="INotifyPropertyChanged"/></typeparam>
    /// <remarks>
    /// This specific class is licensed under CC-BY-SA v2.5 as it was adapted from a StackOverflow user post. 
    /// Original: http://stackoverflow.com/questions/1427471/observablecollection-not-noticing-when-item-in-it-changes-even-with-inotifyprop/5256827#5256827
    /// License: CC-BY-SA v2.5 (http://creativecommons.org/licenses/by-sa/2.5/)
    /// </remarks>
    [Serializable]
    public class TrulyObservableCollection<T> : ObservableCollection<T>, INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        //explicitly implemented so the subclass doesn't interfere w/ serialization (http://kentb.blogspot.com/2007/11/serializing-observablecollection.html)

        public TrulyObservableCollection() : base()
        {
            CollectionChanged += new NotifyCollectionChangedEventHandler(TrulyObservableCollection_CollectionChanged);
        }

        private void TrulyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                    (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                    (item as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(a);
        }
    }
}