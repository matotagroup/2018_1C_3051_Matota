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
using TGC.Core.Textures;

namespace TGC.Group
{
    public class Disparo
    {
        private TGCBox modelo;
        private int tiempoDisparo = 1000;
        private Stopwatch vida = null;
        public bool ShouldDie { get; private set; }
        private TGCVector3 MovementDirection;

        private const float velocidadDisparo = -5f;


        public Disparo(TGCVector3 startPosition,TGCVector3 targetPosition,TGCVector3 size, Color color)
        {
            vida = Stopwatch.StartNew();
            ShouldDie = false;
            modelo = TGCBox.fromSize(size, color);
            modelo.AutoTransform = true;
            modelo.Position = startPosition;
            MovementDirection = targetPosition;

            //MovementDirection = targetPosition - startPosition;
        }

        public void Live(List<Disparo> disparos)
        {
            if (vida.Elapsed.TotalMilliseconds >= tiempoDisparo)
            {
                ShouldDie = true;
                this.Dispose();
                disparos.Remove(this);
            }

            modelo.Move(MovementDirection*velocidadDisparo);
        }

        public bool HayColision(NaveEspacial nave)
        {
            return TgcCollisionUtils.testObbAABB(nave.OOB, this.modelo.BoundingBox);
        }

        public void Render()
        {
            modelo.BoundingBox.Render();
            if(!ShouldDie)
                this.modelo.Render();
        }

        public void Dispose()
        {
            this.modelo.Dispose();
        }
    }
}
