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
using TGC.Core.BoundingVolumes;
using TGC.Core.Textures;

namespace TGC.Group
{
    public class Disparo
    {
        private TGCBox modelo;
        private int tiempoDisparo = 5000;
        private Stopwatch vida = null;
        public bool ShouldDie { get; private set; }
        private TGCVector3 MovementDirection;

        private const float velocidadDisparo = -5f;

        public TgcBoundingOrientedBox OOB
        {
            private set; get;
        }

        public float obtenerRotacionX(TGCVector3 a, TGCVector3 b)
        {
            var rotacionAbs = obtenerRotacion(new TGCVector3(0, a.Y, a.Z), new TGCVector3(0, b.Y, b.Z));
            if (a.Y > b.Y)
                return FastMath.TWO_PI - rotacionAbs;
            else
                return rotacionAbs;
        }
        public float obtenerRotacionY(TGCVector3 a, TGCVector3 b)
        {
            var rotacionAbs= obtenerRotacion(new TGCVector3(a.X, 0, a.Z), new TGCVector3(b.X, 0, b.Z));
            if (a.X > b.X)

                return FastMath.TWO_PI - rotacionAbs; 
            else
                return rotacionAbs;
        }

        public float obtenerRotacion(TGCVector3 a,TGCVector3 b)
        {
            return FastMath.Acos(TGCVector3.Dot(a, b) / (a.Length() * b.Length()));
        }

        public Disparo(TGCVector3 startPosition,TGCVector3 targetPosition,TGCVector3 size, Color color)
        {
            vida = Stopwatch.StartNew();
            ShouldDie = false;
            modelo = TGCBox.fromSize(size, color);
            modelo.AutoTransform = true;
            modelo.Position = startPosition;
            MovementDirection = TGCVector3.Normalize(startPosition-targetPosition);
            modelo.Rotation = new TGCVector3(obtenerRotacionX(new TGCVector3(0, 0, 1), MovementDirection), obtenerRotacionY(new TGCVector3(0, 0, 1), MovementDirection),0);
            this.OOB = TgcBoundingOrientedBox.computeFromAABB(modelo.BoundingBox);
            //this.OOB.move(this.MovementDirection);
            this.OOB.rotate(modelo.Rotation);
            //modelo.BoundingBox.transform(TGCMatrix.RotationYawPitchRoll(modelo.Rotation.Y,modelo.Rotation.X,0));
        }

        public void Live(List<Disparo> disparos)
        {
            if (vida.Elapsed.TotalMilliseconds >= tiempoDisparo)
            {
                ShouldDie = true;
                this.modelo.Dispose();
                this.OOB.Dispose();
                this.Dispose();
                disparos.Remove(this);
            }

            OOB.move(MovementDirection * velocidadDisparo);
            modelo.Move(MovementDirection*velocidadDisparo);
        }

        public bool HayColision(NaveEspacial nave)
        {
            return TgcCollisionUtils.testObbAABB(nave.OOB, this.modelo.BoundingBox);
        }

        public void Render()
        {
            this.OOB.Render();
            if(!ShouldDie)
                this.modelo.Render();
        }

        public void Dispose()
        {
            this.modelo.Dispose();
        }
    }
}
