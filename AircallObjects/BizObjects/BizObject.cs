using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public abstract class BizObject : IDisposable
    {
        protected abstract void _LoadFromDb(ref DataRow drRow);

        public abstract void AddInsertParams(ref IDataLib dataLib);

        public abstract void AddUpdateParams(ref IDataLib dataLib);

        public abstract void AddSearchParams(ref IDataLib dataLib);

        public virtual void Dispose()
        {

        }
    }
}
