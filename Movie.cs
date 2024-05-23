using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movieuniverse
{
    public class Movie
    {
        public string title { get; set; }
        public string genre { get; set; }
        public string producer { get; set; }
        public string actors { get; set; }
        public DateTime dateFilm { get; set; }
        public DateTime timeFilm { get; set; }
        public decimal ticketCost { get; set; }
        public int cinemaHall { get; set; }
        public TimeSpan duration { get; set; }
        public bool subtitles { get; set; }
        public Image image { get; set; }
    }
}
