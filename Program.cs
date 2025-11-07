

//********************************************************* start of file and entry into the program *******************************************//

//-------------------------------- start of imports -------------//
using Microsoft.EntityFrameworkCore;
using Programming_7312_Part_1.Data;
using Programming_7312_Part_1.Services;
//---------------------------------- end of imports -----------//

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ REFERENCES, AI DECLERATION AND CURRENT ISSUES @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@//

 ///////////////////////////////////////////////////////////// REFEREMCES USED FOR THIS PROJECT //////////////////////////////////////////////////////////////////
/*
 
 *Brevo.com. (2025). Brevo | Email Marketing Software, Automation & CRM. [online] Available at: https://www.brevo.com/?gad_source=1&gad_campaignid=23086271243&gbraid=0AAAAAp4YiPJM7xOPrXsppAUZnGIAu-LvQ&gclid=Cj0KCQjw6bfHBhDNARIsAIGsqLjfE68ZXXIQRwSdE_GX3qLjhs0mMFeMj1Idn_ShXWA3lIatJ3mTXQQaArjtEALw_wcB [Accessed 11 Oct. 2025].

 * GeeksforGeeks (2024). Queue Data Structure. [online] GeeksforGeeks. Available at: https://www.geeksforgeeks.org/dsa/queue-data-structure/.

 *GeeksforGeeks (2024). Stack Data Structure. [online] GeeksforGeeks. Available at: https://www.geeksforgeeks.org/dsa/stack-data-structure/.


 *Caleb Curry (2024). SQLite Introduction - Beginners Guide to SQL and Databases. [online] YouTube. Available at: https://www.youtube.com/watch?v=8Xyn8R9eKB8.


 *Sajjaad Khader (2025). Data Structures Explained for Beginners - How I Wish I was Taught. [online] YouTube. Available at: https://www.youtube.com/watch?v=O9v10jQkm5c.


 * W3Schools (2019). AJAX Introduction. [online] W3schools.com. Available at: https://www.w3schools.com/xml/ajax_intro.asp.


 *SendGrid. (2024). Email API - Start for Free | SendGrid. [online] Available at: https://sendgrid.com/en-us/solutions/email-api.


 *Code With RaiGenics (2021). Sorted list and sorted dictionary in C# | C# Collection Part 8. [online] YouTube. Available at: https://www.youtube.com/watch?v=QGw9ozFgSw0.


 *Tech With RGenics (2021). C# List with example | List in C# | C# Collection Part 5. [online] YouTube. Available at: https://www.youtube.com/watch?v=bJEVvhcW8GU [Accessed 8 Oct. 2025].


 *Tech With RGenics (2021). Stack in C# with real time example | C# Collection Part 10. [online] YouTube. Available at: https://www.youtube.com/watch?v=lzfNSP0DYeE&list=PL_xlJum5pRdDC-SCtuLI0D_m4KJcIsCLq&index=10 [Accessed 8 Oct. 2025].


 *Tech With RGenics (2021). C# Collection Best Practices | Collection in C# | C# Collection part 15. [online] YouTube. Available at: https://www.youtube.com/watch?v=Ks0dDWNVURc&list=PL_xlJum5pRdDC-SCtuLI0D_m4KJcIsCLq&index=16 [Accessed 2 Oct. 2025].


 *Tech With RGenics (2021). C# Dictionary with example | Dictionary in C# | C# Collection Part 6. [online] YouTube. Available at: https://www.youtube.com/watch?v=tx_JNYL0Img&list=PL_xlJum5pRdDC-SCtuLI0D_m4KJcIsCLq&index=6 [Accessed 7 Oct. 2025].


 */

/*
 *
 **************************************************************** AI DECLERATION ****************************************************************
 * I Kashvir Sewpersad, the student , ST10257503,  hereby declare that i have made use of artifical intelligence in the creation of this project 
 * The nature of my usage has primarily been for debugging issues, either logical or syntaxed based, improving functionality, assistance with ui elements (front end) and overall improving the quality of the work i produce 
 * i have also used AI to gain a better understanding of concepts, specifically the data structures and algoryths implemented
 *NOTE : THE UI WAS IMPROVED BY AI SPECIFICALLY FOR ADDING ANIMATIONS
 * NOTE : THE CONTACT -> WHICH IS NOT RELEVANT TO THIS POE, was develped along with AI 
 * 
 * below are links to my ai usage
 * https://chatgpt.com/share/68ee7ec7-2df8-8010-ac96-18b83ad84849
 * https://chatgpt.com/share/68ee805a-b258-8010-b34b-a8a98b18cf4b
 * https://chatgpt.com/share/68ee814e-ddfc-8010-b2fa-da3e1cbcbd9f
 * https://chatgpt.com/share/68ee83d0-6d94-8010-995f-c50d8a2bc85b
 *
 * **********************************************************************************************************************************************
 *
 * ------------------------------------------------ current issues --> TO BE FIXED IN PART 3 OR REMOVED IN PART 3 ---------------------------------------------------
 * 
 * ----------------------------------------------- the search event (not required asper poe  ) is not functioning correctly on local events
 * ----------------------------------------------- the images (not required as per poe ) displaying on local events are not static at the moment --> i tried to have it so when a user loggs into the system they can see diffrent events each time, this has colided with the recomendation and i made a mess of it
 * ----------------------------------------------- the admin cannont add an event if the active box is ticked ( not required as per poe )
 * ----------------------------------------------- the recomendation algorythem does not display searched event right on top of the paage, it is displaying correctly, just not at the right position on the page 
 * 
 */

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@// 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Register DbContext
// im using a sqlite db to store all of the events added by an admin. 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our services
builder.Services.AddScoped<IssueStorage>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<AnnouncementService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//**************************************************** end of program and program **************************************************//
