using Microsoft.AspNetCore.DataProtection;
using Weavers.Core.Constants;

namespace Weavers.Core.Service {

  public interface ICryptoService {
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
  }
  public class CryptoService : ICryptoService {
    private readonly IDataProtector _protector;

    public CryptoService(IDataProtectionProvider provider) {
      // Creates a purpose-specific protector
      _protector = provider.CreateProtector(Cx.CredentialProtectorName);
    }

    public string Encrypt(string plaintext) {
      return _protector.Protect(plaintext);
    }

    public string Decrypt(string ciphertext) {
      return _protector.Unprotect(ciphertext);
    }
  }

}
