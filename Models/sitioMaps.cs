using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PM2E15044.Models
{
    [SQLite.Table("SitioMaps")]
    public class sitioMaps
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public string? descripcion { get; set; }
        public string? imagen { get; set; }
    }


}
