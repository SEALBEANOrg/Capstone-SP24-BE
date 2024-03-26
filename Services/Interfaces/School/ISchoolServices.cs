using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.School
{
    public interface ISchoolServices
    {
        Task<bool> AddNewSchool(SchoolForCreateViewModel schoolForCreateViewModel, Guid currentUserId);

        Task<IEnumerable<SchoolList>> GetAllSchool(string? search, int status);
        Task<SchoolViewModels> GetSchoolById(Guid schoolId);
        Task<IEnumerable<DropdownSchools>> GetDropdownSchools();

        Task<bool> UpdateSchool(SchoolForUpdateViewModel schoolForUpdateViewModel, Guid currentUserId);
        Task<bool> ChangeStatus(Guid schoolId, ChangeStatusViewModel changeStatusViewModel, Guid currentUserId);

        Task<bool> DeleteSchool(Guid schoolId);
    }
}
