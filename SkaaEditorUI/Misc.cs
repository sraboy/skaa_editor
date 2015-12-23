using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkaaEditorUI
{
    public static class Misc
    {
        public static string GetDesignModeValue<T>(Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }

        public static bool SetField<T>(ref T field, T value, Action onPropertyChanged)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            onPropertyChanged();
            return true;
        }
    }
}
