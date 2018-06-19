using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;

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
             new TGCVector4(1799.243f,-1200f,1775.645f,FastMath.PI),
             new TGCVector4(2000,-999, 5000, FastMath.PI),
             new TGCVector4(2100,-999, -100, FastMath.PI),
             new TGCVector4(3000,-1099, 600, FastMath.PI),
             new TGCVector4(2500,-999, -300, FastMath.PI),
             new TGCVector4(-1351,-1100,2112, 0),
             new TGCVector4(590f, -1000f, -450f, 0),
             new TGCVector4(1800,-999, 1000, FastMath.PI),
             new TGCVector4(300f, -1000f, -1300f, 0),
             new TGCVector4(350f, -1000f, -3000f, 0),
             new TGCVector4(-100f, -1000f, -4000f, 0),

        };

        private static readonly List<TGCBox> boundingBoxesTowers = new List<TGCBox> {
            { TGCBox.fromExtremes(new TGCVector3(850, -1201, 2203),     new TGCVector3(550, -430, 2064))  },
            { TGCBox.fromExtremes(new TGCVector3(564, -1346, 765),      new TGCVector3(90, 95, 576)) },
            { TGCBox.fromExtremes(new TGCVector3(-1501,-1209,219),      new TGCVector3(-2000, 1828, -14)) },
            { TGCBox.fromExtremes(new TGCVector3(1721, -1099, -1639),   new TGCVector3(1520, -636, -1760)) },
            { TGCBox.fromExtremes(new TGCVector3(1545, -780, -1600),    new TGCVector3(1425, -650,-1751)) },
            { TGCBox.fromExtremes(new TGCVector3(-1720, -1211, -1220),  new TGCVector3(-2038, -67, -1386))},
            { TGCBox.fromExtremes(new TGCVector3(-1613, -275, -1244),   new TGCVector3(-1785,-110,-1368)) },
            { TGCBox.fromExtremes(new TGCVector3(3177,-1085,-1628),     new TGCVector3(2700, 30, -1929))  },
            { TGCBox.fromExtremes(new TGCVector3(-2223, -1195, 2536),   new TGCVector3(-2740, 100, 2200))  },
             //PISO
            { TGCBox.fromExtremes(new TGCVector3(-2223, -3000, 4000),   new TGCVector3(2700,-2042,-9000)) },

            //DERECHA
            { TGCBox.fromExtremes(new TGCVector3 (964, -2042,4000) , new TGCVector3 (-2223,-1100,-9000)) },
            
            //IZQUIERDA
           { TGCBox.fromExtremes(new TGCVector3  (2700, -2042,4000),  new TGCVector3 (1495 ,-1053,-9000)) },

            //BB DE PUENTE
            { TGCBox.fromExtremes(new TGCVector3  (1544, -1537, -2873),  new TGCVector3 (830,-1255,-2974)) },
        };

        private static readonly Dictionary<TGCBox, TGCVector3> notDeathfulBB = new Dictionary<TGCBox, TGCVector3> {
            //TECHO
            { TGCBox.fromExtremes(new TGCVector3(3508, 200,4000),   new TGCVector3(-3050,400,-9000)) , new TGCVector3(0,1,0) },
            //Izq
            { TGCBox.fromExtremes(new TGCVector3(3508,-2770,-4000),  new TGCVector3(3550,2770,4000)), new TGCVector3(1,0,0)  },
            //Der
            { TGCBox.fromExtremes(new TGCVector3  (-3050,-2770,-4000),  new TGCVector3 (-3008,2770,4000)), new TGCVector3(1,0,0)  },
        };

        private static readonly Dictionary<TgcBoundingAxisAlignBox, TGCVector3> boundingBoxes = new Dictionary<TgcBoundingAxisAlignBox, TGCVector3>
        {
            //Piso -> 
            { new TgcBoundingAxisAlignBox(new TGCVector3(0,-2770.372f,-4000),       new TGCVector3(4208.421f,0,4000)) , new TGCVector3(1500,-1000,0) },
            { new TgcBoundingAxisAlignBox(new TGCVector3(-700,-2770.372f,-4000),    new TGCVector3(4208.421f,0,4000)) , new TGCVector3(-3300,-1200,0) },
        };
 
        public static Escenario GenerarEscenarioDefault(string MediaDir, int numeroDeEscenario)
        {
            Escenario e = new Escenario(MediaDir, "XWing/death+star-TgcScene.xml", numeroDeEscenario)
            {
                ScaleFactor = TGCMatrix.Scaling(Escenario.defaultScale),
                RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0)
            };

            e.MovementVector = e.GetOffsetVectorMoved();
            e.UpdateBoundingBox();
            e.GenerarTorres(MediaDir, 2);

            SetUpTerrainLight();
            e.ApplyLightEffect();

            return e;
        }

        public Escenario(string MediaDir, string modelToUse)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            this.Scene = loader.loadSceneFromFile(MediaDir + modelToUse, MediaDir + "XWing/");

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

        public void GenerarTorres(string MediaDir, int cantidad)
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
            //return offsetEscenarios + new TGCVector3(0, 0, offsetEscenarios.Z * (SceneNumber - 1));
            return offsetEscenarios * SceneNumber;
        }

        public void UpdateBoundingBox()
        {
            this.Scene.BoundingBox.scaleTranslate(this.MovementVector, defaultScale);
        }

        public List<Torre> TorresEnRango(TGCVector3 targetPosition)
        {
            return torres.FindAll(torre => torre.EnRango(targetPosition));
        }

        public void Render(TGCVector3 lightPosition, TGCVector3 lookFrom, float Elapsedtime, bool renderBoundingBox = false)
        {
            UpdateTerrainLight(lightPosition, lookFrom, Elapsedtime);

            this.ForEachMesh((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                if (renderBoundingBox)
                    mesh.BoundingBox.Render();
            });

            this.torres.ForEach(torre => torre.Render(renderBoundingBox));

            if (renderBoundingBox)
            {
                boundingBoxesTowers.ForEach(m => m.BoundingBox.Render());
                foreach (KeyValuePair<TgcBoundingAxisAlignBox, TGCVector3> entry in boundingBoxes)
                    entry.Key.Render();
                foreach (KeyValuePair<TGCBox, TGCVector3> entry in notDeathfulBB)
                    entry.Key.BoundingBox.Render();
            }
        }

        public void ApplyLightEffect()
        {
            //Unimos todos los meshes que hay en un escenario en una lista y les aplicamos el effect.
            Scene.Meshes.Union(torres.SelectMany(t => t.Scene.Meshes)).ToList().ForEach(m =>  m.Effect = TgcShaders.Instance.TgcMeshPointLightShader );
        }

        //En caso que el TgcMeshPointLightShader shader se reutilizara en otro lado habria que poner este metodo al principio del render para que 
        // vuelva a poner los valores en lo que nosotros queremos, como hacemos con UpdateTerrainLight.
        public static void SetUpTerrainLight()
        {
            var effect = TgcShaders.Instance.TgcMeshPointLightShader;

            effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.Black));
            effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(255, 65, 65, 65)));

            effect.SetValue("lightIntensity", 40f);
            effect.SetValue("lightAttenuation", 0.01f);//0.0099f);

            effect.SetValue("lightColor", ColorValue.FromColor(Color.White));

            effect.SetValue("materialSpecularExp", 1000);
        }

        public void UpdateTerrainLight(TGCVector3 lightPosition, TGCVector3 lookFrom, float ElapsedTime)
        {
            var effect = TgcShaders.Instance.TgcMeshPointLightShader;
            // A la posicion de la luz le restamos 3000 en Z para que este un poco mas adelante en el terreno, el objetivo es que se vea mejor el specular como si estuviera un poco adelante nuestro.
            effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPosition - new TGCVector3(0,0,3000)));
            effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(lookFrom));
            effect.SetValue("time", ElapsedTime);
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

        public TGCVector3 CheckLimits(NaveEspacial nave, TGCVector3 movement)
        {
            TGCVector3 counterMovement = TGCVector3.Empty;

            foreach (KeyValuePair<TGCBox, TGCVector3> entry in notDeathfulBB)
            {
                entry.Key.Transform = TGCMatrix.Translation(this.GetOffsetVectorMoved());
                entry.Key.BoundingBox.transform(entry.Key.Transform);
                if (TgcCollisionUtils.testObbAABB(nave.OOB, entry.Key.BoundingBox))
                    counterMovement += new TGCVector3(movement.X * entry.Value.X, movement.Y * entry.Value.Y, 0);
            }

            return counterMovement;
        }

        public bool CheckCollision(NaveEspacial nave)
        {
            TGCVector3 offset = this.GetOffsetVectorMoved();
            foreach (KeyValuePair<TgcBoundingAxisAlignBox, TGCVector3> entry in boundingBoxes)
            {
                entry.Key.scaleTranslate(entry.Value + offset, TGCVector3.One);
                if (TgcCollisionUtils.testObbAABB(nave.OOB, entry.Key))
                    return true;
            }

            foreach(var m in boundingBoxesTowers)
            {
                m.Transform = TGCMatrix.Translation(offset);
                m.BoundingBox.transform(m.Transform);
                if (TgcCollisionUtils.testObbAABB(nave.OOB, m.BoundingBox))
                    return true;
            }

            return torres.FindAll(t => TgcCollisionUtils.testObbAABB(nave.OOB, t.Scene.BoundingBox) ).Count > 0;
        }

        public void Update()
        {

        }

        public void Dispose()
        {
            this.ForEachMesh(mesh => { mesh.Dispose(); });
            this.torres.ForEach(torre => torre.Dispose());
        }
    }
}
