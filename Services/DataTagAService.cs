using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services
{
    public class DataTagAService
    {
        private MyDbContext _context;

        public DataTagAService(string connectionString)
        {
            _context = new MyDbContext(connectionString);
        }

        public async Task<DataTagA> GetDataTagAForDataBacklinkRow(int id)
        {
            //var context = new MyDbContext(_connectionString);
            return await _context.DataTagA.FirstOrDefaultAsync(x => x.DataBacklinkRowID == id);
        }
        public async Task CreateDataTagA(DataTagA data)
        {
            //var context = new MyDbContext(_connectionString);
            await _context.DataTagA.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task CreateDataTagA(IList<DataTagA> data)
        {
            //var context = new MyDbContext(_connectionString);
            await _context.DataTagA.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }
    }
}
