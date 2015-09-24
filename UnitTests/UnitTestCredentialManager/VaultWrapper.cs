using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Security.Credentials;

namespace UnitTestCredentialManager
{
    class VaultWrapper 
    {
        public bool CredentialExists(string app, string userName)
        {            
            var vault = new PasswordVault();
            try
            {
                var credential = vault.FindAllByResource(app).FirstOrDefault(c => c.UserName == userName);
                return credential != null;
            }
            catch (Exception)
            {
                // If no credentials have been stored with the given name, an exception is thrown.
            }

            return false;
        }

        public string GetAccessCode(string app, string userName)
        {
            var credential = GetCredential(app, userName);
            if (credential != null)
            {
                return credential.Password;
            }

            return "";
        }

        public void AddAccessCode(string app, string userName, string accessToken)
        {
            var vault = new PasswordVault();

            vault.Add(new PasswordCredential(app, userName, accessToken));
        }

        public void RemoveAccessCode(string app, string userName)
        {
            var credential = GetCredential(app, userName);
            if (credential != null)
            {
                var vault = new PasswordVault();
                vault.Remove(credential);
            }
        }

        private PasswordCredential GetCredential(string app, string userName)
        {
            var vault = new PasswordVault();
            try
            {
                var credential = vault.FindAllByResource(app).FirstOrDefault(c => c.UserName == userName);
                if (credential != null)
                {
                    return vault.Retrieve(app, userName);
                }
            }
            catch (Exception)
            {
                // If no credentials have been stored with the given name, an exception is thrown.
            }

            return null;
        }
    }
}
