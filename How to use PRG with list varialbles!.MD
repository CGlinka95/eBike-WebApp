I spent all night on this, so forgive me if it doesn't make sense, I just need to put this down before I forget
I'm guessing you have a IActionResult  doing on OnPost in your page models like herre. Here is how I implemented it on ErrorDetails
## Step 1: Define the IActionResult On Post method first, in my case it was the OnPostRecieve()

![image](https://user-images.githubusercontent.com/22464805/164243217-550a69c4-749e-419e-a4c9-b00ffb784367.png)
## Step 2: Define your List<> of whatever in your PageModel class property in your Page Model. In my case it is was a List<string> called ErrorDetails
![image](https://user-images.githubusercontent.com/22464805/164243712-47c64e32-132c-42b1-80a3-c3caad6280a8.png) 
> Don't put any Annotations like [BindProperty] LEAVE THOSE OUT
## Step 3: Do: a) Manipulate the data however you want, in my case it was a list of error messages I put inside my ErrorDetails. via foreach loop
  
  ![image](https://user-images.githubusercontent.com/22464805/164244933-5fd9d116-e8cd-4c32-8041-34f3cc57cd77.png)
  
## Step 4: return a RedirectToPage(), and pass in a annyonymous type into RedirectPage. 
  > THE ANNONYNOUS TYPE MUST HAVE YOUR LIST IN IT TO WORK
  > Mine has some other properties I have to pass in, but just focus on how I did ErrorDetails
  > ** NOTICE: I passed in ErrorDetail as ErrorDetails. THis is important that the name is unique. **
  ![image](https://user-images.githubusercontent.com/22464805/164246204-fdb35fcb-4c10-47c7-9296-23afc1068d5e.png)

## Step 5: Add a parameter to your OnGet method. the name of the property you passed in, must be the same as the parameter in the OnGet

  ### "ErrorDetail" is the kind of variable AND has the exact same spelling as "ErrorDetail" we passed into the RedirectToPage()
  ![image](https://user-images.githubusercontent.com/22464805/164246899-7ffef3f4-ac01-49dc-bd57-4b8cbc5c9a35.png)
  ### Notice In the photo bellow, that ErrorDetail = ErrorDetails, with an S at the end! Your OnGet must have the same parameter in name and Data TYPE
  ![image](https://user-images.githubusercontent.com/22464805/164247761-6e637fc7-907d-4b36-bcbf-58f84f71c018.png)
### In my case, The parameter is called "ErrorDetail" in my OnGet, and when I pass it into my RedirectToPage()
  
  
## Step 6: inside of your OnGet, set it to one your properties of your PageModel Class
  ![image](https://user-images.githubusercontent.com/22464805/164248650-01afdedb-a382-4069-8246-24e7d01e86ef.png)
![image](https://user-images.githubusercontent.com/22464805/164248724-cdaf2ee5-2808-471d-8c1d-e258e0fccf84.png)
## Step 7:: Implement it in your cshtml !
  ![image](https://user-images.githubusercontent.com/22464805/164248888-29b528e2-dd4f-4b39-8537-57e753a8c772.png)
