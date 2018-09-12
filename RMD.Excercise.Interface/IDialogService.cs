using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMD.Excercise.Interface
{
    public interface IDialogService
    {
        MessageBoxResult ShowMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon);
    }
}
