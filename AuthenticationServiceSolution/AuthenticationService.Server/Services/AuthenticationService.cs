using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using Scrypt;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Server.Services
{
    public class AuthenticationService : Authentication.AuthenticationBase
    {
        private readonly UsersDirectory _usersDirectory;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ScryptEncoder _encoder;

        public AuthenticationService(ILogger<AuthenticationService> logger, UsersDirectory usersDir)
        {
            _logger = logger;
            _encoder = new ScryptEncoder();
            _usersDirectory = usersDir;
        }

        public override Task<SignInReply> SignIn(SignInRequest request, ServerCallContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SignInReply reply = new SignInReply();

            var credentials = request.Credentials;

            if (_usersDirectory.IsAvailableUser(credentials.Username))
            {
                var passwordHash = _encoder.Encode(credentials.Password);
                _usersDirectory.AddUser(credentials.Username, passwordHash);
                reply.Success = true;
                reply.Message = $"User {credentials.Username} created successfully!";
                reply.GeneratedHash = passwordHash;
            }
            else
            {
                reply.Success = false;
                reply.Message = $"User {credentials.Username} is already taken, please choose another!";
            }

            sw.Stop();
            reply.ElapsedTime = sw.ElapsedMilliseconds;
            return Task.FromResult(reply);
        }

        public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            var user = _usersDirectory.GetUser(request.Credentials.Username);

            LoginReply reply = new LoginReply();

            if (user != null)
            {
                reply.Success = _encoder.Compare(request.Credentials.Password, user.Value.Value);

                if (reply.Success)
                {
                    DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);

                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserID", request.Credentials.Username)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(user.Value.Value));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken("https://localhost:7276",
                                                     "EVERYONE",
                                                     claims,
                                                     expires: expirationDate,
                                                     signingCredentials: signIn);

                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                    reply.AuthorizationToken = jwt;
                    reply.ExpirationDate = expirationDate.ToString();
                }
            }

            return Task.FromResult(reply);
        }

        public override Task<GetRegisteredUsersReply> GetRegisteredUsers(Empty request, ServerCallContext context)
        {
            GetRegisteredUsersReply reply = new GetRegisteredUsersReply();

            reply.Users.AddRange(_usersDirectory.GetUsers());

            return Task.FromResult(reply);
        }
    }
}
