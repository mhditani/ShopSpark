using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IRepositories
{
    public interface ICustomerRepo
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> UpdateAsync(string id, ApplicationUser customer);
        Task<bool> DeleteAsync(string id);
    }
}
