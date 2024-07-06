using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entity;
        public UserRepository(IConfiguration config)
        {
            _entity = new DataContextEF(config);
        }
        public bool SaveChanges()
        {
            return _entity.SaveChanges() > 0;
        }
        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entity.Add(entityToAdd);
            }
        }
        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entity.Remove(entityToAdd);
            }
        }
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entity.Users.ToList<User>();
            return users;
        }

        public IEnumerable<UserJobInfo> GetUserJobs()
        {
            IEnumerable<UserJobInfo> jobs = _entity.UserJobInfo.ToList<UserJobInfo>();
            return jobs;
        }
        public IEnumerable<UserSalary> GetUserSalaries()
        {
            IEnumerable<UserSalary> salary = _entity.UserSalary.ToList<UserSalary>();
            return salary;
        }
        public User GetSingleUser(int userId)
        {
            User? user = _entity.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();
            if (user != null)
            {
                return user;
            }
            throw new Exception("Failed to Get User");
        }
        public UserJobInfo GetSingleUserJob(int userId)
        {
            UserJobInfo? job = _entity.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault<UserJobInfo>();
            if (job != null)
            {
                return job;
            }
            throw new Exception("Failed to Get job");
        }

        public UserSalary GetSingleUserSalary(int userId)
        {
            UserSalary? salary = _entity.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault<UserSalary>();
            if (salary != null)
            {
                return salary;
            }
            throw new Exception("Failed to Get User");
        }
    }
}