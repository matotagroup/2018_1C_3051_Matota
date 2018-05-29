using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group
{
    class Barra                 //barra puede ser de vida o de afterburn por ejemplo
    {
        private CustomSprite barra = new CustomSprite();
        public TGCVector2 Position { get { return barra.Position; } set { barra.Position = value; } }
        public TGCVector2 Scaling { get { return barra.Scaling; } set { barra.Scaling = value; } }

        public Barra(string MediaDir, string texture)
        {
            barra.Bitmap = new CustomBitmap(MediaDir + "XWing\\Textures\\" + texture, D3DDevice.Instance.Device);
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(barra);
            drawer.EndDrawSprite();
        }
        public void ModificarBarra(float valorModificado)
        {
            var viejo = barra.Scaling.X;
            barra.Scaling = new TGCVector2((valorModificado / 100), barra.Scaling.Y);
            //100 es la vida máxima de la nave y la cantMáxima de combustible de la nave
        }
    }
}