using System;
using System.Windows.Forms;

namespace Holo.UI.Exceptions
{
    public class ExitApplicationException : Exception
    {
        public ExitApplicationException(string message)
            : base(message)
        {
            // TODO: Needs refactoring. Get rid of this.
            Application.Exit();
        }
    }
}
