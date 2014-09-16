using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace NetGrab
{
    public class ObservableCollectionEx<t> : ObservableCollection<t>
    {
        // Override the event so this class can access it
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableCollectionEx(IEnumerable<t> collection) : base(collection) { }
        public ObservableCollectionEx(List<t> collection) : base(collection) { }
        public ObservableCollectionEx() { }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var eventHandler = CollectionChanged;
            if (eventHandler != null)
            {
                Delegate[] delegates = eventHandler.GetInvocationList();
                foreach (var @delegate in delegates)
                {
                    var handler = (NotifyCollectionChangedEventHandler)@delegate;
                    var dispatcher = handler.Target as DispatcherObject;
                    if (dispatcher != null && dispatcher.CheckAccess() == false)
                        dispatcher.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                    else
                        handler(this, e);
                }
            }
        }
    }
}
