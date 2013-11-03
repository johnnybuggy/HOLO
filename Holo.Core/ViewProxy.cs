using System;
using System.Collections.Generic;
using System.Text;

namespace Holo.Core
{
    internal sealed class ViewProxy : IView
    {
        private readonly IView View;

        public ViewProxy() : this(null)
        {
        }

        public ViewProxy(IView view)
        {
            View = view;
        }

        #region Implementation of IView

        public void ShowError(Exception e)
        {
            if (View != null)
            {
                View.ShowError(e);
            }
        }

        public void UpdateAddedCountLabel(int count)
        {
            if (View != null)
            {
                View.UpdateAddedCountLabel(count);
            }
        }

        public void DisplayItems()
        {
            if (View != null)
            {
                View.DisplayItems();
            }
        }

        #endregion
    }
}
