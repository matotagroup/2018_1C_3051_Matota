using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

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

        public TgcBoundingOrientedBox OOB
        {
            private set; get;
        }

        public NaveEspacial(string MediaDir, string modelToUse)
        {
            this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/" + modelToUse, MediaDir + "XWing/");
            this.TransformMatix = TGCMatrix.Identity;
            this.ScaleFactor = TGCMatrix.Identity;
            this.RotationVector = TGCVector3.Empty;
            this.MovementVector = TGCVector3.Empty;

            this.ActionOnNave((mesh) => {
                mesh.AutoTransform = false; //Desactivar el autotransform para poder usar el mesh.transform.
            });
        }

        public void RefreshOOB()
        {

        }

        public void CreateOOB()
        {

            this.OOB = TgcBoundingOrientedBox.computeFromAABB(Scene.BoundingBox);
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

        public void Render(bool renderBoundingBox = false)
        {
            this.OOB.Render();
            if(shouldBarrelRoll)
                this.PerformBarrelRoll();
            this.ActionOnNave((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                if (renderBoundingBox)
                    mesh.BoundingBox.Render();
            });
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
