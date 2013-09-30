using System;
using System.Windows.Forms;

namespace HoloUI
{
    public class ExitApplicationException : Exception
    {
        public ExitApplicationException(string message)
            : base(message)
        {
            Application.Exit();
        }
    }
}
