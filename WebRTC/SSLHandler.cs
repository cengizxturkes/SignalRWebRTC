using System.Net;

public class SSLHandler
{
    public static void DisableCertificateValidation()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, sslPolicyErrors) => true;
    }
}