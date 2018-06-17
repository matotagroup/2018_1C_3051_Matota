using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Input;
using TGC.Group.Model.UtilsParaGUI;
using TGC.Core.Direct3D;

namespace TGC.Group.Model.MenuesImplementados
{
    class MenuInstrucciones : TipoMenu
    {
        private Simbolo parrafo1;
        private Simbolo parrafo2;
        private Simbolo parrafo3;
        private Boton atras;

        public int W = D3DDevice.Instance.Width;
        public int H = D3DDevice.Instance.Height;

        private List<Simbolo> simbolos;
        public MenuInstrucciones(string MediaDir, TgcD3dInput input)
        {
            parrafo1 = new Simbolo(MediaDir, "parrafo1.png", input);
            parrafo2 = new Simbolo(MediaDir, "parrafo2.png", input);
            parrafo3 = new Simbolo(MediaDir, "parrafo3.png", input);
            atras = new Boton(MediaDir, "atras.png", input, "seleccion_atras.png", new AccionAtras());

            parrafo1.Position = new TGCVector2(W / 7.5f, H / 3f);
            parrafo2.Position = new TGCVector2(W / 7.5f, H / 2.5f);
            parrafo3.Position = new TGCVector2(W / 7.5f, H / 2.15f);
            atras.Position = new TGCVector2(75, H / 1.15f);
            atras.Scaling = new TGCVector2(0.4f, 0.4f);

            simbolos = new List<Simbolo>();
            simbolos.Add(parrafo1);
            simbolos.Add(parrafo2);
            simbolos.Add(parrafo3);
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            simbolos.ForEach(simbolo => simbolo.Render(elapsedTime, drawer));
            atras.Render(elapsedTime, drawer);
        }
        public void Update(float elapsedTime, Menu menu)
        {
            atras.Update(elapsedTime, menu);
        }


    }
}
