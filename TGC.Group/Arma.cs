using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Group.Form;

namespace TGC.Group
{
    public class Arma
    {
        private List<Disparo> disparos;
        private TGCVector3 shotSize;
        private Color shotColor;
        private Stopwatch shotLimiter;
        private TGCVector3 position;
        public TGCMatrix TransformMatix { get; set; }

        public int Danio { set; get; }

        private int cooldownDisparo;
        private float minDistance;

        public Arma(TGCVector3 tamanioDisparo, Color colorDisparo, int danio, int cdDisparo, TGCVector3 startPosition, float minDistance = -1f)
        {
            this.Danio = danio;
            this.shotSize = tamanioDisparo;
            this.shotColor = colorDisparo;
            this.disparos = new List<Disparo>();
            shotLimiter = Stopwatch.StartNew();
            position = startPosition;
            cooldownDisparo = cdDisparo;
            this.minDistance = minDistance;

        }


        // TODO: Agregar un target con el mouse o algo para que dispare a cierta direccion no solo para adelante.
        public bool Disparar(TGCVector3 targetPosition, string soundPath, Microsoft.DirectX.DirectSound.Device device)
        {
            if (shotLimiter.ElapsedMilliseconds > cooldownDisparo)
            {
                if (minDistance != -1)
                    disparos.Add(new Disparo(position, targetPosition, shotSize, shotColor, soundPath, device, minDistance));
                else
                    this.disparos.Add(new Disparo(position, targetPosition, shotSize, shotColor, soundPath, device));
                shotLimiter.Restart();
                return true;
            }
            return false;
        }

        public bool CheckShots(NaveEspacial nave)
        {
            var cols = disparos.FindAll(t => t.HayColision(nave)).Select(t => disparos.IndexOf(t)).ToList();
            cols.ForEach(e =>
            {
                try
                {
                    disparos.RemoveAt(e);
                }
                catch (IndexOutOfRangeException)
                {
                    //El disparo ya habia sido removido
                }
            });

            return cols.Count() > 0;
        }

        public void Update(float elapsedTime)
        {

            //hay que hacer un for feo para no dar tanta vuelta con el tema de la modificacion de la lista en tiempo de ejecucion
            for (var x = 0; x < disparos.Count; x++)
                disparos[x].Live(this.disparos, elapsedTime);
        }


        public void Render(bool renderBoundingBox = false)
        {
            this.disparos.ForEach((disparo) =>
            {
                disparo.Render();
            });
        }

        public void Move(TGCVector3 posicionNueva)
        {
            position = posicionNueva;
        }
    }

}


