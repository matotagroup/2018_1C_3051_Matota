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
    class Escenario
    {

        public TgcScene Scene { get; private set; }

        public TGCMatrix TransformMatix { get; set; }

        public TGCMatrix ScaleFactor { get; set; }
        public TGCVector3 RotationVector { get; set; }
        public TGCVector3 MovementVector { get; set; }
        public int SceneNumber { get; private set; } = 0;
        public List<Torre> torres;


        public static readonly TGCVector3 offsetEscenarios = new TGCVector3(0, 0, -8000f);
        public static readonly TGCVector3 defaultScale = new TGCVector3(50f, 200f, 80f);
        private static List<TGCVector4> posicionesTorres = new List<TGCVector4> {
            new TGCVector4(711.83f, -1000, 4500, 0),
            new TGCVector4(1799.243f,-946.1815f,1775.645f, FastMath.PI),
            new TGCVector4(2664,-1099, 120, FastMath.PI),
            new TGCVector4(662.0941f, -1126.118f, -371.27f, 0),
            new TGCVector4(-1351,-1100,2112, 0),
        };
        private static readonly Dictionary<TgcBoundingAxisAlignBox, TGCVector3> boundingBoxes = new Dictionary<TgcBoundingAxisAlignBox, TGCVector3>
        {
            { new TgcBoundingAxisAlignBox(new TGCVector3(0,-2770.372f,-4000), new TGCVector3(4208.421f,0,4000)) , new TGCVector3(1500,-1000,0) },
            { new TgcBoundingAxisAlignBox(new TGCVector3(-700,-2770.372f,-4000), new TGCVector3(4208.421f,0,4000)) , new TGCVector3(-3300,-1200,0) }
        };
 
        public static Escenario GenerarEscenarioDefault(string MediaDir, int numeroDeEscenario)
        {
            Escenario e = new Escenario(MediaDir, "XWing/death+star-TgcScene.xml", numeroDeEscenario);
            e.ScaleFactor = TGCMatrix.Scaling(Escenario.defaultScale);
            e.RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0);
            e.MovementVector = e.GetOffsetVectorMoved();
            e.UpdateBoundingBox();
            e.generarTorres(MediaDir, 2);
            return e;
        }

        public Escenario(string MediaDir, string modelToUse)
        {
            this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + modelToUse, MediaDir + "XWing/");
            this.TransformMatix = TGCMatrix.Identity;
            this.ScaleFactor = TGCMatrix.Identity;
            this.RotationVector = TGCVector3.Empty;
            this.MovementVector = TGCVector3.Empty;
            this.ForEachMesh((mesh) => {
                mesh.AutoTransform = false;

            });
            this.torres = new List<Torre>();
        }

        public Escenario(string MediaDir, string modelToUse, int sceneNumber) : this(MediaDir, modelToUse)
        {
            this.SceneNumber = sceneNumber;
        }

        public void MoveScene(int timesToMove)
        {
            SceneNumber += timesToMove;
            torres.ForEach(t => t.Relocate( GetTowerPosition() ));
        }

        private TGCVector4 GetTowerPosition()
        {
            TGCVector4 temp = posicionesTorres[new Random().Next(posicionesTorres.Count)];
            temp.Z += this.GetOffsetVectorMoved().Z;
            return temp;
        }

        public void generarTorres(string MediaDir, int cantidad)
        {   
            for(int i = 0; i < cantidad; i++)
            {
                var torre = new Torre(MediaDir);
                torre.Relocate(GetTowerPosition());
                torres.Add(torre);
            }
        }

        public TGCVector3 GetOffsetVectorMoved()
        {
            return offsetEscenarios + new TGCVector3(0, 0, offsetEscenarios.Z * (SceneNumber - 1));
        }

        public void UpdateBoundingBox()
        {
            this.Scene.BoundingBox.scaleTranslate(this.MovementVector, defaultScale);
        }

        public List<Torre> torresEnRango(TGCVector3 targetPosition)
        {
            return torres.FindAll(torre => torre.enRango(targetPosition));
        }

        public void Render(bool renderBoundingBox = false)
        {
            this.ForEachMesh((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                if (renderBoundingBox)
                    mesh.BoundingBox.Render();
            });
                this.torres.ForEach(torre => torre.Render());

            foreach (KeyValuePair<TgcBoundingAxisAlignBox, TGCVector3> entry in boundingBoxes)
            {
                entry.Key.scaleTranslate(entry.Value + this.GetOffsetVectorMoved(), TGCVector3.One);
                entry.Key.Render();
            }

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
            this.torres.ForEach(torre => torre.Dispose());
        }
    }
}
