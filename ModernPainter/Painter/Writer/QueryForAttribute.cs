using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Writer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class QueryForAttribute : Attribute
    {
        public Type DatabaseType { get; }
        public QueryForAttribute(Type databaseType) => DatabaseType = databaseType;
    }
}
