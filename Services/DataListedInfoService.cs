using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services
{
    public class DataListedInfoService
    {
        private MyDbContext _context;

        public DataListedInfoService(string connectionString)
        {
            _context = new MyDbContext(connectionString);
        }
        public async Task CreateDataListedInfo(DataListedInfo data)
        {
            //var context = new MyDbContext(_connectionString);
            await _context.DataListedInfo.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task<DataListedInfo> GetDataListedInfoByDataBacklinkRowID(int id)
        {
            return await _context.DataListedInfo.FirstOrDefaultAsync(x => x.DataBacklinkRowID== id);
        }
        public async Task CreateDataListedInfo(IList<DataListedInfo> data)
        {
            //var context = new MyDbContext(_connectionString);
            await _context.DataListedInfo.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }
    }
}
