using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace movieuniverse
{
    public interface IMoviePanelUpdater
    {
        void UpdateMoviePanels(MySqlDataReader reader, FlowLayoutPanel flowLayoutPanel, BookingManager bookingManager, string userLogin);
    }
}
