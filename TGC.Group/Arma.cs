﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;


namespace TGC.Group
{
    public class Arma
    {
        private List<Disparo> disparos;
        private TGCVector3 shotSize;
        private Color shotColor;
        private Stopwatch shotLimiter;

        public int Danio { set; get; }


        public Arma(TGCVector3 tamanioDisparo, Color colorDisparo, int danio)
        {
            this.Danio = danio;
            this.shotSize = tamanioDisparo;
            this.shotColor = colorDisparo;
            this.disparos = new List<Disparo>();
            shotLimiter = Stopwatch.StartNew();
        }
        

        // TODO: Agregar un target con el mouse o algo para que dispare a cierta direccion no solo para adelante.
        public void Disparar(TGCVector3 startPosition,TGCVector3 targetPosition)
        {
            if(shotLimiter.ElapsedMilliseconds > 250)
            {
                this.disparos.Add(new Disparo(startPosition,targetPosition,shotSize,shotColor));
                shotLimiter.Restart();
            }
        }

        public bool CheckShots(NaveEspacial nave)
        {
            var cols = disparos.FindAll(t => t.HayColision(nave)).Select( t => disparos.IndexOf(t)).ToList();
            cols.ForEach(e => disparos.RemoveAt(e));
            return cols.Count() > 0;
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
