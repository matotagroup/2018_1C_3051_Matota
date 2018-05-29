using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group.Model.MenuesImplementados
{
    interface TipoMenu
    {
        void Update(float elapsedTime, Menu menu);
        void Render(float elapsedTime, Drawer2D drawer);

    }
}
