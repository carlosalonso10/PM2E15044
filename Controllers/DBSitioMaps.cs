using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using PM2E15044.Models;

namespace PM2E15044.Controllers
{
    public class DBSitioMaps
    {
        SQLiteAsyncConnection _connection;

        public DBSitioMaps() { }

        public async Task Init()
        {
            try
            {
                if (_connection is null)
                {
                    SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;
                    if (string.IsNullOrEmpty(FileSystem.AppDataDirectory))
                    {
                        return;
                    }

                    _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBSitioMaps"), extensiones);
                    var creacion = await _connection.CreateTableAsync<Models.sitioMaps>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Init(): {ex.Message}");
            }
        }

        //Crear metodos crud para la clase de sitio
        public async Task<int> InsertMapaSitio(sitioMaps sitiomaps)
        {
            await Init();
            if (sitiomaps.Id == 0)
            {
                return await _connection.InsertAsync(sitiomaps);
            }
            else
            {
                return await _connection.UpdateAsync(sitiomaps);
            }
        }

        //Update

        //Acutalizaciones
        //funciona
        public async Task<int> updateSitios(sitioMaps sitiomaps)
        {
            await Init();
            return await _connection.UpdateAsync(sitiomaps);
        }

        //Read
        public async Task<List<Models.sitioMaps>> getListSitio()
        {
            await Init();
            return await _connection.Table<sitioMaps>().ToListAsync();
        }

        //Read Element
        public async Task<Models.sitioMaps> getsitioMaps(int pid)
        {
            await Init();
            return await _connection.Table<sitioMaps>().Where(i => i.Id == pid).FirstOrDefaultAsync();
        }

        //Delete
        public async Task<int> deleteSitio(int sitioMapsID)
        {
            await Init();
            var sitioMapsToDelete = await getsitioMaps(sitioMapsID);

            if (sitioMapsToDelete != null)
            {
                return await _connection.DeleteAsync(sitioMapsToDelete);
            }

            return 0;
        }

    }
}
