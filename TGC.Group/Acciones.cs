using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.MenuesImplementados;

namespace TGC.Group
{
    interface Accion
    {
        void realizar(Menu menu);
    }


    class AccionEmpezar : Accion
    {
        public void realizar(Menu menu)
        {
            menu.estaEnMenu = false;
        }
    }
    class AccionSalir : Accion
    {
        public void realizar(Menu menu)
        {
            Environment.Exit(-1);
        }
    }
    class AccionOpciones : Accion
    {
        public void realizar(Menu menu)
        {
            menu.setMenuOpciones();
        }
    }
    class AccionAudio : Accion
    {
        public void realizar(Menu menu)
        {
            menu.setMenuAudio();
        }
    }
    class AccionControles : Accion
    {
        public void realizar(Menu menu)
        {
            menu.setMenuControles();
        }
    }
    class AccionInstrucciones : Accion
    {
        public void realizar(Menu menu)
        {
            menu.setMenuInstrucciones();
        }
    }
    class AccionVolver : Accion
    {
        public void realizar(Menu menu)
        {
            menu.setMenuPrincipal();
        }
    }
    class AccionAtras : Accion
    {
        public void realizar(Menu menu)
        {
            menu.setMenuOpciones();
        }
    }
    class AccionSonido : Accion
    {
        TipoSonido tipoSonido;
        string MediaDir;
        public AccionSonido(TipoSonido tipo,string MediaDir)
        {
            this.tipoSonido = tipo;
            this.MediaDir = MediaDir;
        }
        public void realizar(Menu menu)
        {
            if (tipoSonido.On(menu))
            {
                tipoSonido.cambiarTextura(menu.menuAudio,MediaDir, "off.png");
            }
            else
            {
                tipoSonido.cambiarTextura(menu.menuAudio, MediaDir, "on.png");
            }
            tipoSonido.Modificar(menu);
        }
 
    }
    interface TipoSonido
    {
         Boolean On(Menu menu);
         void cambiarTextura(MenuAudio menu,string MediaDir, string texture);
         void Modificar(Menu menu);
    }
    class AccionSonidoAmbiente : TipoSonido
    {
        public Boolean On(Menu menu)
        {
            return menu.playSonidoAmbiente;
        }
        public void cambiarTextura(MenuAudio menu,string MediaDir,string texture)
        {
            menu.onSonidoAmbiente.cambiarTextura(MediaDir, texture);
        }
        public void Modificar(Menu menu)
        {
            menu.playSonidoAmbiente = !menu.playSonidoAmbiente;
        }
    }
    class AccionSonidoMenu : TipoSonido
    {
        public Boolean On(Menu menu)
        {
            return menu.playSonidoMenu;
        }
        public void cambiarTextura(MenuAudio menu, string MediaDir, string texture)
        {
            menu.onSonidoMenu.cambiarTextura(MediaDir, texture);
        }
        public void Modificar(Menu menu)
        {
            menu.playSonidoMenu = !menu.playSonidoMenu;
        }

    }
    class AccionSonidoDisparos : TipoSonido
    {
        public Boolean On(Menu menu)
        {
            return menu.playSonidoDisparos;
        }
        public void cambiarTextura(MenuAudio menu, string MediaDir, string texture)
        {
            menu.onSonidoDisparos.cambiarTextura(MediaDir, texture);
        }
        public void Modificar(Menu menu)
        {
            menu.playSonidoDisparos = !menu.playSonidoDisparos;
        }
    }
}
