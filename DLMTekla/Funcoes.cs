using Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Model;

namespace TeklaMedabilAPIs
{
   public class Funcoes
    {
        public static Tekla.Structures.Model.UI.Color GetCor()
        {
            Random rnd = new Random();
            return new Tekla.Structures.Model.UI.Color((double)rnd.Next(0, 100)/100, (double)rnd.Next(0, 100)/100, (double)rnd.Next(0, 100)/100);
        }
    }
}
