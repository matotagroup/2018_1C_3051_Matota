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

        private static List<Tuple<TGCVector3, bool>> posicionesRelativas = new List<Tuple<TGCVector3, bool>>
        {
            new Tuple<TGCVector3,bool> (new TGCVector3 (500f,0,-6000f),true),
            new Tuple<TGCVector3,bool> (new TGCVector3 (-500f,0, 6000f),true),
            new Tuple<TGCVector3,bool> ( new TGCVector3 (150f,0,-3000f),true),
            new Tuple<TGCVector3,bool> (new TGCVector3 (-150f,0,-3000f),true),
            new Tuple<TGCVector3,bool> (new TGCVector3 (-500f,0,-10000f),true),
            new Tuple<TGCVector3,bool> (new TGCVector3 (500f,0,-10000f),true)
        };

        public NaveEnemiga(string MediaDir, string modelToUse, int danio, int cdDisparo, NaveEspacial naveAPerseguir) : base(MediaDir, modelToUse, danio, cdDisparo)
        {
            this.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.RotationVector = new TGCVector3(0, -FastMath.PI_HALF, 0);
            this.naveAPerseguir = naveAPerseguir;

        }

        private bool EsIgual(float num1, float num2)
        {
            return Math.Abs(num1 - num2) < 7.5f;
        }

        private bool EstaAlineado()
        {
            return EsIgual(MovementVector.X, naveAPerseguir.GetPosition().X) && EsIgual(MovementVector.Y, naveAPerseguir.GetPosition().Y);
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

        private float ObtenerDireccion(float coordenadaNave, float coordenadaNaveAPerseguir)
        {
            if (EsIgual(coordenadaNave, coordenadaNaveAPerseguir))
                return 0;
            else if (coordenadaNave > coordenadaNaveAPerseguir)
                return -0.25f;
            else
                return 0.25f;
        }

        private TGCVector3 ObtenerMovimiento()
        {
            var movimiento = TGCVector3.Empty;
            movimiento.X = ObtenerDireccion(MovementVector.X, naveAPerseguir.GetPosition().X);
            movimiento.Y = ObtenerDireccion(MovementVector.Y, naveAPerseguir.GetPosition().Y);
            movimiento.Z = 0.5f;
            return movimiento;
        }

        public void Perseguir(float elapsedTime)
        {
            Move(ObtenerMovimiento() * elapsedTime);
            if (PuedeDisparar())
                Disparar(naveAPerseguir.GetPosition());
        }

        public void Relocate()
        {
            this.MovementVector = naveAPerseguir.GetPosition() + getRelativePosition();
            ArmaPrincipal.Move(MovementVector);
            this.OOB.Center = MovementVector;
            Revivir();
        }

        private TGCVector3 getRelativePosition()
        {
            Tuple<TGCVector3,bool> posicion = posicionesRelativas[new Random().Next(posicionesRelativas.FindAll(tupla => tupla.Item2).Count)];
            posicionesRelativas[posicionesRelativas.FindIndex(p=>p.Item1==posicion.Item1)] = new Tuple<TGCVector3, bool>(posicion.Item1,false);
            return posicion.Item1;
        }

        public static void resetearPosiciones()
        {
            posicionesRelativas=posicionesRelativas.Select(tupla => new Tuple<TGCVector3, bool>(tupla.Item1, true)).ToList();
        }
    }
}
