using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group.Model.MenuesImplementados
{
    class MenuControles : TipoMenu
    {
        private Simbolo accion;
        private Simbolo tecla;
        private Simbolo derecha;
        private Simbolo izquierda;
        private Simbolo afterBurn;
        private Simbolo disparar;
        private Simbolo d;
        private Simbolo a;
        private Simbolo left_click;
        private Simbolo shift;
        private Boton atras;

        public int W = D3DDevice.Instance.Width;
        public int H = D3DDevice.Instance.Height;
        List<Simbolo> simbolos;

        public MenuControles(string MediaDir, TgcD3dInput input)
        {
            accion = new Simbolo(MediaDir, "accion.png", input);
            tecla = new Simbolo(MediaDir, "tecla.png", input);
            derecha = new Simbolo(MediaDir, "derecha.png", input);
            izquierda = new Simbolo(MediaDir, "izquierda.png", input);
            afterBurn = new Simbolo(MediaDir, "afterburn.png", input);
            disparar = new Simbolo(MediaDir, "disparar.png", input);
            d = new Simbolo(MediaDir, "d.png", input);
            a = new Simbolo(MediaDir, "a.png", input);
            left_click = new Simbolo(MediaDir, "left_click.png", input);
            shift = new Simbolo(MediaDir, "shift.png", input);
            atras = new Boton(MediaDir, "atras.png", input, "seleccion_atras.png", new AccionAtras());

            accion.Scaling = new TGCVector2(0.6f, 0.6f);
            tecla.Scaling = new TGCVector2(0.6f, 0.6f);
            derecha.Scaling = new TGCVector2(0.4f, 0.5f);
            izquierda.Scaling = new TGCVector2(0.4f, 0.5f);
            afterBurn.Scaling = new TGCVector2(0.4f, 0.5f);
            d.Scaling = new TGCVector2(0.4f, 0.5f);
            a.Scaling = new TGCVector2(0.4f, 0.5f);
            left_click.Scaling = new TGCVector2(0.4f, 0.5f);
            atras.Scaling = new TGCVector2(0.4f, 0.4f);
            disparar.Scaling = new TGCVector2(0.4f, 0.4f);
            shift.Scaling = new TGCVector2(0.4f, 0.4f);

            accion.Position = new TGCVector2(75, H / 5.5f);
            derecha.Position = new TGCVector2(100, H / 3.25f);
            izquierda.Position = new TGCVector2(100, H / 2.25f);
            afterBurn.Position = new TGCVector2(100, H / 1.75f);
            disparar.Position = new TGCVector2(100, H / 1.45f);
            atras.Position = new TGCVector2(75, H / 1.15f);
            d.Position = new TGCVector2(625, H / 3.25f);
            a.Position = new TGCVector2(625, H / 2.25f);
            left_click.Position = new TGCVector2(625, H / 1.45f);
            shift.Position = new TGCVector2(625, H / 1.75f);
            tecla.Position = new TGCVector2(570, H / 5.5f);

            simbolos = new List<Simbolo>();
            simbolos.Add(accion);
            simbolos.Add(tecla);
            simbolos.Add(d);
            simbolos.Add(derecha);
            simbolos.Add(izquierda);
            simbolos.Add(a);
            simbolos.Add(disparar);
            simbolos.Add(left_click);
            simbolos.Add(afterBurn);
            simbolos.Add(shift);
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
