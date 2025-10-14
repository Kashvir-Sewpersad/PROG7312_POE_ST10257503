
//********************************************************** start of file ***************************************************************//

//--------------------------------- start of imports -----------------------//
using Programming_7312_Part_1.Models;
using System.Collections.Generic;

//----------------------------------- end of imports --------------------------------//
namespace Programming_7312_Part_1.Services
{
    public class IssueStorage
    {
        private int _nextId = 1; // id variable 
        
        /*
         * linked list to match poe requirements for data structure usage 
         * 
         */
        public LinkedList<Issue> ReportedIssues { get; } = new LinkedList<Issue>(); // linked list for the reported issues

        public void AddIssue(Issue issue)
        {
            issue.Id = _nextId++; // increment 
            
            ReportedIssues.AddLast(issue); // added to the reported issues 
        }
        /*
         *
         * the below method is used ti retrive the issues by id
         *
         * this makes sence because each issue is granted a unique id
         *
         *  the unque id comes from the fact that the ids are incremented by 1  before being saved into the system
         */
        public Issue GetIssueById(int issueId)
        {
            return ReportedIssues.FirstOrDefault(i => i.Id == issueId);
        }
        /*
         *
         * the below is a 1 / 0 for the upvote a issue. the goal here will be to reccord the upvotes based on an issue (reported)
         *
         * this in terns shall be used to increase or decrease priority
         *
         *  a high upvote = high priority
         *
         * a low upvote = low priority 
         * 
         */
        public bool UpvoteIssue(int issueId)
        {
            var issue = GetIssueById(issueId);
            if (issue != null)
            {
                issue.Upvotes++;
                return true;
            }
            return false;
        }
    }
}
//****************************************************** end of file *****************************************************************//