# EDConlonizationTracker
Tracking resources needed for Colonization

# Beginning notes
This reads your local journal file and determines if you are moving cargo to a carrier. It will then update a specific Google Sheets with the corresponding value

# Prerequisites
1. First and foremost, you need a fleet carrier  
2. Second you'll need to have your data in a Google Sheet and it needs to be in a specific format see screenshot
![image](https://github.com/user-attachments/assets/287f7e56-7f00-45ab-a3b2-259d9f535050)  
*Note I highly reccommend setting your B Column as an equation of =total-CRow to automatically reduce the number in Column B*
*Example Column B row 1 would be =22-C1*  
*The application is going to update Column C is why*

# Setup
1. You'll need to turn on your Google Sheet API [here](https:\\console.google.cloud)
2. Click the `APIs & Services`
   ![image](https://github.com/user-attachments/assets/def5eb5d-a0e5-437c-a6f3-7711e5495d8c)
3. Search for `Google Sheets API` Once the results show up you should be looking for this  
 ![image](https://github.com/user-attachments/assets/d8c37a3e-0635-4131-9b15-e4b136885190)
4. Once you click on that link click the blue button that says `Activate`  
4a. If it doesn't already take you to the details of that, click the blue Manage button  
  ![image](https://github.com/user-attachments/assets/3f274be0-dfe0-4011-b888-61084064c241)
5. Go to `Creditials` in the left hand menu
   ![image](https://github.com/user-attachments/assets/f5c3b052-8955-4b4e-a8c6-0d03df8ac02c)
6. Once in here click the `+ CREATE CREDENTIALS` at the top of the page
  ![image](https://github.com/user-attachments/assets/60cec8d5-77ba-4605-b027-98e458b9d6e9)
7. Click `Service Account`
    ![image](https://github.com/user-attachments/assets/650bc677-cc5d-4aae-a455-b703c912cd84)
8. Fill out this form. It doesn't matter too much what goes in these, but in case you want to make changes to them later, would be a good idea to name it something you'll know what it's for later.
    ![image](https://github.com/user-attachments/assets/fc5a1cc0-f052-4bf6-80a3-5d7ebb35f299)
9. Once you click done it should take you back to the `Creditials` page and the bottom grid should have your new service account. Click it.
  ![image](https://github.com/user-attachments/assets/7dd06756-f2c8-401a-8663-271d808add01)
10. Once in here you want to copy the `Email` that is was generated to this account **Note: don't close this page, you'll still need it in a bit**
    ![image](https://github.com/user-attachments/assets/588a118f-f0c8-4083-afb3-09ae382d71e2)
11. Go to your Google Sheet spreadsheet and click the Share button at the top right of the page
  ![image](https://github.com/user-attachments/assets/c1ac0f33-6adb-4240-a887-c6c2acc0ab5c)
12. Paste the email you just copied in the box and in the dropdown next to your newly pasted email, select `Editor`
  ![image](https://github.com/user-attachments/assets/d04ed8f7-5de7-467b-b582-8ade453766a7)
13. Back to the Service Account details page you shouldn't have closed yet, click the `Keys` at the top of the page
    ![image](https://github.com/user-attachments/assets/31114eff-0ab1-4a4c-a28c-37a71afb05dd)
14. Click the `Add key` button 
  ![image](https://github.com/user-attachments/assets/22c0ecf7-08b4-44d6-9f68-d4c17ef28da9)
15. Click `Create new key`
    ![image](https://github.com/user-attachments/assets/7c4e7cd1-a61f-4a74-925a-bd034ff930bb)
16. Make sure `JSON` is selected and click `Create`
    ![image](https://github.com/user-attachments/assets/166681a5-cd23-4513-91b2-a209d0331ebf)
17. This will download a .json file. You'll need to create a folder under `My Documents` on your computer called `GoogleSheetsApi` and move the downloaded file to that folder

# Running the app
**IMPORTANT: Launch the game before launching the app. It's using your latest journal file and you get a new file everytime you launch the game. If you launch the game second, it won't get any new data and the app won't work**
Once the app starts a console app will appear and ask for the SpreadsheetId. This is in the URL of your spreadsheet.  
It's what is between the slashs after the `/d/` and before the `/edit?gid=`  
![image](https://github.com/user-attachments/assets/3faf90f2-a946-4c17-be07-c2651300b7a1)
This is how the app knows what spreadsheet to update
Then it will ask for the Workbook Name. This is the tab at the bottom of the screen. In the example below it would be `Space Farm`  
![image](https://github.com/user-attachments/assets/19b6b56e-9853-462a-b5bd-88cdd474c3f9)
Once you give it that, it's locked, cocked and ready to rock!









