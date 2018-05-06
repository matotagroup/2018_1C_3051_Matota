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
    class NaveEspacial
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
        private Arma arma;

        public TgcBoundingOrientedBox OOB
        {
            private set; get;
        }

        public NaveEspacial(string MediaDir, string modelToUse)
        {
            this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/" + modelToUse, MediaDir + "XWing/");
            this.arma = new Arma();
            this.TransformMatix = TGCMatrix.Identity;
            this.ScaleFactor = TGCMatrix.Identity;
            this.RotationVector = TGCVector3.Empty;
            this.MovementVector = TGCVector3.Empty;

            this.ActionOnNave((mesh) => {
                mesh.AutoTransform = false; //Desactivar el autotransform para poder usar el mesh.transform.
            });
        }

        // TODO: Agregar un target con el mouse o algo para que dispare a cierta direccion no solo para adelante.
        public void Disparar()
        {
            this.arma.Disparar(this.MovementVector);
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
        private void PerformBarrelRoll()
        {
            /*TGCVector3 origPosition = new TGCVector3(1, 0, 0);

            var v3 = new TGCVector3(-1, 0, 19);
            var v4 = new TGCVector3(3, 0, -5);
            var angle = FastMath.Acos(TGCVector3.Dot(TGCVector3.Normalize(v3), TGCVector3.Normalize(v4)));
            
            float mm = TGCVector3.Dot(this.RotationVector, origPosition);
            if (FastMath.Acos(mm) < FastMath.TWO_PI)*/

            if (this.RotationVector.X > -FastMath.TWO_PI)
                this.Rotate(new TGCVector3(-0.05f, 0, 0));
            else
            {
                this.Rotate(new TGCVector3(-this.RotationVector.X,0,0));
                this.shouldBarrelRoll = false;
            }
        }

        public void DoBarrelRoll()
        {
            this.shouldBarrelRoll = true;
        }

        private void PerformLeft90Spin()
        {

            if (stopSpinning)
            {
                if (this.RotationVector.X < 0)
                    this.Rotate(new TGCVector3(0.05f, 0, 0));
                else
                {
                    this.shouldLeft90Spin = false;
                    this.stopSpinning = false;
                }
            }
            else
            {
                if (this.RotationVector.X < FastMath.PI_HALF)
                    this.Rotate(new TGCVector3(0.05f, 0, 0));
                else
                {
                    this.shouldLeft90Spin = false;
                    this.shouldRight90Spin = true;
                    this.stopSpinning = true;
                }
            }           
        }

        private void PerformRight90Spin()
        {
            if (stopSpinning)
            {
                if (this.RotationVector.X > 0)
                    this.Rotate(new TGCVector3(-0.05f, 0, 0));
                else
                {
                    this.shouldRight90Spin = false;
                    this.stopSpinning = false;
                }
            }
            else
            {
                if (this.RotationVector.X > -FastMath.PI_HALF)
                {
                    this.Rotate(new TGCVector3(-0.05f, 0, 0));
                }
                else
                {
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

        public void Rotate(TGCVector3 rotation)
        {
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
            this.arma.Update();
        }

        public void Render(bool renderBoundingBox = false)
        {
            this.OOB.Render();
            if (shouldBarrelRoll)
                this.PerformBarrelRoll();
            else if (shouldLeft90Spin)
                this.PerformLeft90Spin();
            else if (shouldRight90Spin)
                this.PerformRight90Spin();
            this.ActionOnNave((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                //if (renderBoundingBox)
                //    mesh.BoundingBox.Render();
            });

            this.arma.Render();
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
