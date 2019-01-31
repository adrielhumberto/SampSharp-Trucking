using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace GamemodeDatabase.Data
{
    public class TruckerCargoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static List<TruckerCargoModel> GetCargoList
        {
            get
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    return db.Query<TruckerCargoModel>(@"SELECT * FROM truckerjobcargo").ToList();
                }
            }
        }
    }
}