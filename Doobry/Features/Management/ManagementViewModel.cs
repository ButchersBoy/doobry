using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Features.Management
{
    public class ManagementViewModel : INamed
    {
        public ManagementViewModel()
        {
            Name = "DB Manager";
        }

        public string Name { get; }
    }
}
