using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NetGrab.Annotations;

namespace NetGrab
{
    public class NotifyingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator("propertyName")]
        protected void SetValue<T>(ref T oldValue, T newValue, string propertyName)
        {
            if (Equals(oldValue, newValue))
                return;

            oldValue = newValue;
            NotifyPropertyChanged(propertyName);
        }
    }
}
