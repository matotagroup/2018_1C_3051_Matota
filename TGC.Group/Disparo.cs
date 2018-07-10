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
using TGC.Core.Sound;



namespace TGC.Group
{
    public class Disparo
    {
        private TGCBox modelo;
        private int tiempoDisparo = 5000;
        private Stopwatch vida = null;
        public bool ShouldDie { get; private set; }
        private TGCVector3 MovementDirection;
        private Tgc3dSound sonido;


        private const float velocidadDisparo = 8000f;

        public TgcBoundingOrientedBox OOB
        {
            private set; get;
        }

        public float ObtenerRotacionX(TGCVector3 a, TGCVector3 b)
        {
            var rotacionAbs = ObtenerRotacion(new TGCVector3(0, a.Y, a.Z), new TGCVector3(0, b.Y, b.Z));
            if (a.Y > b.Y)
                return FastMath.TWO_PI - rotacionAbs;
            else
                return rotacionAbs;
        }
        public float ObtenerRotacionY(TGCVector3 a, TGCVector3 b)
        {
            var rotacionAbs= ObtenerRotacion(new TGCVector3(a.X, 0, a.Z), new TGCVector3(b.X, 0, b.Z));
            if (a.X > b.X)

                return FastMath.TWO_PI - rotacionAbs; 
            else
                return rotacionAbs;
        }

        public float ObtenerRotacion(TGCVector3 a,TGCVector3 b)
        {
            return FastMath.Acos(TGCVector3.Dot(a, b) / (a.Length() * b.Length()));
        }

        public Disparo(TGCVector3 startPosition, TGCVector3 targetPosition,TGCVector3 size, Color color, string soundPath, Microsoft.DirectX.DirectSound.Device device)
        {
            vida = Stopwatch.StartNew();
            ShouldDie = false;
            modelo = TGCBox.fromSize(size, color);
            modelo.AutoTransform = true;
            modelo.Position = startPosition;
            MovementDirection = TGCVector3.Normalize(targetPosition- startPosition);
            modelo.Rotation = new TGCVector3(ObtenerRotacionX(new TGCVector3(0, 0, 1), MovementDirection), ObtenerRotacionY(new TGCVector3(0, 0, 1), MovementDirection),0);
            this.OOB = TgcBoundingOrientedBox.computeFromAABB(modelo.BoundingBox);
            //this.OOB.move(this.MovementDirection);
            this.OOB.rotate(modelo.Rotation);
            this.sonido = new Tgc3dSound(soundPath, modelo.Position, device);
            sonido.Position = modelo.Position;
            sonido.MinDistance = 50f;
            sonido.play();
            //modelo.BoundingBox.transform(TGCMatrix.RotationYawPitchRoll(modelo.Rotation.Y,modelo.Rotation.X,0));
        }
        public Disparo(TGCVector3 startPosition, TGCVector3 targetPosition, TGCVector3 size, Color color, string soundPath, Microsoft.DirectX.DirectSound.Device device, float minDistance): this(startPosition, targetPosition, size, color, soundPath, device)
        {
            sonido.MinDistance = minDistance;

        }

        public void Live(List<Disparo> disparos, float elapsedTime)
        {
            if (vida.Elapsed.TotalMilliseconds >= tiempoDisparo)
            {
                ShouldDie = true;
                this.modelo.Dispose();
                this.OOB.Dispose();
                sonido.dispose();
                this.Dispose();
                disparos.Remove(this);
            }
            else
            {
                OOB.move(MovementDirection * velocidadDisparo * elapsedTime);
                modelo.Move(MovementDirection * velocidadDisparo * elapsedTime);
                sonido.Position = modelo.Position;
            }
        }

        public bool HayColision(NaveEspacial nave)
        {
            return TgcCollisionUtils.testObbAABB(nave.OOB, this.modelo.BoundingBox);
        }

        public void Render()
        {
            //this.OOB.Render();
            if(!ShouldDie)
                this.modelo.Render();
        }

        public void Dispose()
        {
            this.modelo.Dispose();
            sonido.dispose();
        }
    }
}
