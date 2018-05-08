﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group
{
    class Escenario
    {

        public TgcScene Scene { get; private set; }

        public TGCMatrix TransformMatix { get; set; }

        public TGCMatrix ScaleFactor { get; set; }
        public TGCVector3 RotationVector { get; set; }
        public TGCVector3 MovementVector { get; set; }

        public int SceneNumber { get; private set; } = 0;

        public static readonly TGCVector3 offsetEscenarios = new TGCVector3(0, 0, -8000f);
        public static readonly TGCVector3 defaultScale = new TGCVector3(50f, 200f, 80f);

        public TGCVector3 GetOffsetVectorMoved()
        {
            return offsetEscenarios + new TGCVector3(0, 0, offsetEscenarios.Z * (SceneNumber -1 ));
        }

        public static Escenario GenerarEscenarioDefault(string MediaDir, int numeroDeEscenario)
        {
            Escenario e = new Escenario(MediaDir, "XWing/death+star-TgcScene.xml", numeroDeEscenario);
            e.ScaleFactor = TGCMatrix.Scaling(Escenario.defaultScale);
            e.RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0);
            e.MovementVector = e.GetOffsetVectorMoved();
            e.UpdateBoundingBox();
            return e;
        }

        public void MoveScene(int timesToMove)
        {
            SceneNumber += timesToMove;
        }

        private static List<TGCVector4> torres = new List<TGCVector4> {
            new TGCVector4(711.83f, -1100, 4000, 0),
            new TGCVector4(1799.243f,-946.1815f,1775.645f, 0),
            new TGCVector4(662.0941f, -1126.118f, -371.27f, 0),
        };

        public Escenario(string MediaDir, string modelToUse)
        {
            this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir  + modelToUse, MediaDir + "XWing/");
            this.TransformMatix = TGCMatrix.Identity;
            this.ScaleFactor = TGCMatrix.Identity;
            this.RotationVector = TGCVector3.Empty;
            this.MovementVector = TGCVector3.Empty;
            this.ForEachMesh((mesh) => {
                mesh.AutoTransform = false; 
            });
        }

        public Escenario(string MediaDir, string modelToUse, int sceneNumber) : this(MediaDir, modelToUse)
        {
            this.SceneNumber = sceneNumber;
        }

        public void UpdateBoundingBox()
        {
            this.Scene.BoundingBox.scaleTranslate(this.MovementVector, defaultScale);
        }

        public void Render(bool renderBoundingBox = false)
        {
            this.ForEachMesh((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                if (renderBoundingBox)
                    mesh.BoundingBox.Render();
            });
        }

        public TGCMatrix RotationMatrix()
        {
            return TGCMatrix.RotationYawPitchRoll(RotationVector.Y, RotationVector.X, RotationVector.Z);
        }

        public TGCMatrix MovementMatrix()
        {
            return TGCMatrix.Translation(MovementVector);
        }

        public void Move(TGCVector3 newOffset)
        {
            this.MovementVector = this.MovementVector + newOffset;
        }

        public void ForEachMesh(System.Action<TgcMesh> action)
        {
            this.Scene.Meshes.ForEach(action);
        }

        public void Dispose()
        {
            this.ForEachMesh(mesh => { mesh.Dispose(); });
        }
    }
}