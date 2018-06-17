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
    class MenuPrincipal : TipoMenu
    {
        private Simbolo tituloPrincipal;
        private Boton empezar;
        private Boton opciones;
        private Boton salir;

        private TGCVector2 posicionTitulo = new TGCVector2(750, 50);
        //private TGCVector2 posicionEmpezar = new TGCVector2(100, 400);
        private TGCVector2 posicionOpciones = new TGCVector2(100, 600);
        private TGCVector2 posicionSalir = new TGCVector2(100, 800);

        private List<Simbolo> simbolos;
        private List<Boton> botonesMenuPrincipal;

        public int W = D3DDevice.Instance.Width;
        public int H = D3DDevice.Instance.Height;

        public MenuPrincipal(string MediaDir, TgcD3dInput input)
        {
            tituloPrincipal = new Simbolo(MediaDir, "titulo.png", input);
            empezar = new Boton(MediaDir, "empezar.png", input, "seleccion_empezar.png", new AccionEmpezar());
            opciones = new Boton(MediaDir, "opciones.png", input, "seleccion_opciones.png", new AccionOpciones());
            salir = new Boton(MediaDir, "salir.png", input, "seleccion_salir.png", new AccionSalir());

            tituloPrincipal.Position = posicionTitulo;
            empezar.Position = new TGCVector2(100, H / 4);
            opciones.Position = new TGCVector2(100, H / 2.25f);
            salir.Position = new TGCVector2(100, H / 1.5f);
            tituloPrincipal.Scaling = new TGCVector2(1f, 1f);
            empezar.Scaling = new TGCVector2(0.6f, 0.6f);
            opciones.Scaling = new TGCVector2(0.6f, 0.6f);
            salir.Scaling = new TGCVector2(0.6f, 0.6f);

            tituloPrincipal.Scaling = new TGCVector2(0.8f,0.8f);
            this.simbolos = new List<Simbolo>();
            simbolos.Add(tituloPrincipal);

            this.botonesMenuPrincipal = new List<Boton>();
            botonesMenuPrincipal.Add(empezar);
            botonesMenuPrincipal.Add(opciones);
            botonesMenuPrincipal.Add(salir);
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            simbolos.ForEach(simbolo => simbolo.Render(elapsedTime, drawer));
            botonesMenuPrincipal.ForEach(boton => boton.Render(elapsedTime, drawer));
        }
        public void Update(float elapsedTime, Menu menu)
        {
            botonesMenuPrincipal.ForEach(boton => boton.Update(elapsedTime, menu));
        }
    }
}
