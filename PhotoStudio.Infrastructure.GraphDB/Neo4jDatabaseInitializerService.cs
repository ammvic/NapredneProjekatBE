using Neo4jClient;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoStudio.Infrastructure.Neo4j
{
    public class Neo4jDatabaseInitializerService
    {
        private readonly IGraphClient _graphClient;

        public Neo4jDatabaseInitializerService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task InitializeDatabaseAsync()
        {
            // Provera da li je inicijalizacija već obavljena
            var result = await _graphClient.Cypher
                .Match("(n:InitializationStatus)")
                .Return(n => n.Count())
                .ResultsAsync;

            if (result.FirstOrDefault() > 0)
            {
                // Inicijalizacija je već obavljena
                return;
            }

            // Dodavanje početnih korisnika
            await _graphClient.Cypher
                .Create("(user1:User {Id: $id1, FirstName: $firstName1, LastName: $lastName1, Email: $email1, Password: $password1, PhoneNumber: $phoneNumber1, Address: $address1, Credits: $credits1, VerificationCode: $verificationCode1, IsVerified: $isVerified1})")
                .WithParam("id1", 1)
                .WithParam("firstName1", "John")
                .WithParam("lastName1", "Doe")
                .WithParam("email1", "john.doe@example.com")
                .WithParam("password1", "hashedpassword1") // Ovde treba biti hashirana lozinka
                .WithParam("phoneNumber1", "1234567890")
                .WithParam("address1", "123 Main St")
                .WithParam("credits1", 100)
                .WithParam("verificationCode1", "ABC123")
                .WithParam("isVerified1", true)
                .Create("(user2:User {Id: $id2, FirstName: $firstName2, LastName: $lastName2, Email: $email2, Password: $password2, PhoneNumber: $phoneNumber2, Address: $address2, Credits: $credits2, VerificationCode: $verificationCode2, IsVerified: $isVerified2})")
                .WithParam("id2", 2)
                .WithParam("firstName2", "Jane")
                .WithParam("lastName2", "Smith")
                .WithParam("email2", "jane.smith@example.com")
                .WithParam("password2", "hashedpassword2") // Ovde treba biti hashirana lozinka
                .WithParam("phoneNumber2", "0987654321")
                .WithParam("address2", "456 Oak St")
                .WithParam("credits2", 150)
                .WithParam("verificationCode2", "XYZ789")
                .WithParam("isVerified2", false)
                .ExecuteWithoutResultsAsync();

            // Dodavanje početnih zaposlenih
            await _graphClient.Cypher
                .Create("(employee1:Employee {Id: $employeeId1, JMBG: $jmbg1, FirstName: $firstName1, LastName: $lastName1, PasswordHash: $passwordHash1, Role: $role1})")
                .WithParam("employeeId1", 1)
                .WithParam("jmbg1", "1234567890123")
                .WithParam("firstName1", "Marko")
                .WithParam("lastName1", "Petrović")
                .WithParam("passwordHash1", "hashedpassword1") // Ovde treba biti hashirana lozinka
                .WithParam("role1", "Photographer")
                .Create("(employee2:Employee {Id: $employeeId2, JMBG: $jmbg2, FirstName: $firstName2, LastName: $lastName2, PasswordHash: $passwordHash2, Role: $role2})")
                .WithParam("employeeId2", 2)
                .WithParam("jmbg2", "9876543210987")
                .WithParam("firstName2", "Ana")
                .WithParam("lastName2", "Jovanović")
                .WithParam("passwordHash2", "hashedpassword2") // Ovde treba biti hashirana lozinka
                .WithParam("role2", "Administrator")
                .Create("(employee3:Employee {Id: $employeeId3, JMBG: $jmbg3, FirstName: $firstName3, LastName: $lastName3, PasswordHash: $passwordHash3, Role: $role3})")
                .WithParam("employeeId3", 3)
                .WithParam("jmbg3", "1231231231231")
                .WithParam("firstName3", "Miloš")
                .WithParam("lastName3", "Nikolić")
                .WithParam("passwordHash3", "hashedpassword3") // Ovde treba biti hashirana lozinka
                .WithParam("role3", "Designer")
                .ExecuteWithoutResultsAsync();

            // Dodavanje početnih placanja
            await _graphClient.Cypher
                .Match("(user1:User {Id: $userId1})")
                .Create("(payment1:Payment {Id: $paymentId1, Amount: $amount1, PaymentDate: $paymentDate1, Status: $status1})-[:PAID_BY]->(user1)")
                .WithParam("userId1", 1)
                .WithParam("paymentId1", 1)
                .WithParam("amount1", 150.00m)
                .WithParam("paymentDate1", DateTime.Now.AddDays(-2))
                .WithParam("status1", "Completed")
                .Create("(payment2:Payment {Id: $paymentId2, Amount: $amount2, PaymentDate: $paymentDate2, Status: $status2})-[:PAID_BY]->(user1)")
                .WithParam("paymentId2", 2)
                .WithParam("amount2", 250.00m)
                .WithParam("paymentDate2", DateTime.Now.AddDays(-5))
                .WithParam("status2", "Pending")
                .ExecuteWithoutResultsAsync();

            // Dodavanje početnih medija
            await _graphClient.Cypher
                .Match("(album1:Album {Id: $albumId1})")
                .Create("(media1:Media {Id: $mediaId1, FileName: $fileName1, FilePath: $filePath1, FileType: $fileType1, UploadDate: $uploadDate1})-[:PART_OF]->(album1)")
                .WithParam("albumId1", 1)
                .WithParam("mediaId1", 1)
                .WithParam("fileName1", "summer_wedding_1.jpg")
                .WithParam("filePath1", "/media/summer_wedding_1.jpg")
                .WithParam("fileType1", "image/jpeg")
                .WithParam("uploadDate1", DateTime.Now.AddMonths(-2))
                .Create("(media2:Media {Id: $mediaId2, FileName: $fileName2, FilePath: $filePath2, FileType: $fileType2, UploadDate: $uploadDate2})-[:PART_OF]->(album1)")
                .WithParam("mediaId2", 2)
                .WithParam("fileName2", "summer_wedding_2.jpg")
                .WithParam("filePath2", "/media/summer_wedding_2.jpg")
                .WithParam("fileType2", "image/jpeg")
                .WithParam("uploadDate2", DateTime.Now.AddMonths(-2))
                .Create("(media3:Media {Id: $mediaId3, FileName: $fileName3, FilePath: $filePath3, FileType: $fileType3, UploadDate: $uploadDate3})-[:PART_OF]->(album2)")
                .WithParam("albumId2", 2)
                .WithParam("mediaId3", 3)
                .WithParam("fileName3", "corporate_event_1.jpg")
                .WithParam("filePath3", "/media/corporate_event_1.jpg")
                .WithParam("fileType3", "image/jpeg")
                .WithParam("uploadDate3", DateTime.Now.AddMonths(-1))
                .ExecuteWithoutResultsAsync();

            // Dodavanje početnih albuma
            await _graphClient.Cypher
                .Create("(album1:Album {Id: $albumId1, Name: $name1, Code: $code1, IsPublic: $isPublic1, CreatedAt: $createdAt1})-[:CREATED_BY]->(employee1)")
                .WithParam("albumId1", 1)
                .WithParam("name1", "Summer Wedding")
                .WithParam("code1", "SUMMER2024")
                .WithParam("isPublic1", true)
                .WithParam("createdAt1", DateTime.Now.AddMonths(-2))
                .Create("(album2:Album {Id: $albumId2, Name: $name2, Code: $code2, IsPublic: $isPublic2, CreatedAt: $createdAt2})-[:CREATED_BY]->(employee2)")
                .WithParam("albumId2", 2)
                .WithParam("name2", "Corporate Event")
                .WithParam("code2", "CORP2024")
                .WithParam("isPublic2", false)
                .WithParam("createdAt2", DateTime.Now.AddMonths(-1))
                .ExecuteWithoutResultsAsync();

            // Oznaka da je inicijalizacija završena
            await _graphClient.Cypher
                .Create("(initStatus:InitializationStatus {Initialized: $initialized})")
                .WithParam("initialized", true)
                .ExecuteWithoutResultsAsync();
        }
    }
}
