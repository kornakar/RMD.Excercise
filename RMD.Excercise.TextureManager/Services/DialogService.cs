using RMD.Excercise.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMD.Excercise.TextureManager.Services
{
    [Export(typeof(IDialogService))]
    public class DialogService : IDialogService
    {
        public MessageBoxResult ShowMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }
    }
}
