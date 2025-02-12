using AutoMapper;
using WashMachine.Business.Interfaces;
using WashMachine.Repositories.Entities;
using Microsoft.AspNetCore.Http;

namespace WashMachine.Business.Services
{
    public class ReportService : IBusiness
    {
        private readonly IMapper _mapper;
        private readonly WashMachineContext _dbContext;
        IHttpContextAccessor _httpContextAccessor;

        public ReportService(IMapper mapper, WashMachineContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
