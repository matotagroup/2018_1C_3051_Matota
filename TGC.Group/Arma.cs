using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;


namespace TGC.Group
{
    class Arma
    {
        private List<Disparo> disparos;
        private TGCVector3 shotSize;
        private Color shotColor;

        public Arma(TGCVector3 tamanioDisparo, Color colorDisparo)
        {
            this.shotSize = tamanioDisparo;
            this.shotColor = colorDisparo;
            this.disparos = new List<Disparo>();
        }
        

        // TODO: Agregar un target con el mouse o algo para que dispare a cierta direccion no solo para adelante.
        public void Disparar(TGCVector3 startPosition,TGCVector3 targetPosition)
        {
            this.disparos.Add(new Disparo(startPosition,targetPosition,shotSize,shotColor));
        }

        public void Update()
        {

            //hay que hacer un for feo para no dar tanta vuelta con el tema de la modificacion de la lista en tiempo de ejecucion
            for (var x = 0; x < disparos.Count; x++)
                disparos[x].Live(this.disparos);
        }


        public void Render(bool renderBoundingBox = false)
        {
            this.disparos.ForEach((disparo) =>
            {
                disparo.Render();
            });
        }
    }
}
