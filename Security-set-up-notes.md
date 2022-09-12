# Set up
- Update all your packages
- Compile project
# MAKE SURE YOU PUT IN THE NUGET PACKAGES AT THE VERY START
# Startup.cs is Program.cs in .net6
- You might get an error saying: I don't know what UseCollation is
- ![image](https://user-images.githubusercontent.com/22464805/161402456-0a8efd1e-7103-455b-84a2-441ff786e110.png)

## App Security Set Up
 - New Class library App Security
 - Back end engineering of Employee table, or any other table that is needed
 - Ensure it's .net 6
 - Add Entities Folder
 - Add DAL folder
 - EF Power Tools
   - Ensure you're using .net core 6
   - press oaky
   - Just select Employees?
   - Context Name: AppSecurityDbContext
   - Name Space: AppSecurity
   - Should look like bellow: 
   - ![image](https://user-images.githubusercontent.com/22464805/161401974-3bc17171-ddd6-4a67-be33-801562ee4bfe.png)

## NuGet Packages to install
 - ![image](https://user-images.githubusercontent.com/22464805/161402002-dcb61bdb-6adb-410b-be73-0ab4bce82cac.png)
 - ![image](https://user-images.githubusercontent.com/22464805/161402008-57acba87-cc19-4cd3-bb15-67539f7b2592.png)
 - ![image](https://user-images.githubusercontent.com/22464805/161402021-f2a2a1f6-686b-4944-8658-5e1c2dd59ed2.png)
 - Update NuGet packages if need be

## Set up Model Classes
 - Put these classes in Models folder
 - Employee class
 - THESE MODELS ARE PUBLIC NOT INTERNAL or PRIVATE
   - IIdentifyEmployee as a Interface class
   - ![image](https://user-images.githubusercontent.com/22464805/161402097-3fb642de-3484-4309-b878-0bd259e97ed8.png)
 - staff member Class
   - ![image](https://user-images.githubusercontent.com/22464805/161402117-b8cbe44e-c8c3-4d5c-accd-2000ad1dbb38.png)

 - ![image](https://user-images.githubusercontent.com/22464805/161402074-629e0a13-1d9d-46b7-9721-58f164465897.png)


## Create class Security Service
 - Add new folder BLL if you haven't, and add class SecurityService in BLL
 -
 ```c#
public class SecurityService
{
    private readonly AppSecurityDbContext _context;

    internal SecurityService(AppSecurityDbContext context)
    {
        _context = context;
    }

    public List<IIdentifyEmployee> ListEmployees()
    {
        var people = from emp in _context.Employees
                        select new StaffMember
                        {
                            EmployeeId = emp.EmployeeId,
                            Email = $"{emp.FirstName}.{emp.LastName}@eBikes.edu.ca",
                            // NOTE: UserName as an email to match the default login page
                            UserName = $"{emp.FirstName}.{emp.LastName}@eBikes.edu.ca"
                            //UserName = $"{emp.FirstName}.{emp.LastName}" // Alternative
                        };
        return people.ToList<IIdentifyEmployee>();
    }

    public string GetEmployeeName(int employeeId)
    {
        string result = "";
        var found = _context.Employees.Find(employeeId);
        if (found != null)
            result = $"{found.FirstName} {found.LastName}";
        return result;
    }
}

```
- Add Additional/missing namespaces by automatically by intellisense 
- Add SecurityExtensions calss
- You can copy+paset from Extensions class
- Change dependency names to something more semantic
- Should look something like this: 
- ![image](https://user-images.githubusercontent.com/22464805/161402392-94db035f-00fe-4dd5-8fc7-19f0905de14e.png)
- 


## Add Security dependecy to the WebApp
 - ![image](https://user-images.githubusercontent.com/22464805/161402494-f92b8a7d-ea2d-4a8e-a851-41a273d46ade.png)


## Web Application Modifications
- Add/Change following classes
- ![image](https://user-images.githubusercontent.com/22464805/161402541-963ed088-6687-4dec-b521-1eba528d9a8b.png)
- ![image](https://user-images.githubusercontent.com/22464805/161402572-03b05927-a5ca-41a1-a665-a8b3ee00d6a0.png)
- Should ApplicationUser should look like this: 
- ![image](https://user-images.githubusercontent.com/22464805/161402600-6385a11e-1878-4bb1-9489-0cc59e595d06.png)
- ![image](https://user-images.githubusercontent.com/22464805/161402630-b26daa3c-7f17-449e-a4a1-7a419cc163ee.png)
- Change _LoginPartial.cshtml
- ![image](https://user-images.githubusercontent.com/22464805/161402674-dea12d73-3fd9-4953-bf03-5dbb1342ee4f.png)
- Change Program.cs: 
- ![image](https://user-images.githubusercontent.com/22464805/161402721-ca057043-2b8d-4514-a7f7-25ef91c41096.png)
- Replace the Last line _App.Run()_ of Program.cs with the following
```csharp
await ApplicationUserSeeding(app);
app.Run();

private static async Task ApplicationUserSeeding(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();
        var env = services.GetRequiredService<IWebHostEnvironment>();
        if (env is not null && env.IsDevelopment())
        {
            try
            {
                var configuration = services.GetRequiredService<IConfiguration>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                if (!userManager.Users.Any())
                {
                    var securityService = services.GetRequiredService<SecurityService>();
                    var users = securityService.ListEmployees();
                    string password = configuration.GetValue<string>("Setup:InitialPassword");
                    foreach (var person in users)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = person.UserName, 
                            Email = person.Email,
                            EmployeeId = person.EmployeeId,
                            EmailConfirmed = true
                        };
                        var result = await userManager.CreateAsync(user, password);
                        if (!result.Succeeded)
                        {
                            logger.LogInformation("User was not created");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error occurred seeing the website users");
            }
        }
    }
}
```
- Remove private access type from Program.cs
- ![image](https://user-images.githubusercontent.com/22464805/161402826-311db791-37ca-42ef-85ed-b2b8d139af2b.png)


- For User SEcrets make sure you only have the minimum brackets
- Remove any unused Connection string, making sure it's just defeatConnection string

## Double check 
- Nuget Pcakges
- Explorer Details
- ![image](https://user-images.githubusercontent.com/22464805/161403037-4f7e2f30-956f-41da-bfba-7c1d6d5c5284.png)
-  Make sure the to delete these in Exlplorer Details
-  ![image](https://user-images.githubusercontent.com/22464805/161403048-98b07954-272b-448c-80d3-282b5f1fbbd3.png)
-  For this step, you have to press "ok" multiple times to delete all the selected tables, as other tables can't be deleted before others.
- **Delete: All the Asp.net tables AND migration table**
-  ![image](https://user-images.githubusercontent.com/22464805/161403058-2b84d5a3-afc0-4fc1-abe6-228d81b4304b.png)
-  
## Go to Package Manager Console: 
![image](https://user-images.githubusercontent.com/22464805/161403109-e5bf72da-9488-4b5d-bdaa-9abd2420b07f.png)

## Enter this command: 
- ** Make sure that there is no conflict between NuGet Packages and they're all up to date **
- ![image](https://user-images.githubusercontent.com/22464805/161403125-42c3b04b-fb01-48dd-9e80-cb98110b072d.png)
- Migration automatically created some files under WebApp>Data>Migrations: ![image](https://user-images.githubusercontent.com/22464805/161403186-2f682e92-4318-4a5c-b888-cd2ea8629009.png)

## Run applicaiton and attempt as log in
- Running Application in debug mode you can run into problems
- Clean and rebuild entire solution+projects
- Try running without debugging Ctrl+f5
- Loggin in should bring up this page: 
- ![image](https://user-images.githubusercontent.com/22464805/161403250-a3f626c1-7634-4a19-be55-c90d06234ae5.png)
- Try pressing "apply Migrations"
- Check to see if Databse if migration tables are created
- Rebuild and run entire solution again
- Try loggin in with default user password with any
- Remember the email portion is importatn
- ![image](https://user-images.githubusercontent.com/22464805/161403304-4583b917-cd9e-4d10-acde-86cf1d0322ac.png)

## Requiring Authorization
- Replace builder.services.AddRazorPages() with something like this: 
![image](https://user-images.githubusercontent.com/22464805/161403364-3bea7c0c-9d9d-470a-be8b-b759b662647b.png)
- In the nav bar, if you are logged out, you should be redirected to a login page if you are not logged in for restricted pages: 
- for your own page it should look similaar this to add secuirty: 
- ![image](https://user-images.githubusercontent.com/22464805/161403439-02d6cdf4-0e54-48fd-ba84-b2bd825fe42c.png)
- Add this to your page constructor as a paramter: 
- ![image](https://user-images.githubusercontent.com/22464805/161403477-e12ad0e7-fe11-4916-b661-5a2acd85b4e1.png)
- in the constructor body: 
- ![image](https://user-images.githubusercontent.com/22464805/161403511-e28ad34a-f1a8-4171-9243-e96c0c6c4a76.png)

- Change your OnGet from being a void to public async Task 

![image](https://user-images.githubusercontent.com/22464805/161403534-01941b51-9337-41df-bef3-0611e8f9784d.png)
- Remember to identify employee that is logged in for each page to the user
