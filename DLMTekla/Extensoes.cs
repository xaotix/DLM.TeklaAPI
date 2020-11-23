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
    public static class Cores
    {
        public static Tekla.Structures.Model.UI.Color azul { get; set; } = new Tekla.Structures.Model.UI.Color(0,0,1);
        public static Tekla.Structures.Model.UI.Color vermelho { get; set; } = new Tekla.Structures.Model.UI.Color(1,0,0);
        public static Tekla.Structures.Model.UI.Color verde { get; set; } = new Tekla.Structures.Model.UI.Color(0,1,0);
    }
}
