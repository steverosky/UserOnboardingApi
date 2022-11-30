//using MailKit.Net.Smtp;
using System.Net;
using System.Net.Mail;
using System.Text;
using UserOnboardingApi.EFCore;


namespace UserOnboardingApi.Model
{
    public class DbHelper
    {
        private EF_DataContext _context;
        public DbHelper(EF_DataContext context)
        {
            _context = context;
        }



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
                Password = DecodeFrom64(row.Password)
            }));

            return response;
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
                    Password = DecodeFrom64(row.Password)

                };
            else return null;



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
                        _context.Users.Add(dbTable);
                        _context.SaveChanges();
                    }
                    var msg = $"Welcome to User Onboarding,  Hello  {usermodel.Name}, Your account has been created successfully.\nYour password is: {Password}\n Please change your password after login.";
                    sendMail(msg, usermodel.Email);
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
                string subject = "Cyberteq I'm Testing Something";
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
                    Body = body
                })
                {
                    try
                    {
                        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

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

    }

}


// var message = new Message(new string[] { dbTable.Email }, "Welcome to User Onboarding", "Hello " + dbTable.Name + ", Your account has been created successfully.\n Your password is " + dbTable.Password + "\n Please change your password after login.");
//EmailSender.SendEmail(message);


//_context.SaveChanges();

// if (_context.Users.Contains(usermodel.Id))

// else
//{


//put


