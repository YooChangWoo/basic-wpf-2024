using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex06_caliburn_basic.ViewModels
{
    internal class MainViewModel : Conductor<object>
    {
        public string Greeting { get { return "헬로, 칼리번!!"; } }
    }
}
