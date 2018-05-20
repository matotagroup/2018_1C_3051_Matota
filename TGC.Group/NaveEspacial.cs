using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group
{
    public class NaveEspacial
    {

        public TgcScene Scene { get; set; }

        public TGCMatrix TransformMatix { get; set; }

        public TGCMatrix ScaleFactor { get; set; }
        public TGCVector3 RotationVector { get; set; }
        public TGCVector3 MovementVector { get; set; }
        public float speed = 1000f;
        private bool shouldBarrelRoll = false;
        private bool shouldLeft90Spin = false;
        private bool shouldRight90Spin = false;
        private bool stopSpinning = false;
        public Arma ArmaPrincipal { get; private set; }
        private TGCVector3 shipShotSize = new TGCVector3(0.4f, 0.3f, 8f);

        public int Vida { get; private set; }  = 100;

        public TgcBoundingOrientedBox OOB
        {
            private set; get;
        }

        public NaveEspacial(string MediaDir, string modelToUse)
        {
            this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/" + modelToUse, MediaDir + "XWing/");
            this.ArmaPrincipal = new Arma(shipShotSize, Color.Red, 10);
            this.TransformMatix = TGCMatrix.Identity;
            this.ScaleFactor = TGCMatrix.Identity;
            this.RotationVector = TGCVector3.Empty;
            this.MovementVector = TGCVector3.Empty;

            this.ActionOnNave((mesh) => {
                mesh.AutoTransform = false; //Desactivar el autotransform para poder usar el mesh.transform.
            });
        }

        public NaveEspacial(string MediaDir, string modelToUse, int danio): this(MediaDir, modelToUse)
        {
            this.ArmaPrincipal.Danio = danio;
        }

        public void Daniar(int cantidadDanio)
        {
            this.Vida -= cantidadDanio;

            //if(this.vida <= 0)
            //Hacer algo cuando muere una nave!
        }

        // TODO: Agregar un target con el mouse o algo para que dispare a cierta direccion no solo para adelante.
        public void Disparar()
        {
            this.ArmaPrincipal.Disparar(this.MovementVector,this.MovementVector-new TGCVector3(0f,0f,1f));
        }

        public void Disparar(TGCVector3 target)
        {
            this.ArmaPrincipal.Disparar(this.MovementVector,target);
        }

        public void CreateOOB()
        {
            //Hacemos un OOB a partir de un AABB mas chico porque el original es muy grande.
            this.OOB = TgcBoundingOrientedBox.computeFromAABB(new TgcBoundingAxisAlignBox(TGCVector3.Multiply(Scene.BoundingBox.PMin, 0.5f), TGCVector3.Multiply(Scene.BoundingBox.PMax, 0.5f)));
            this.OOB.move(this.MovementVector);
            this.OOB.rotate(this.RotationVector);
        }

        public TGCMatrix RotationMatrix()
        {
            return TGCMatrix.RotationYawPitchRoll(RotationVector.Y, RotationVector.X, RotationVector.Z);
        }

        public TGCMatrix MovementMatrix()
        {
            return TGCMatrix.Translation(MovementVector);
        }
        private void PerformBarrelRoll(float ElapsedTime)
        {
            /*TGCVector3 origPosition = new TGCVector3(1, 0, 0);

            var v3 = new TGCVector3(-1, 0, 19);
            var v4 = new TGCVector3(3, 0, -5);
            var angle = FastMath.Acos(TGCVector3.Dot(TGCVector3.Normalize(v3), TGCVector3.Normalize(v4)));
            
            float mm = TGCVector3.Dot(this.RotationVector, origPosition);
            if (FastMath.Acos(mm) < FastMath.TWO_PI)*/

           
            if (this.RotationVector.X > -FastMath.TWO_PI)
                this.Rotate(new TGCVector3( FastMath.ToRad(-500 * ElapsedTime), 0, 0));
            else
            {
                this.Rotate(new TGCVector3(-this.RotationVector.X, 0, 0), false);
                this.OOB.setRotation(new TGCVector3(0,FastMath.PI_HALF,0));
                this.shouldBarrelRoll = false;
            }
            
        }

        public void DoBarrelRoll()
        {
            this.shouldBarrelRoll = true;
        }

        private void PerformLeft90Spin(float ElapsedTime)
        {

            if (stopSpinning)
            {
                if (this.RotationVector.X < 0)
                    this.Rotate(new TGCVector3(FastMath.ToRad(250*ElapsedTime), 0, 0));
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(-this.RotationVector.X, FastMath.PI_HALF, 0));
                    this.shouldLeft90Spin = false;
                    this.stopSpinning = false;
                }
            }
            else
            {
                if (this.RotationVector.X < FastMath.PI_HALF)
                    this.Rotate(new TGCVector3(FastMath.ToRad(250 *ElapsedTime), 0, 0));
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X + FastMath.PI_HALF, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(FastMath.PI_HALF, FastMath.PI_HALF, 0));
                    this.shouldLeft90Spin = false;
                    this.shouldRight90Spin = true;
                    this.stopSpinning = true;
                }
            }           
        }

        private void PerformRight90Spin(float ElapsedTime)
        {
            if (stopSpinning)
            {
                if (this.RotationVector.X > 0)
                    this.Rotate(new TGCVector3(FastMath.ToRad(-250*ElapsedTime), 0, 0));
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(-this.RotationVector.X, FastMath.PI_HALF, 0));
                    this.shouldRight90Spin = false;
                    this.stopSpinning = false;
                }
            }
            else
            {
                if (this.RotationVector.X > -FastMath.PI_HALF)
                {
                    this.Rotate(new TGCVector3(FastMath.ToRad(-250*ElapsedTime), 0, 0));
                }
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X - FastMath.PI_HALF, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(-FastMath.PI_HALF, FastMath.PI_HALF, 0));
                    this.shouldRight90Spin = false;
                    this.shouldLeft90Spin = true;
                    this.stopSpinning = true;
                }
            }
        }

        public void DoLeft90Spin()
        {
            this.shouldLeft90Spin = true;
            this.shouldRight90Spin = false;
            this.stopSpinning = false;
        }

        public void DoRight90Spin()
        {
            this.shouldRight90Spin = true;
            this.shouldLeft90Spin = false;
            this.stopSpinning = false;
        }

        public void Rotate(TGCVector3 rotation, bool updateOOB = true)
        {
            if(updateOOB)
                this.OOB.rotate(new TGCVector3(rotation.Z, rotation.Y, -rotation.X));
            this.RotationVector = this.RotationVector + rotation;
        }

        public void Move(TGCVector3 newOffset)
        {
            this.OOB.move(newOffset * speed);
            this.MovementVector = this.MovementVector + newOffset * speed;
        }

        public TGCVector3 GetPosition()
        {
            return MovementVector;
        }

        public void Update()
        {
            this.ArmaPrincipal.Update();
        }

        public bool CheckIfMyShotsCollided(NaveEspacial otraNave)
        {
            return this.ArmaPrincipal.CheckShots(otraNave);
        }

        public void Update(float ElapsedTime)
        {
            if (shouldBarrelRoll)
                this.PerformBarrelRoll(ElapsedTime);
            else if (shouldLeft90Spin)
                this.PerformLeft90Spin(ElapsedTime);
            else if (shouldRight90Spin)
                this.PerformRight90Spin(ElapsedTime);
        }

        public void Render(bool renderBoundingBox = false)
        {
            this.OOB.Render();
            
            this.ActionOnNave((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                //if (renderBoundingBox)
                //    mesh.BoundingBox.Render();
            });

            this.ArmaPrincipal.Render();
        }

        public void ActionOnNave(System.Action<TgcMesh> action)
        {
            this.Scene.Meshes.ForEach(action);
        }

        public void UpdateBoundingBox()
        {
            this.ActionOnNave((mesh) => {
                mesh.updateBoundingBox();
            });
        }
    }
}
