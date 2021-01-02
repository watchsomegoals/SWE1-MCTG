using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    public interface DatabaseManagerInterface
    {
        bool InsertUser(string username, string password);
    }
}
