using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group
{
    class NaveEnemiga : NaveEspacial
    {
        private NaveEspacial naveAPerseguir;
        private bool estaListoParaAtacar;
        private float distancia;

        public NaveEnemiga(string MediaDir, string modelToUse,TGCVector3 relativePosition,NaveEspacial naveAPerseguir,float distanciaALaNave) : base (MediaDir, modelToUse)
        {
            this.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.RotationVector = new TGCVector3(0, -FastMath.PI_HALF, 0);
            this.MovementVector = naveAPerseguir.MovementVector + relativePosition;
            this.naveAPerseguir = naveAPerseguir;
            this.estaListoParaAtacar = false;
            this.distancia = distanciaALaNave;
        }
        private bool esIgual(float num1, float num2)
        {
            return Math.Abs(num1 - num2) < 10f;
        }
        private bool estaAlineado()
        {
            return esIgual(MovementVector.X,naveAPerseguir.GetPosition().X) && esIgual(MovementVector.Y,naveAPerseguir.GetPosition().Y);
        }
        private float obtenerDireccion(float coordenadaNave,float coordenadaNaveAPerseguir)
        {
            if (esIgual(coordenadaNave,coordenadaNaveAPerseguir))
                 return 0;
            else if (coordenadaNave > coordenadaNaveAPerseguir)
                return -0.75f;
            else 
                return 0.75f;
        }
        private bool estaLejos()
        {
            return Math.Abs(MovementVector.Z - naveAPerseguir.GetPosition().Z) > distancia;
        }

        private void obtenerMovimiento(ref TGCVector3 movimiento)
        {

            movimiento.X = obtenerDireccion(MovementVector.X, naveAPerseguir.GetPosition().X);
            movimiento.Y = obtenerDireccion(MovementVector.Y, naveAPerseguir.GetPosition().Y);
            if (this.estaLejos())
                movimiento.Z = this.obtenerDireccion(this.MovementVector.Z, naveAPerseguir.GetPosition().Z);
            else
                movimiento.Z = 0;
        }
        private void acercarse(ref TGCVector3 movimiento)
        {
            //if (this.estaLejos())
                movimiento.Z = obtenerDireccion(MovementVector.Z, naveAPerseguir.GetPosition().Z)*0.000001f;
            //else
            //    movimiento.Z = 0;
        }

        public void perseguir(float elapsedTime)
        {
            var movimiento = TGCVector3.Empty;
            if (!this.estaAlineado()||estaLejos())
            {
                this.obtenerMovimiento(ref movimiento);
                this.Move(movimiento * elapsedTime);
                this.estaListoParaAtacar = true;
            }
            else if(estaAlineado())
                Disparar();
            //if (estaLejos())
            //{
            //    movimiento = TGCVector3.Empty;
            //    this.acercarse(ref movimiento);
            //    this.Move(movimiento * elapsedTime);
            //}
        }
    }
}
