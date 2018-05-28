using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            menu.menuPrincipal = false;
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
            menu.menuPrincipal = false;
            menu.menuOpciones = true;
        }
    }
    class AccionAudio : Accion
    {
        public void realizar(Menu menu)
        {
            menu.menuOpciones = false;
            menu.menuAudio = true;
        }
    }
    class AccionControles : Accion
    {
        public void realizar(Menu menu)
        {
            menu.menuOpciones = false;
            menu.menuControles = true;
        }
    }
    class AccionInstrucciones : Accion
    {
        public void realizar(Menu menu)
        {
            menu.menuOpciones = false;
            menu.menuInstrucciones = true;
        }
    }
    class AccionVolver : Accion
    {
        public void realizar(Menu menu)
        {
            menu.menuOpciones = false;
            menu.menuPrincipal = true;
        }
    }
    class AccionAtras : Accion
    {
        public void realizar(Menu menu)
        {
            menu.menuInstrucciones = false;
            menu.menuAudio = false;
            menu.menuControles = false;
            menu.menuOpciones = true;
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
                tipoSonido.cambiarTextura(menu,MediaDir, "off.png");
            }
            else
            {
                tipoSonido.cambiarTextura(menu, MediaDir, "on.png");
            }
            tipoSonido.Modificar(menu);
        }
 
    }
    interface TipoSonido
    {
         Boolean On(Menu menu);
         void cambiarTextura(Menu menu,string MediaDir, string texture);
         void Modificar(Menu menu);
    }
    class AccionSonidoAmbiente : TipoSonido
    {
        public Boolean On(Menu menu)
        {
            return menu.playSonidoAmbiente;
        }
        public void cambiarTextura(Menu menu,string MediaDir,string texture)
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
        public void cambiarTextura(Menu menu, string MediaDir, string texture)
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
        public void cambiarTextura(Menu menu, string MediaDir, string texture)
        {
            menu.onSonidoDisparos.cambiarTextura(MediaDir, texture);
        }
        public void Modificar(Menu menu)
        {
            menu.playSonidoDisparos = !menu.playSonidoDisparos;
        }
    }
}
