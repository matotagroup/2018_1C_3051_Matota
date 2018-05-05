using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group
{
    public class Disparo
    {
        public TGCBox modelo;
        public float tiempoDisparo = 15f;
        public Disparo(TGCBox modeloNuevo)
        {
            modelo = modeloNuevo;
            modelo.AutoTransform = true;
        }
    }
}
