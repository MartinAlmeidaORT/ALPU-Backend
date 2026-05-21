#:project ../ALPU-Backend/DataAccess/DataAccess.csproj
#:project ../ALPU-Backend/Domain/Domain.csproj
#:package BCrypt.Net-Next@4.0.3

using BCrypt.Net;

string[] passwords = ["admin123", "supervisor123", "contador123", "client123", "broadcaster123"];

foreach (var password in passwords)
{
    Console.WriteLine($"{password} => {BCrypt.Net.BCrypt.HashPassword(password)}");
}
