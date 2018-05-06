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


        public NaveEnemiga(string MediaDir, string modelToUse,TGCVector3 relativePosition,NaveEspacial naveAPerseguir) : base (MediaDir, modelToUse)
        {
            this.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.RotationVector = new TGCVector3(0, -FastMath.PI_HALF, 0);
            this.MovementVector = naveAPerseguir.MovementVector + relativePosition;
        }
        //void Atacar();
    }
}
