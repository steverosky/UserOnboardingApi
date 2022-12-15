namespace UserOnboardingApi.Model
{
    
        public class loginRequest
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public int expiryTime { get; set; }
            public string token { get; set; }
            public string role { get; set; }
        }

    
}
