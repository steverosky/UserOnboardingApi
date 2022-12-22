using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using UserOnboardingApi.EFCore;


namespace UserOnboardingApi.Model
{
    public class DbHelper : ControllerBase
    {
        private EF_DataContext _context;
        private IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        public DbHelper(EF_DataContext context, IConfiguration config, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _configuration = config;
            _signInManager = signInManager;
        }

        // Authenticate  user details email & password for login
        public async Task<User> GetUser(string Email, string Password)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(e => e.Email == Email);
            var decoded = DecodeFrom64(user.Password);
            if (user is not null && decoded == Password)
            {
                //return new userModel() {
                //    Token = CreateTokenAsync().ToString()
                return user;
            }

            return null;
        }

        //public async Task<User> GetUser(string email, string password)
        //{
        //    Func<User, bool> isValidUser = (User user) => {
        //        return user.Email == email &&
        //           DecodeFrom64(user.Password) == password;
        //    };
        //    return  _context.Set<User>().ToList().Where(e => isValidUser(e)).FirstOrDefault();
        //}




        //this function Convert to Encord your Password
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //this function Convert to Decord your Password
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }




        //generates a random password from 0-1 and a-z lower and uppercase using the random class
        public static string GetRandomPassword(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }

        public static string DefaultPass()
        {
            int length = 10;

            string password = GetRandomPassword(length);

            return password;
        }




        //get all users
        public List<userModel> GetUsers()
        {
            List<userModel> response = new List<userModel>();
            var dataList = _context.Users.ToList();
            dataList.ForEach(row => response.Add(new userModel()
            {
                Id = row.Id,
                Name = row.Name,
                Email = row.Email,
                Password = DecodeFrom64(row.Password),
                Role = row.Role

            }));
            

            return response.OrderBy(e => e.Id).ToList();
        }

        //Get by Id
        public userModel GetUserById(int id)
        {
            userModel response = new userModel();
            var row = _context.Users.Where(d => d.Id == (id)).FirstOrDefault();
            if (row != null)
                return new userModel()
                {
                    Id = row.Id,
                    Name = row.Name,
                    Email = row.Email,
                    Password = DecodeFrom64(row.Password),
                    Role = row.Role

                };
            else return null;

        }


        public void AssignRoles(userModel userModel)
        {
            User response = new User();
            var EmailList = _context.Users.Select(dbTable => dbTable.Email).ToList();

            if (EmailList.Contains(userModel.Email))
            {
                var dbTable = _context.Users.Where(d => d.Email == userModel.Email).FirstOrDefault();
                if (dbTable != null)
                {
                    if (userModel.Role == "admin" || userModel.Role == "user")
                    {
                        dbTable.Role = userModel.Role;
                        _context.Users.Update(dbTable);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Invalid Role");
                    }
                }
            }
        }


        //post or add user
        public void AddUser(userModel usermodel)
        {
            User dbTable = new User();
            {
                // check if user Email exists before adding and auto-increment the id
                //var emailExists = await _context.FindByEmailAsync(usermodel.Email);
                var EmailList = _context.Users.Select(dbTable => dbTable.Email).ToList();
               
                if (EmailList.Contains(usermodel.Email))
                {
                    throw new Exception("email already exists");
                }
                else
                {
                    var idList = _context.Users.Select(x => x.Id).ToList();
                    var maxId = idList.Max();
                    usermodel.Id = maxId + 1;
                    string Password = DefaultPass();
                    usermodel.Password = EncodePasswordToBase64(Password);

                    {
                        dbTable.Id = usermodel.Id;
                        dbTable.Name = usermodel.Name;
                        dbTable.Email = usermodel.Email;
                        dbTable.Password = usermodel.Password;
                        dbTable.Status = "InActive";
                        dbTable.Role = "user";
                        _context.Users.Add(dbTable);
                        _context.SaveChanges();


                        //read html file from current directory and pass it to the email body
                        string FilePath = Directory.GetCurrentDirectory() + "\\index2.html";
                        StreamReader str = new StreamReader(FilePath);
                        string MailText = str.ReadToEnd();
                        str.Close();
                        MailText = MailText.Replace("[username]", usermodel.Name).Replace("[email]", usermodel.Email).Replace("[Password]", Password).Replace("[logo]", "cid:image1");

                        //send mail
                        sendMail(MailText, usermodel.Email);
                    }
                }
            }
        }


        //put or update user details
        public void SaveUser(userModel userModel)
        {
            User dbTable = new User();
            dbTable = _context.Users.Where(d => d.Id == userModel.Id).FirstOrDefault();
            if (dbTable != null)
            {
                userModel.Password = EncodePasswordToBase64(userModel.Password);
                dbTable.Name = userModel.Name;
                dbTable.Email = userModel.Email;
                dbTable.Password = userModel.Password;
                _context.Users.Update(dbTable);
                _context.SaveChanges();
            }
        }

        //change password
        public void ChangePass(userModel userModel)
        {
            User dbTable = new User();
            var EmailList = _context.Users.Select(dbTable => dbTable.Email).ToList();
            if (EmailList.Contains(userModel.Email))
            {
                dbTable = _context.Users.Where(d => d.Email == userModel.Email).FirstOrDefault();
                if (dbTable != null)
                {
                    userModel.Password = EncodePasswordToBase64(userModel.Password);
                    dbTable.Password = userModel.Password;
                    dbTable.Status = "Active";
                    _context.Users.Update(dbTable);
                    _context.SaveChanges();
                }
            }
            else
            {
                throw new Exception("Email Does not exists");
            }
        }

        //Delete
        public void DeleteUser(int id)
        {
            var user = _context.Users.Where(d => d.Id == id).FirstOrDefault();
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public void sendMail(string msg, string recipientEmail)
        {
            try
            {
                var fromAddress = new MailAddress("donotreplyme1234@gmail.com");
                var toAddress = new MailAddress(recipientEmail);
                string fromPassword = "okcnmpqkrwkjuexg";
                string subject = "Registered for Heaven";
                string body = msg;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 50000

                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true

                })

                {
                    try
                    {
                        // Open the image file from directory and attach it to the email
                        var imagePath = "transparent.png";
                        var image = new Attachment(imagePath);
                        message.Attachments.Add(image);

                        // Set the Content-ID of the image
                        image.ContentId = "image1";

                        smtp.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        //_logger.LogError(ex.FailedRecipient, this);
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable ||
                                status == SmtpStatusCode.TransactionFailed ||
                                status == SmtpStatusCode.ServiceNotAvailable ||
                                status == SmtpStatusCode.ServiceClosingTransmissionChannel ||
                                status == SmtpStatusCode.GeneralFailure)
                            {
                                Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(message);
                            }
                            else
                            {
                                //_logger.LogError("Failed to deliver message to {0}",
                                //ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
                //_logger.LogError(ex.Message, this);
            }

        }

        public async Task<object> CreateTokenAsync(string email, string password)
        {
            var token1 = "";
            double expireTime = 0;
            if (email != null && password != null)
            {
                var user = await GetUser(email, password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["JwtConfig:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Email", user.Email.ToString()),
                        new Claim("Password", user.Password.ToString()),
                        new Claim("Role", user.Role.ToString()),


                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["JwtConfig:Issuer"],
                        _configuration["JwtConfig:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(120),
                        signingCredentials: signIn);


                    token1 = new JwtSecurityTokenHandler().WriteToken(token);
                    expireTime = token.ValidTo.Subtract(DateTime.UtcNow).TotalSeconds;
                    double expiryTimeInSeconds = Math.Ceiling(expireTime);


                    return new userModel()
                    {
                        Email = email,
                        ExpiryTime = expiryTimeInSeconds,
                        Token = token1

                    };
                }
                else
                {
                    throw new Exception("Invalid credentials");
                }

            }
            else
            {
                throw new Exception("Invalid credentials");
            }


        }

        //public string DecodeJWT(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token);
        //    var tokenS = handler.ReadToken(token) as JwtSecurityToken;
        //    var email = tokenS.Claims.First(claim => claim.Type == "Email").Value;
        //    var password = tokenS.Claims.First(claim => claim.Type == "Password").Value;
        //    var id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
        //    var role = tokenS.Claims.First(claim => claim.Type == "Role").Value;
        //    var expiryTime = tokenS.ValidTo.Subtract(DateTime.UtcNow).TotalSeconds;
        //    double expiryTimeInSeconds = Math.Ceiling(expiryTime);

        //    return email + " " + password + " " + id + " " + role + " " + expiryTimeInSeconds;
        //}

    }



}


// var message = new Message(new string[] { dbTable.Email }, "Welcome to User Onboarding", "Hello " + dbTable.Name + ", Your account has been created successfully.\n Your password is " + dbTable.Password + "\n Please change your password after login.");
//EmailSender.SendEmail(message);


//_context.SaveChanges();

// if (_context.Users.Contains(usermodel.Id))

// else
//{


//put


