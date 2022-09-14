using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication16.ProctectorDotNetCore
{
    public class MyClass
    {
        // The IDataProtectionProvider is registered by default in ASP.NET Core
        readonly IDataProtectionProvider _rootProvider;
        public MyClass(IDataProtectionProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        public void RunSample()
        {
            // Create a child key using the purpose string
            string purpose = "Contoso.MyClass.v1";
            IDataProtector protector = provider.CreateProtector(purpose);

            // Get the data to protect
            Console.Write("Enter input: ");
            string input = Console.ReadLine();
            // Enter input: Hello world!

            // protect the payload
            string protectedPayload = _protector.Protect(input);
            Console.WriteLine($"Protect returned: {protectedPayload}");
            //PRINTS: Protect returned: CfDJ8ICcgQwZZhlAlTZT...OdfH66i1PnGmpCR5e441xQ

            // unprotect the payload
            string unprotectedPayload = _protector.Unprotect(protectedPayload);
            Console.WriteLine($"Unprotect returned: {unprotectedPayload}");
            //PRINTS: Unprotect returned: Hello world
        }
    }
}