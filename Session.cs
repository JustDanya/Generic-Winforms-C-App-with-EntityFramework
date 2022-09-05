using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PM01
{
    class Session
    {
            static Users emp;

            public static bool authority = false;

            private Session(Users e)
            {
                emp = e;
            }

            public static void MakeSession(Users e)
            {
                new Session(e);
                authority = true;
            }

            public static Users GetSession()
            {
                return emp;
            }

            public static void DestroySession()
            {
                authority = false;
                emp = null;
            }

            public static void AccessDenied()
            {
                MessageBox.Show("У вас нет прав доступа", "У вас нет прав доступа", MessageBoxButtons.OK);
            }

    }
}
