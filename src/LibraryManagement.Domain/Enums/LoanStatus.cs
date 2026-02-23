namespace LibraryManagement.Domain.Enums;

public enum LoanStatus
{
    Pending = 0,        
    Approved = 1,       
    Active = 2,         
    Returned = 3,       
    PendingReturn = 4,  
    Overdue = 5,        
    Rejected = 6        
}