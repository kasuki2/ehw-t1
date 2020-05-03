using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace ehw_t1
{
    public static class External_1
    {
        public async static void Show(this String str)
        {
            var dialog = new MessageDialog(str);
            await dialog.ShowAsync();
        }

    }
}
