# [CLICK HERE FOR CLASS VIDEO](https://naitca.sharepoint.com/teams/DMIT2018-IntermediateApplicationDevelopment20212022Winter500/Shared%20Documents/Forms/AllItems.aspx?FolderCTID=0x0120005E27AD6FEB963D4CBD10994177543698&id=%2Fteams%2FDMIT2018%2DIntermediateApplicationDevelopment20212022Winter500%2FShared%20Documents%2FGeneral%2FRecordings%2FView%20Only%2FDMIT2018%2D1212%2DOA02%2D20220215%5F130433%2DMeeting%20Recording%2Emp4&parent=%2Fteams%2FDMIT2018%2DIntermediateApplicationDevelopment20212022Winter500%2FShared%20Documents%2FGeneral%2FRecordings%2FView%20Only)
# BACK END Set up for you 

 1. Ensure we have database install
 1. Add Class Library
 1. Add BLL, DAL and Entities folder
 1. New backend project "theName"System, Eg if "theName" is Chinook, the complete system name would be **ChinookSystem**
 1. Add in dependecy to Web app 
 1. Click on back end Sytstem EF Core Power Tools > Reverse Engineer
 1. Click on red X near the add field
 1. Make sure you select EF Core 6 in the bottom Left
 1. Add > CHoose Data source "." or "(localhost)" > click ok bottom right
 1. Select Tables you need for your individual back end system. Ignore STORE PROCEDUREs
 1.  Context Name is "yourSubSystemName"+System 
 1. EntityTypes put Entities, 
    - ![image](https://user-images.githubusercontent.com/22464805/159991087-74f40710-a4a3-4e89-9505-23b1ad428862.png)
 1. Under DbContext path put DAL 
    - ![image](https://user-images.githubusercontent.com/22464805/159991431-47ccbbb1-9953-4355-9342-da6b673f2631.png)

 1 What to generate: Select EntityTypes and DbContext
    - ![image](https://user-images.githubusercontent.com/22464805/159991231-ec7b9e72-afab-4583-9b36-0888cfbda155.png)

 1 Under Naming, check off "Pluralize or singluraize generated object names (English), and the rest highlighted on the images
   - ![image](https://user-images.githubusercontent.com/22464805/159990887-16449909-4728-4669-b828-a34299648454.png)

 1. Under Naming check of "Use Table and Column Names directly 
# IT SHOULD LOOK LIKE THIS BEFORE YOU CLICK OKAY
![image](https://user-images.githubusercontent.com/22464805/159989273-12da4d69-99ee-4f29-8236-8efba6e65d11.png)

# Remember to do these 
> These steps are not as good as the previous ones, It might be missing a step, Sorry about that
> 
 2. Change all Entity classes to internal under the **Entities** Folder
 2. UNDER Program CS, add in connection string you added
   3. 
    - ![image](https://user-images.githubusercontent.com/22464805/159992485-97b2d0e9-9c8a-495f-beb3-82e968c42c4a.png)
   3. ![image](https://user-images.githubusercontent.com/22464805/159992654-3ca35912-8ed2-40f2-b706-acb5f68f4f07.png)
   3. ![image](https://user-images.githubusercontent.com/22464805/159993089-11d719f9-ae61-428c-9388-a91bce6215b8.png)


 2. Add Dependencies to the WebApp Project
 2. Add in Connection String to Program.CS
 2. Add In Servicing Dependencies
 
