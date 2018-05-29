using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Simbolo f;
        private Simbolo shift;
        private Boton atras;

        private TGCVector2 posicionAccion = new TGCVector2(100, 100);
        private TGCVector2 posicionTecla = new TGCVector2(800, 100);
        private TGCVector2 posicionDerecha = new TGCVector2(100, 300);
        private TGCVector2 posicionD = new TGCVector2(900, 300);
        private TGCVector2 posicionIzquierda = new TGCVector2(100, 400);
        private TGCVector2 posicionA = new TGCVector2(900, 400);
        private TGCVector2 posicionAfterBurn = new TGCVector2(100, 500);
        private TGCVector2 posicionShift = new TGCVector2(900, 500);
        private TGCVector2 posicionDisparar = new TGCVector2(100, 600);
        private TGCVector2 posicionF = new TGCVector2(900, 600);
        private TGCVector2 posicionAtras = new TGCVector2(100, 800);

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
            f = new Simbolo(MediaDir, "f.png", input);
            shift = new Simbolo(MediaDir, "shift.png", input);
            atras = new Boton(MediaDir, "atras.png", input, "seleccion_atras.png", new AccionAtras());

            accion.Position = posicionAccion;
            tecla.Position = posicionTecla;
            derecha.Position = posicionDerecha;
            izquierda.Position = posicionIzquierda;
            afterBurn.Position = posicionAfterBurn;
            disparar.Position = posicionDisparar;
            d.Position = posicionD;
            a.Position = posicionA;
            f.Position = posicionF;
            shift.Position = posicionShift;
            atras.Position = posicionAtras;

            simbolos = new List<Simbolo>();
            simbolos.Add(accion);
            simbolos.Add(tecla);
            simbolos.Add(d);
            simbolos.Add(derecha);
            simbolos.Add(izquierda);
            simbolos.Add(a);
            simbolos.Add(disparar);
            simbolos.Add(f);
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
