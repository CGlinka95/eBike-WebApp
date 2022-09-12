# Deliverable 2 - **Project Setup and Security**

There are three stages to accomplishing the setup of your solution.

1. **Project Setup**
1. **Security Setup**
1. **Scenario Setup**

The first two stages are to be completed by different team members. **Use GitHub issues** to delegate specific tasks for those stages among your team members; divide up the tasks as equitably as possible.

For the third stage, each team member must set up a class library for their selected scenario.

**Each** member **must** demonstrate their participation in the project setup by making small, meaningful commits. When performing commits of your code, be sure to reference the issue number in your commit so that your work can be easily distinguished by your instructor.

## Stage 1 - Project Setup

Using the techniques and practices demonstrated in class, set up the repository project files by generating the following items.

- [ ] **Repository Documentation**
  - Ensure the **`ReadMe.md`** file at the root of your repository has the following:
    - Team Name for the project
    - Group Logo
    - Team member names mapped to each person's chosen subsystem
- [ ] **Web Application Project**
  - *This should be the first project in the solution, so that it opens as the default startup project.*
  - Use **individual accounts** for the authentication when setting up the project
    - *Customization for the website's security will be addressed in **Stage 2**.*
  - Use a class-less styling system (such as [Holiday-CSS](https://holidaycss.js.org/)) for the look & feel
  - Home page for the site must include the following
    - Group Logo
    - Team member names with their Scenario/Subsystem
  - Subfolders for each scenario to organize the pages related to each scenario. Ensure there is a *default page* for each subfolder
  - Site layout must include working navigation to default pages for each subsystem as well as the team name.
  - Add project references to the class libraries when **Stage 3** is complete
  - Configure **services** and [**user secrets**](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows) (e.g.: *connection string* values) for each subsystem when **Stage 3** is complete
    - Document the names and purposes of the **user secrets** in the `ReadMe.md` at the root of the repository

<!-- 
> **Disabling SSL** - When you create the Web Application, be sure to set up *Individual User Accounts*. Note that doing so will force the web application to "Configure for HTTPS". You must disable SSL for your project to work in the labs.
>
> ![Project Setup](./ProjectSetup.png)
>
> To disable SSL, first change the web application's project properties to use an `http://` URL for the web; when you save these changes, click "Yes" for creating a virtual directory.
>
> ![Web Project Settings](./ChangeToHttp.png)
>
> Lastly, change the "SSL Enabled" setting to "False".
>
> ![Disable SSL](./ChangeToDisableSSL.png)
-->

## Stage 2 - Security Setup

Using the techniques discussed in class, customize the default authentication that was generated when the project was first set up.

> ***NOTE:** Follow the [step-by-step instructions](./Addendum/ReadMe.md) for this semester's handling of security.


<!-- RESTORE NEXT SEMESTER

- [ ] [**Application User customization**](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-5.0#customize-the-model)
  - Add a nullable reference to the Employee's ID
- [ ] **Seed the database**
  - Add all the employees as users in the database
  - Use a default password generated from a *user secret*
  - Generate usernames in the form of `firstName.lastName`
  - Generate emails in the form of `firstName.lastName@eBikes.edu.ca`
- [ ] **Customize the User Experience**
  - Remove the ability for users to register on the site
- [ ] **Set an Authorization Policy**
  - Authorize the logged-in user (*Employee*) based on the values in the `Positions` table of the database
-->

## Stage 3 - Subsystem Setup

Each team member must create a **class library project** for their subsystem. In that library, reverse engineer the database including only the tables relevant for your scenario. Ensure the generated Entity and DbContext classes are changed from being `public` to being **`internal`**.

----

*Back to the [General Instructions](./ReadMe.md)*
