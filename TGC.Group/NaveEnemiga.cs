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
        private float distancia;

        public NaveEnemiga(string MediaDir, string modelToUse, int danio, int cdDisparo, NaveEspacial naveAPerseguir,float distanciaALaNave) : base (MediaDir, modelToUse, danio,cdDisparo)
        {
            this.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.RotationVector = new TGCVector3(0, -FastMath.PI_HALF, 0);
            this.naveAPerseguir = naveAPerseguir;
            this.distancia = distanciaALaNave;
            //this.Vida = 0;
        }

    
        private bool EsIgual(float num1, float num2)
        {
            return Math.Abs(num1 - num2) < 7.5f;
        }

        private bool EstaAlineado()
        {
            return EsIgual(MovementVector.X,naveAPerseguir.GetPosition().X) && EsIgual(MovementVector.Y,naveAPerseguir.GetPosition().Y);
        }

        public bool EnemigoEstaAdelante()
        {
            return naveAPerseguir.GetPosition().Z > GetPosition().Z;
        }

        public bool EnemigoDentroDeAlcanceDisparo()
        {
            TGCVector3 posicionRelativa = naveAPerseguir.GetPosition() - GetPosition();
            return FastMath.Pow2(posicionRelativa.Z / 10) > (FastMath.Pow2(posicionRelativa.X) + FastMath.Pow2(posicionRelativa.Y));
        }

        private bool PuedeDisparar()
        {
            return EnemigoEstaAdelante() && EnemigoDentroDeAlcanceDisparo();
        }

        private float ObtenerDireccion(float coordenadaNave,float coordenadaNaveAPerseguir)
        {
            if (EsIgual(coordenadaNave,coordenadaNaveAPerseguir))
                 return 0;
            else if (coordenadaNave > coordenadaNaveAPerseguir)
                return -0.75f;
            else 
                return 0.75f;
        }

        private bool EstaLejos()
        {
            return Math.Abs(MovementVector.Z - naveAPerseguir.GetPosition().Z) > distancia;
        }

        private void ObtenerMovimiento(ref TGCVector3 movimiento)
        {

            movimiento.X = ObtenerDireccion(MovementVector.X, naveAPerseguir.GetPosition().X);
            movimiento.Y = ObtenerDireccion(MovementVector.Y, naveAPerseguir.GetPosition().Y);
            movimiento.Z = ObtenerDireccion(MovementVector.Z, naveAPerseguir.GetPosition().Z);
        }

        private bool EstaCerca()
        {
            return Math.Abs(MovementVector.Z - naveAPerseguir.GetPosition().Z) < 250f;
        }

        //private void Acercarse(ref TGCVector3 movimiento)
        //{
        //    movimiento.Z = ObtenerDireccion(MovementVector.Z, naveAPerseguir.GetPosition().Z)*0.000001f;
        //}

        public void Perseguir(float elapsedTime)
        {
            var movimiento = TGCVector3.Empty;
            if (!this.EstaAlineado()||EstaLejos())
            {
                this.ObtenerMovimiento(ref movimiento);
                this.Move(movimiento * elapsedTime);
            }
           if(PuedeDisparar())
                Disparar(naveAPerseguir.GetPosition());
        }

        public void Relocate (TGCVector3 posicionRelativaNueva)
        {
            this.MovementVector = naveAPerseguir.GetPosition() + posicionRelativaNueva;
            ArmaPrincipal.Move(MovementVector);
            this.OOB.Center = MovementVector;
            this.Vida = 100;
        }
    }
}
