using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.MenuesImplementados;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group
{
    class Menu
    {
        public Boolean estaEnMenu { get; set; }
        public Boolean playSonidoAmbiente { get; set; }
        public Boolean playSonidoMenu { get; set; }
        public Boolean playSonidoDisparos { get; set; }

        public MenuAudio menuAudio { get; set; }
        private MenuInstrucciones menuInstrucciones;
        private MenuControles menuControles;
        private MenuOpciones menuOpciones;
        private MenuPrincipal MenuPrincipal;

        private TipoMenu menuActual;

        public Menu(string MediaDir, TgcD3dInput input)
        {
            menuAudio = new MenuAudio(MediaDir, input);
            menuInstrucciones = new MenuInstrucciones(MediaDir, input);
            menuControles = new MenuControles(MediaDir, input);
            menuOpciones = new MenuOpciones(MediaDir, input);
            MenuPrincipal = new MenuPrincipal(MediaDir, input);

            estaEnMenu = true;
            this.setMenuPrincipal();

            this.playSonidoAmbiente = true;
            this.playSonidoDisparos = true;
            this.playSonidoMenu = true;
        }

        public void Update(float elapsedTime)
        {
            menuActual.Update(elapsedTime, this);

        }

        public MenuPrincipal GetMenuPrincipal()
        {
            return this.MenuPrincipal;
        }

        public void Render(float elapsedTime, Drawer2D drawer)
        {
            menuActual.Render(elapsedTime, drawer);
        }
        public void setMenuPrincipal()
        {
            menuActual = MenuPrincipal;
        }
        public void setMenuAudio()
        {
            menuActual = menuAudio;
        }
        public void setMenuInstrucciones()
        {
            menuActual = menuInstrucciones;
        }
        public void setMenuOpciones()
        {
            menuActual = menuOpciones;
        }
        public void setMenuControles()
        {
            menuActual = menuControles;
        }
    }
}