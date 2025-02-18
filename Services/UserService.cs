namespace WebApi.Services;

using BCrypt.Net;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    User Register(RegisterRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
    }


    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Неправильное имя пользователя или пароль");

        // authentication successful so generate jwt token
        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, jwtToken);
    }
    
    public User Register(RegisterRequest model)
    {
        var newUser = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.Username,
            Role = model.Role,
            PasswordHash = BCrypt.HashPassword(model.Password)
        };

        if (_context.Users.FirstOrDefault(x => x.Username == newUser.Username) != null)
        {
            throw new AppException("Такой пользователь уже существует");
        }

        _context.Users.Add(newUser);
        _context.SaveChanges();

        return newUser;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(int id) 
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}