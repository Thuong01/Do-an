using Datas.Data;
using Datas.Infrastructures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas.Infrastructures.Cores
{
    public class DbFactory : Disposable, IDbFactory
    {
        private CameraNowContext dbContext;

        public CameraNowContext Init()
        {
            return dbContext ?? (dbContext = new CameraNowContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }

}
