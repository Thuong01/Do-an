using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas.Extensions.Responses
{
    public class Responses<T>
    {
        public Responses()
        {

        }

        public Responses(T result)
        {
            Succeeded = true;
            Result = result;
        }

        public T Result { get; set; }
        public bool Succeeded { get; set; }
    }
}
