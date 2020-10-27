using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklaMedabilAPIs
{
    public static class Extensoes
    {
        public static List<T> Getlista<T>(this IEnumerator enumerador)
        {
            var retorno = new List<T>();
            while (enumerador.MoveNext())
            {
                var cc = (T)enumerador.Current;
                if (cc != null)
                {
                    retorno.Add(cc);
                }
            }
            return retorno;
        }
    }
}
