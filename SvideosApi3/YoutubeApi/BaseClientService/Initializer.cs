using Google.Apis.Auth.OAuth2;

namespace BaseClientService
{
    internal class Initializer
    {
        private int v;

        public Initializer()
        {
        }

        public Initializer(int v)
        {
            this.v = v;
        }

        public string ApiKey { get; set; }
        public string ApplicationName { get; set; }
        public UserCredential HttpClientInitializer { get; internal set; }
    }
}