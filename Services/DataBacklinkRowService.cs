using Backlinks_LE.Models;
using Backlinks_LE.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backlinks_LE.Services
{
    public class DataBacklinkRowService
    {
        private MyDbContext _context;

        public DataBacklinkRowService(string connectionString)
        {
            _context = new MyDbContext(connectionString);
        }
        public async Task<IList<DataBacklinkRow>> GetDataBacklinkRowRangeAndStatus(int skip, int take, Status status)
        {
            return await _context.DataBacklinkRow.Skip(skip).Take(take).Where(x=>x.Status==status).ToListAsync();
        }
        public void Sort()
        {
            _context.Set<DataBacklinkRow>().OrderBy(item => item.Position);
        }
        public async Task<DataBacklinkRow> GetDataBacklinkRowByPosition(int pos)
        {
            return await _context.DataBacklinkRow.FirstOrDefaultAsync(x => x.Position == pos);
        }
        public async Task<DataBacklinkRow> GetDataBacklinkRowByID(int id)
        {
            return await _context.DataBacklinkRow.FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<DataBacklinkRow> GetFirstDataBacklinkRowByStatus(Status status)
        {
            DataBacklinkRow row = await _context.DataBacklinkRow.FirstOrDefaultAsync(x => x.Status == status);
            return row;
        }
        public DataBacklinkRow GetFirstDataBacklinkRowByStatusSync(Status status)
        {
            DataBacklinkRow row = _context.DataBacklinkRow.FirstOrDefault(x => x.Status == status);
            return row;
        }
        public async Task UpdateDataBacklinkRow(DataBacklinkRow data)
        {
            _context.Update(data);
            await _context.SaveChangesAsync();
        }
        public void UpdateDataBacklinkRowSync(DataBacklinkRow data)
        {
            _context.Update(data);
            _context.SaveChanges();
        }
        public async Task CreateDataBacklinkRow(DataBacklinkRow data)
        {
            
            await _context.DataBacklinkRow.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task CreateDataBacklinkRow(IList<DataBacklinkRow> data)
        {
            
            await _context.DataBacklinkRow.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }
    }
}
