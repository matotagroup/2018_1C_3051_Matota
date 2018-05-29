using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Input;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group.Model.MenuesImplementados
{
    class MenuInstrucciones : TipoMenu
    {
        private Simbolo parrafo1;
        private Simbolo parrafo2;
        private Simbolo parrafo3;
        private Boton atras;

        private TGCVector2 posicionParrafo1 = new TGCVector2(300, 300);
        private TGCVector2 posicionParrafo2 = new TGCVector2(300, 400);
        private TGCVector2 posicionParrafo3 = new TGCVector2(300, 500);
        private TGCVector2 posicionAtras = new TGCVector2(100, 800);

        private List<Simbolo> simbolos;
        public MenuInstrucciones(string MediaDir, TgcD3dInput input)
        {
            parrafo1 = new Simbolo(MediaDir, "parrafo1.png", input);
            parrafo2 = new Simbolo(MediaDir, "parrafo2.png", input);
            parrafo3 = new Simbolo(MediaDir, "parrafo3.png", input);
            atras = new Boton(MediaDir, "atras.png", input, "seleccion_atras.png", new AccionAtras());

            parrafo1.Position = posicionParrafo1;
            parrafo2.Position = posicionParrafo2;
            parrafo3.Position = posicionParrafo3;
            atras.Position = posicionAtras;

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
