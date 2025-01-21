using AutoMapper;
using ObserverScheduler.Abstractions;
using ObserverScheduler.Entities;
using ObserverScheduler.Models;

namespace ObserverScheduler.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserViewModel> GetUserById(Guid userId)
        => _mapper.Map<UserViewModel>(await _userRepository.GetUserById(userId));
    

    public async Task Create(UserCreateModel user)
        => await _userRepository.Create(_mapper.Map<User>(user));

    public async Task Delete(Guid userId)
        => await _userRepository.Delete(userId);

    public async Task<UserViewModel> GetUserByApiKey(string apiKey)
        => _mapper.Map<UserViewModel>(await _userRepository.GetUserByApiKey(apiKey));
}