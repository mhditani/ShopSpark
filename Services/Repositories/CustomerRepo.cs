using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity.Models;
using Services.IRepositories;

namespace Services.Repositories
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerRepo(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            // Get all users who have the Customer role
            var customerUsers = new List<ApplicationUser>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    customerUsers.Add(user);
                }
            }

            return customerUsers;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            // Check if user is a customer
            return await _userManager.IsInRoleAsync(user, "Customer") ? user : null;
        }

        public async Task<ApplicationUser?> UpdateAsync(string id, ApplicationUser customer)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null) return null;

            // Update user properties
            existingUser.FirstName = customer.FirstName;
            existingUser.LastName = customer.LastName;
            existingUser.Email = customer.Email;
            existingUser.UserName = customer.Email; // Keep username in sync with email
            existingUser.PhoneNumber = customer.PhoneNumber;

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded ? existingUser : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}