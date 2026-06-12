using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class QueryForAttribute : Attribute
    {
        public Type DatabaseType { get; }
        public QueryForAttribute(Type databaseType) => DatabaseType = databaseType;
    }
}
